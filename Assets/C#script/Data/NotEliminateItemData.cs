using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//该脚本伴随于NotEliminateItem内部作为变量，在创建的时候，顺便放入全局数据管理中心的集合里面，方便存档
[System.Serializable]
public class NotEliminateItemData
{
    public string FatherName;//我们记住父级物体的名字，可以保证读档位置不乱
    public string Content;
    //自定义概率：
    public double Odds;//概率只能通过修改面板显示，正常只显示内容
    //比重
    public double FinalOdds;

    public void SetNotEliminateItemData(NotEliminateItem item)
    {
        FatherName = item.NotEliminateItemPrefab.transform.parent.name;
        Content = item.Content;
        Odds = item.Odds;
        FinalOdds = item.FinalOdds;
    }
    public void SetNotEliminateItem(NotEliminateItem item)
    {
        item.itemData = this;
        item.Content = Content;
        item.Odds = Odds;
        item.FinalOdds = FinalOdds;
    }
}
