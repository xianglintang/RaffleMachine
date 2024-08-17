using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTransferStation
{
    private static GlobalTransferStation instance = null;

    //通用物体，预制件想要打开的，也就是修改预制件信息的面板
    public GameObject NotEliminateItemInformationPanel;
    public GameObject EliminateItemInformationPanel;

    public string TipMessageText;
    public int InititalFontSize;//提示面板字体尺寸，帮助复原
    //总概率
    public double NotEliminate_AllOdds = 0;
    public double Eliminate_AllOdds = 0;
    //非淘汰集合，两个不同类型的数据，但是他们的内容是一样的，data表示可以被序列化
    public List<NotEliminateItem> ListNotEliminateItem = new List<NotEliminateItem>();
    public List<NotEliminateItemData> ListNotEliminateItemData = new List<NotEliminateItemData>();
    public NotEliminateItem CurrentCheckedNotEliminateItem;
    //淘汰集合
    public List<EliminateItem> ListEliminateItem = new List<EliminateItem>();
    public List<EliminateItemData> ListEliminateItemData = new List<EliminateItemData>();
    public EliminateItem CurrentCheckedEliminateItem;
    public bool RaffleIsRuning = false;//flase表示没有在运行的

    //读档存档的脚本，不序列化，因为CreateOrLoadData脚本会提前赋值
    public CreateOrLoadData createOrLoadData;//需要存档操作


    private GlobalTransferStation()
    {

    }

    public static GlobalTransferStation Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GlobalTransferStation ();
            }
            return instance;
        }
    }
}
