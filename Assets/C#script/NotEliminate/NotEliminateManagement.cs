using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotEliminateManagement:MonoBehaviour
{
    public List<GameObject> gameObjects = new List<GameObject>();
    public GameObject ItemsPanel;
    public GameObject NotEliminateItemPrefab;
    //提示
    public GameObject TipPanel;
    public Text TipMessageText;//我们要修改尺寸，因为位置不够，不能放大了
    //int InititalFontSize;//转移到GlobalTransferStation，方便提示面板，关闭后自动复原原字体

    public double NotEliminate_AllOdds = 0;

    GlobalTransferStation transferStation;
    GameObject newGameObject;
    NotEliminateItem Item;

   // double[] PrefixSum;
    double CurrentRaffleOdds;
    double Sum;//累计概率
    string RaffleMessage;
    int i;
    void Start()//不使用awake，防止存档读取的时候，另一个awake还没执行完，导致错误
    {
        transferStation = GlobalTransferStation.Instance;

        for (int i = 0; i < ItemsPanel.transform.childCount; i++)
        {
            Debug.Log(ItemsPanel.transform.GetChild(i).name);
            gameObjects.Add(ItemsPanel.transform.GetChild(i).gameObject);
        }

        //读取存档：我们添加了父级物体名字变量，保证还是原本位置
        for(int i = 0;i<transferStation.ListNotEliminateItemData.Count ;i++ )
        {
            CreateNewItemByData(transferStation.ListNotEliminateItemData[i]);
        }
        transferStation.InititalFontSize = TipMessageText.fontSize;//记录初始尺寸，在提示面板脚本Disable自动复原
    }
    //读档直接创建
    public void CreateNewItemByData(NotEliminateItemData NewItem)
    {
        for(int i = 0; i < gameObjects.Count; i++)
        {
            if(NewItem.FatherName == gameObjects[i].name)
            {
                newGameObject = Instantiate(NotEliminateItemPrefab, gameObjects[i].transform);
                newGameObject.transform.SetParent(gameObjects[i].transform);//预制件xyz三值必须初始化为0，不然跟父级物体不重合

                Item = newGameObject.GetComponent<NotEliminateItem>();
                newGameObject.SetActive(true);//好像默认是关的，必须重新打开
                gameObjects[i].SetActive(true);//父级
                NewItem.SetNotEliminateItem(Item);
                Item.SetContent();
                //ItemData不用交付数据管理中心，因为已经存在了,但是脚本要，因为这个是新的
                transferStation.ListNotEliminateItem.Add(Item);
                //不用重新计算总概率，因为总概率，已经直接保存了

                break;
            }
        }
    }

    //重新计算总概率，通过添加选项和删除选项调用。
    public void SetAllOddsAndFinalOdds()
    {
        if (transferStation.ListNotEliminateItem.Count != 0)
        {
            //计算总概率,必须先将AllOdd归0
            NotEliminate_AllOdds = 0;
            for (i = 0; i < transferStation.ListNotEliminateItem.Count; i++)
            {
                NotEliminate_AllOdds = NotEliminate_AllOdds + transferStation.ListNotEliminateItem[i].Odds;
            }
            Debug.Log("NotEliminate_AllOdds:" + NotEliminate_AllOdds);
            //通过比例的情况，重新分配最终概率
            for (i = 0; i < transferStation.ListNotEliminateItem.Count; i++)
            {
                transferStation.ListNotEliminateItem[i].FinalOdds = transferStation.ListNotEliminateItem[i].Odds / NotEliminate_AllOdds;
                Debug.Log("分配完FinalOdds:" + transferStation.ListNotEliminateItem[i].FinalOdds);
            }

            //同时计算前缀和,方便十连抽，直接得出结果，而不是重新叠加计算
           /* PrefixSum = new double[transferStation.ListNotEliminateItem.Count];
            PrefixSum[0] = transferStation.ListNotEliminateItem[0].FinalOdds;//先确定第一个数
            for (i = 1; i< transferStation.ListNotEliminateItem.Count; i++)
            {
                PrefixSum[i] = transferStation.ListNotEliminateItem[i].FinalOdds + PrefixSum[i-1];
            }
            //测试
            for(i = 0; i < PrefixSum.Length;i++)
            {
                Debug.Log("PrefixSum[" + i + "]:"+PrefixSum[i]);
            }*/
        }

    }

    public bool Raffle()//相当于false，表示没创建
    {
        if(transferStation.ListNotEliminateItem.Count == 0)//防止没有创建选项也要抽
        {
            return false;
        }
        //要进行判断是否创建了，如果没创建，不可以抽奖
        CurrentRaffleOdds = Random.Range(0F, 1F);
        Debug.Log(CurrentRaffleOdds);
        /*for (int i = 0;i< transferStation.ListNotEliminateItem.Count;i++ )
        {
            if () ;
        }*/
        //[0,10],(10,20],之类的区间，第一个比较特殊，如果直接0，区别于其他区间，这个是全闭区间，
        Sum = 0;
        //非前缀和方法，常用循环+判断方法，叠加就不用弄区间，不过区间也是这样一个个判断，
        //由于sum已经赋值第一个了，所以我们从下标1开始，也就是第二个
        for (i = 0;i< transferStation.ListNotEliminateItem.Count; i++)
        {

            Sum = Sum + transferStation.ListNotEliminateItem[i].FinalOdds;
            if (CurrentRaffleOdds <= Sum)
            {
                RaffleMessage = transferStation.ListNotEliminateItem[i].Content;
                //Debug.Log("sum：" + sum);
                return true;
            }
            else
            {
                Sum = Sum + transferStation.ListNotEliminateItem[i].FinalOdds;
            }
        }
        return false;
    }

    public void StartRaffle()//一次抽奖
    {
        transferStation.TipMessageText = "抽到：\n";
        if (Raffle())
        {
            transferStation.TipMessageText = transferStation.TipMessageText + "          " +RaffleMessage;
            TipPanel.SetActive(true);
        }
        else
        {
            transferStation.TipMessageText = "未创建可抽选选项";
            TipPanel.SetActive(true);
        }

    }
    public void StartMoreRaffle()//多次抽奖,顺便修改尺寸大小，因为位置不够
    {
        //TipMessageText.fontSize = 12;//改小，因为不够尺寸
        transferStation.TipMessageText = "抽到：";
        for (int i = 0; i < 10; i++)
        {
            if (Raffle())
            {
                if ((i+1) % 2 == 0)//偶数不换行，位置不够，字体变小太奇怪了
                {
                    transferStation.TipMessageText = transferStation.TipMessageText  + "          " + (i + 1) + ":" + RaffleMessage;
                }
                else
                {
                    transferStation.TipMessageText = transferStation.TipMessageText + "\n" + "          "+(i+1) + ":" + RaffleMessage;
                }


            }
            else
            {
                transferStation.TipMessageText = "未创建可抽选选项";
                TipPanel.SetActive(true);
                return;
            }
        }

        TipPanel.SetActive(true);

    }


}
