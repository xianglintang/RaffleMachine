using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//该类为全局存档管理类，其实就是纪录中转站里面必须记录的信息
[System.Serializable]
public class DataManagement
{
    public List<NotEliminateItemData> ListNotEliminateItemData;
    public List<EliminateItemData> ListEliminateItemData;

    public void SetDataManagement()//存档
    {
        GlobalTransferStation transferStation = GlobalTransferStation.Instance;

        ListNotEliminateItemData = transferStation.ListNotEliminateItemData;
        ListEliminateItemData = transferStation.ListEliminateItemData;
       
    }

    public void SetGlobalTransferStation()//读档
    {
        GlobalTransferStation transferStation = GlobalTransferStation.Instance;

        transferStation.ListNotEliminateItemData = ListNotEliminateItemData;
        transferStation.ListEliminateItemData = ListEliminateItemData;
    }
}
