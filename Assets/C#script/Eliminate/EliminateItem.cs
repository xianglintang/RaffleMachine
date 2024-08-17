using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//通过OnEnable()和OnDisable(),来展示数据和删除，删除直接禁用，触发OnDisable()，这时候直接清空数据就可以了
//该脚本自带图片透明度改变，方便管理类协程调用，不用在那边额外写
public class EliminateItem:MonoBehaviour
{
    public EliminateItemData itemData;//保留一个引用，方便同时修改
    public GameObject EliminateItemPrefab;//执行子级，方便删除操作
    public Image CheckingImage;
    public Image CheckedImage;
    public Text ContentText;
    //
    public string Content;
    //范围,本来设定大小变量是表示区间的，但是发现抽奖的时候，累加就可以表示区间了，思路是一样的
    //自定义概率：
    public double Odds;//概率只能通过修改面板显示，正常只显示内容
    //比重
    public double FinalOdds;
    GlobalTransferStation transferStation;
    Color color;
    private void OnEnable()
    {
        transferStation = GlobalTransferStation.Instance;
        //当被创建就会触发这个方法，此时俩图片要将透明度改为0

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
        if (!transferStation.RaffleIsRuning)
        {
            transferStation.CurrentCheckedEliminateItem = GetComponent<EliminateItem>();
            transferStation.EliminateItemInformationPanel.SetActive(true);
        }

    }
    public void Checking()
    {

        color = CheckingImage.color;
        color.a = 1;
        CheckingImage.color = color;
    }
    public void OverChecking()//正常状态,俩图片全部透明度为0
    {
        itemData.IsChecked = false;//标记没选过了
        color = CheckingImage.color;
        color.a = 0;
        CheckingImage.color = color;
        color = CheckedImage.color;
        color.a = 0;
        CheckedImage.color = color;
    }
    public void Checked()
    {
        itemData.IsChecked = true;//标记已经选择过了，我们到时候读档会对这些进行操作
        color = CheckedImage.color;
        color.a = 1;
        CheckedImage.color = color;
    }
}
