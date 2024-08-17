using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EliminateManagement:MonoBehaviour
{
    public List<GameObject> gameObjects = new List<GameObject>();
    public GameObject ItemsPanel;
    public GameObject EliminateItemPrefab;
    public GameObject CoveringAllPanel;//这个就是挡住全部，只能看，不能点其他，因为toggl而无法挡
    //提示
    public GameObject TipPanel;
    public Text TipMessageText;//我们要修改尺寸，因为位置不够，不能放大了
    //int InititalFontSize;//转移到GlobalTransferStation，方便提示面板，关闭后自动复原原字体

    public double Eliminate_AllOdds = 0;

    GlobalTransferStation transferStation;
    GameObject newGameObject;
    EliminateItem Item;
    
    double CurrentRaffleOdds;
    double Sum;//累计概率
    EliminateItem RaffleMessageItem;
    int i;

    //int NotCheckedItemCount;//记录还有多少个没选的


    void Start()//不使用awake，防止存档读取的时候，另一个awake还没执行完，导致错误
    {
        transferStation = GlobalTransferStation.Instance;

        Debug.Log("第一次Eliminate_AllOdds:" + Eliminate_AllOdds);
        for (int i = 0; i < ItemsPanel.transform.childCount; i++)
        {
            Debug.Log(ItemsPanel.transform.GetChild(i).name);
            gameObjects.Add(ItemsPanel.transform.GetChild(i).gameObject);
        }

        //读取存档：
        for(int i = 0;i<transferStation.ListEliminateItemData.Count ;i++ )
        {
            CreateNewItemByData(transferStation.ListEliminateItemData[i]);
        }
       
    }

    //读档直接创建,改为父级物体名字判断，保证读档后位置不变
    public void CreateNewItemByData(EliminateItemData NewItem)
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (NewItem.FatherName == gameObjects[i].name)
            {
 
                newGameObject = Instantiate(EliminateItemPrefab, gameObjects[i].transform);
                newGameObject.transform.SetParent(gameObjects[i].transform);//预制件xyz三值必须初始化为0，不然跟父级物体不重合

                Item = newGameObject.GetComponent<EliminateItem>();
                newGameObject.SetActive(true);//好像默认是关的，必须重新打开
                gameObjects[i].SetActive(true);//父级
                NewItem.SetEliminateItem(Item);
                Item.SetContent();
                if (Item.itemData.IsChecked)//读档的时候，判断是否已经被选了，然后呈现被选了的图片
                {
                    Item.Checked();
                }
                else
                {
                    Item.OverChecking();
                }
                //ItemData不用交付数据管理中心，因为已经存在了,但是脚本要，因为这个是新的
                transferStation.ListEliminateItem.Add(Item);
                //不用重新计算总概率，因为总概率，已经直接保存了 }
                break;
            }

        }
    }

    //重新计算总概率，通过添加选项和删除选项调用。
    public void SetAllOddsAndFinalOdds()
    {
        if(transferStation.ListEliminateItem.Count != 0)
        {
            //计算总概率,必须先将AllOdd归0
            Eliminate_AllOdds = 0;
            for (i = 0; i < transferStation.ListEliminateItem.Count; i++)
            {
                if (transferStation.ListEliminateItem[i].itemData.IsChecked)//跳过已经选过的
                {
                    continue;
                }
                Eliminate_AllOdds = Eliminate_AllOdds + transferStation.ListEliminateItem[i].Odds;
            }
            Debug.Log("Eliminate_AllOdds:" + Eliminate_AllOdds);
            //通过比例的情况，重新分配最终概率
            for (i = 0; i < transferStation.ListEliminateItem.Count; i++)
            {
                if (transferStation.ListEliminateItem[i].itemData.IsChecked)//跳过已经选过的
                {
                    continue;
                }
                transferStation.ListEliminateItem[i].FinalOdds = transferStation.ListEliminateItem[i].Odds / Eliminate_AllOdds;
                Debug.Log("分配完FinalOdds:" + transferStation.ListEliminateItem[i].FinalOdds);
            }
            //淘汰的话，前缀和收效甚微，不采纳
        }

    }

    public bool Raffle()//相当于false，表示没创建
    {

        if (transferStation.ListEliminateItem.Count == 0)//防止没有创建选项也要抽
        {
            return false;
        }
        //要进行判断是否创建了，如果没创建，不可以抽奖
        CurrentRaffleOdds = Random.Range(0F, 1F);
        Debug.Log(CurrentRaffleOdds);

        Sum = 0;//归0重置
        //非前缀和方法，常用循环+判断方法，叠加就不用弄区间，不过区间也是这样一个个判断，
        //由于sum已经赋值第一个了，所以我们从下标1开始，也就是第二个
        for ( i = 0;i< transferStation.ListEliminateItem.Count; i++)
        {
            if (transferStation.ListEliminateItem[i].itemData.IsChecked)//跳过已经选过的
            {
                continue;
            }

            Sum = Sum + transferStation.ListEliminateItem[i].FinalOdds;

            if (CurrentRaffleOdds <= Sum)
            {
                RaffleMessageItem = transferStation.ListEliminateItem[i];
                //Debug.Log("sum：" + sum);
                return true;
            }
        }
        return false;
    }

    public void StartRaffle()//一次抽奖
    {
        CoveringAllPanel.SetActive(true);//这里开始挡

        if (transferStation.RaffleIsRuning == false)
        {
            transferStation.TipMessageText = "抽到：\n";
            if (Raffle())
            {
                transferStation.TipMessageText = transferStation.TipMessageText + "          " + RaffleMessageItem.Content;
            }
            else
            {
                transferStation.TipMessageText = "未创建可抽选选项，\n或者已经被抽取过了，\n打开对应选项信息面板，修改即可取消已选状态";
                TipPanel.SetActive(true);
                CoveringAllPanel.SetActive(false);//这里也要结束
                return;
            }

            //搞一下动画
            StartCoroutine(RaffleAnimation());
        }

    }

    IEnumerator RaffleAnimation()
    {
        transferStation.RaffleIsRuning = true;
        //
        //最终时间基本以1f秒左右为准，到了0.5秒的时候就要减速了，
        float interval = 0.1f; // 间隔时间
        float speed = 0.02f;
        int Num = 0;
        bool determine = false;
        //bug：最后一个，由于num为48，太少，其他选项被选，导致跳过也num++，留给真正的时间加载次数过少，导致时间不够，num尽量大一点，别低于100最好
        for(int i =0;Num< 200; i++,Num++ )//走两百步，一定要多，不然太少num会跳出去，导致没有概率重算和设置已选标志
        {
            //transferStation.ListEliminateItem[i-1].OverChecking();//
            if (i >= transferStation.ListEliminateItem.Count)
            {
                i = 0;
            }
            //跳过选过的
            if (transferStation.ListEliminateItem[i].itemData.IsChecked)//跳过已经选过的
            {
                continue;
            }

            //
            transferStation.ListEliminateItem[i].Checking();
            Debug.Log("0:" + RaffleMessageItem.transform.parent.name);
            //速度增加变慢一点
            if (interval >= 0.5f)
            {
                speed = 0.01f;
                determine = true;
            }
            Debug.Log("1:" + RaffleMessageItem.transform.parent.name);
            //唯一值，只有父级物体是唯一，就可以拿来判断
            if (determine && RaffleMessageItem.transform.parent.name == transferStation.ListEliminateItem[i].transform.parent.name)
            {
                yield return new WaitForSeconds(0.75f);//停留时间，或者延迟时间;

                //RaffleMessage和transferStation.ListEliminateItem[i]都是同一个对象，以下直接用RaffleMessageItem了，字符少一点
                RaffleMessageItem.Checked();//方法里面有标记已经选了，我们到时候读档会对这些进行操作
                //标记后，概率重算，会跳过有标记的
                SetAllOddsAndFinalOdds();//概率重算
                Debug.Log("2:"+RaffleMessageItem.transform.parent.name);
                transferStation.createOrLoadData.SaveData();//保存
                Debug.Log("3:" + RaffleMessageItem.transform.parent.name);
                break;
            }

            interval = interval + speed;
            yield return new WaitForSeconds(interval);//停留时间，或者延迟时间
            transferStation.ListEliminateItem[i].OverChecking();
        }
        //
        transferStation.RaffleIsRuning = false;
        CoveringAllPanel.SetActive(false);//这里开始结束
        TipPanel.SetActive(true);
        yield return null;
    }
}
