using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


//该类为非淘汰抽奖创建可抽奖项，挂载在NotEliminatePanel，方便存档，不然CreatePanel没激活，用不了这个脚本
public class CreateEliminateItem:MonoBehaviour
{
    //需要这个物体附着的脚本中搜集的全部父级物体，这些父级物体的作用就是为了保证位置的准确，长宽跟预制件一样
    //创建的时候就找到未激活的父级物体，然后让预制件实例化，然后预制件就设置属于这个物体的子级
    public GameObject EliminatePanel;//调用它脚本的方法SetAllOddsAndFinalOdds()，重新计算总概率

    public GameObject CreateItemPanel;//创建后，关闭当前输入窗口
    //预制件
    public GameObject EliminateItemPrefab;
    //这两个物体的输入信息要清空，一旦创建成功的话
    public GameObject InputField_Content;
    public GameObject InputField_Odds;
    //提示
    public GameObject TipPanel;
    //
    string Content;
    string Odds;
    GlobalTransferStation transferStation;
    List<GameObject> gameObjects;
    GameObject newGameObject;
    EliminateItem Item;

    int i = 0;

    //直接顺便判断是否已满
    void OnEnable()
    {
        gameObjects = EliminatePanel.GetComponent<EliminateManagement>().gameObjects;
        transferStation = GlobalTransferStation.Instance;

        //循环判断是否都激活了，没位置了
        //判断通过的话，执行创建,也就是按顺序激活，谁空，谁就激活，然后写入数据，由于数据量少，直接循环即可，不用优化
        for (i = 0; i < gameObjects.Count; i++)
        {
            if (!gameObjects[i].activeSelf)//如果是true，则已经激活，false，则代表没激活，可以使用,所以取反即可
            {
                //此时的i有大用，绝对不可以改变，否则会影响到后面预制件的创建，放在谁的子级里面
                break;//直接跳出，保证i不再++
            }
        }
        if(i == gameObjects.Count)
        {
            //共用一个tip面板，节省开销，
            transferStation.TipMessageText = "位置已满，不可以再添加哦，可以选择删除某个选项";
            TipPanel.SetActive(true);

            //同时悄悄创建脚本关掉
            CreateItemPanel.SetActive(false);
        }
    }

    void OnDisable()
    {
        InputField_Content.GetComponent<InputField>().text = "";
        InputField_Odds.GetComponent<InputField>().text = "";
    }
    public void CreateNewItem()
    {
        Content = InputField_Content.GetComponent<InputField>().text;
        Odds = InputField_Odds.GetComponent<InputField>().text;

        if (Content == "")
        {
            Debug.Log("Content:" + Content);
            transferStation.TipMessageText = "内容不可以为空";
            TipPanel.SetActive(true);
            return;
        }
        else if (Odds == "")
        {
            transferStation.TipMessageText = "概率不可以为空";
            TipPanel.SetActive(true);
            return;
        }
        else if (double.Parse(Odds) > 1 || double.Parse(Odds) <= 0)
        {
            transferStation.TipMessageText = "概率不可以大于1或者等于小于0";
            TipPanel.SetActive(true);
            return;
        }
        //判断转移到OnEnable()，此时i下标，就是未激活的物体,已经判断完了，直接用
        newGameObject = Instantiate(EliminateItemPrefab, gameObjects[i].transform);
        newGameObject.transform.SetParent(gameObjects[i].transform);//预制件xyz三值必须初始化为0，不然跟父级物体不重合

        Item = newGameObject.GetComponent<EliminateItem>();
        newGameObject.SetActive(true);//好像默认是关的，必须重新打开
        gameObjects[i].SetActive(true);//父级
        Item.Content = Content;
        Item.Odds = double.Parse(Odds);
        Item.SetContent();

        //脚本，交付数据管理中心
        transferStation.ListEliminateItem.Add(Item);
        //重新计算总概率
        EliminatePanel.GetComponent<EliminateManagement>().SetAllOddsAndFinalOdds();

        //为内数据类的类赋值，等计算完总概率才可以执行，不然finalOdds是空的，默认为0。
        Item.itemData = new EliminateItemData();
        Item.itemData.SetEliminateItemData(Item);
        //可序列化类完成赋值再交付给数据管理中心
        transferStation.ListEliminateItemData.Add(Item.itemData);

        //创建新选项的时候，顺便调用存档，就是做一个选项立刻存档
        transferStation.createOrLoadData.SaveData();

        //关闭当前窗口
        CreateItemPanel.SetActive(false);
    }
}
