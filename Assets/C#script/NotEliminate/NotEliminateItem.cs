using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//通过OnEnable()和OnDisable(),来展示数据和删除，删除直接禁用，触发OnDisable()，这时候直接清空数据就可以了
public class NotEliminateItem:MonoBehaviour
{
    public NotEliminateItemData itemData;//保留一个引用，方便同时修改
    public GameObject NotEliminateItemPrefab;//执行子级，方便删除操作
    public string Content;
    public Text ContentText;
    //范围,本来设定大小变量是表示区间的，但是发现抽奖的时候，累加就可以表示区间了，思路是一样的
    //自定义概率：
    public double Odds;//概率只能通过修改面板显示，正常只显示内容
    //比重
    public double FinalOdds;
    GlobalTransferStation transferStation;

    private void OnEnable()
    {
        transferStation = GlobalTransferStation.Instance;
        SetContent();
    }
    private void OnDisable()
    {
        ContentText.text = "";
    }
    public void SetContent()
    {
        ContentText.text = Content;
    }

    //点击事件
    public void Touch()
    {
        transferStation.CurrentCheckedNotEliminateItem = GetComponent<NotEliminateItem>();
        transferStation.NotEliminateItemInformationPanel.SetActive(true);
    }
}
