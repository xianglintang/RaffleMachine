using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//该脚本为最高级物体附带，如果没有存档，那么就创建存档，如果有存档，那么自动读取存档
//由于物体是最高级别，所以GlobalTransferStation可以承接预制件想要打开的页面
public class CreateOrLoadData:MonoBehaviour
{
    //物体
    public GameObject NotEliminateItemInformationPanel;
    public GameObject EliminateItemInformationPanel;
    //根据以下物体的激活情况，判断谁需要排序
    public GameObject EliminatePanel;

    string DataPath = "-1";
    DataManagement dataManagement;

    GlobalTransferStation transferStation;

    public void Awake()
    {
        CreateDataPath();
        if(DataPath != "-1")
        {
            //SaveData();
            LoadData();
        }

        transferStation = GlobalTransferStation.Instance;
        transferStation.createOrLoadData = GetComponent<CreateOrLoadData>();//类似委托，我们直接把脚本对象交付出去，就不用访问物体才能再访问脚本
        //物体赋值
        transferStation.NotEliminateItemInformationPanel = NotEliminateItemInformationPanel;
        transferStation.EliminateItemInformationPanel = EliminateItemInformationPanel;
    }
    //如果存在，那么就没事，不存在就要创建
    private void CreateDataPath()//顺便创建存档
    {
        // 获取目标文件夹路径
        string targetFolderPath = Path.Combine(Application.persistentDataPath, "RaffleMachineData");

        // 检查目标文件夹是否存在
        if (Directory.Exists(targetFolderPath))
        {
            // 获取目标文件夹中的所有文件路径  
            string[] filePaths = Directory.GetFiles(targetFolderPath);
            //只要第一个路径即可
            if(filePaths.Length != 0)
            {
                DataPath = filePaths[0];
            }
            else
            {
                DataPath = GetSavePath();//创建新存档路径
                //SaveData();//并且创建一个新文件,只需要路径即可，文件不用创建，因为其他界面还没有执行Start搜集里面的物品
            }
            
        }
        else
        {
            Directory.CreateDirectory(targetFolderPath);
            DataPath = GetSavePath();//然后创建存档路径

        }

    }
    //每个操作都会调用这个存储方法
    //由于淘汰机制，如果删除了又创建，那么动画轮播，会导致顺序不对，所以我们要对淘汰集合进行一次排序，
    //我们根据淘汰面板的激活情况，才觉得排序，非淘汰抽奖，没必要重新排序
    public void SaveData()
    {
        //先排序
        if (EliminatePanel.activeSelf)
        {
            EliminateItem mid;
            int namei,namej;
            for (int i = 0; i < transferStation.ListEliminateItem.Count; i++)
            {
                Debug.Log("transferStation.ListEliminateItem[i].transform.parent.name:" + transferStation.ListEliminateItem[i].transform.parent.name);
                for (int j =i+1;j< transferStation.ListEliminateItem.Count; j++)
                {
                    //应该是父级物体，而不是预制件本身
                    namei = int.Parse(transferStation.ListEliminateItem[i].transform.parent.name);
                    namej = int.Parse(transferStation.ListEliminateItem[j].transform.parent.name);
                    if (namei > namej)
                    {
                        mid = transferStation.ListEliminateItem[i];
                        transferStation.ListEliminateItem[i] = transferStation.ListEliminateItem[j];
                        transferStation.ListEliminateItem[j] = mid;
                    }
                }
            }
            //不用排序也不用清空，直接从transferStation.ListEliminateItem里面直接覆盖赋值。
            for(int i = 0; i < transferStation.ListEliminateItem.Count; i++)
            {
                transferStation.ListEliminateItemData[i] = transferStation.ListEliminateItem[i].itemData;
                //由于发现，脚本里面的data类，final只赋值了一次，后续不再修改，如果关闭软件再读档，那么概率是不对的，比例将会不对
                //我们重新赋值，因为Item改了，但是ItemData没改
                //趁赋值，我们顺便也修改
                transferStation.ListEliminateItemData[i].FinalOdds = transferStation.ListEliminateItem[i].FinalOdds;
            }

        }
        else//不是它调用，那么就是非淘汰调用，对final重新赋值
        {
            for (int i = 0; i < transferStation.ListNotEliminateItem.Count; i++)
            {
                transferStation.ListNotEliminateItemData[i].FinalOdds = transferStation.ListNotEliminateItem[i].FinalOdds;
            }
        }


        //赋值
        dataManagement = new DataManagement();
        dataManagement.SetDataManagement();

        //根据路径，写入文件
        string jsonData = JsonUtility.ToJson(dataManagement);//转换json格式
        File.WriteAllText(DataPath, jsonData);//覆盖该文件
    }
    public void LoadData()
    {
        if (File.Exists(DataPath))
        {
            string jsonData = File.ReadAllText(DataPath);//读取文件
            DataManagement data = JsonUtility.FromJson<DataManagement>(jsonData);

            //赋值给全局中转站
            data.SetGlobalTransferStation();
        }

    }

    //固定存档路径，代表存档只能存在一个
    private string GetSavePath()
    {
        return Application.persistentDataPath + "/RaffleMachineData/" + "DataName" + ".json";
    }
}
