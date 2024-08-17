using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINotEliminateItemInformation:MonoBehaviour
{
    public GameObject NotEliminatePanel;//调用它脚本的方法SetAllOddsAndFinalOdds()，重新计算总概率
    public GameObject NotEliminateItemInformationPanel;//修改完立刻关闭
    //这两个物体的输入信息要清空，一旦创建成功的话
    public GameObject InputField_Content;
    public GameObject InputField_Odds;
    //提示
    public GameObject TipPanel;
    //销毁物体的父级，我们需要设置禁用状态
    GameObject FatherGameObject;

    string Content;
    string Odds;
    GlobalTransferStation transferStation;

    void OnEnable()
    {
        transferStation = GlobalTransferStation.Instance;
        //找到脚本，然后转gameObject就是物体，根据当前地点找父级
        FatherGameObject = transferStation.CurrentCheckedNotEliminateItem.gameObject.transform.parent.gameObject;
        //Debug.Log("FatherGameObject.name:" + FatherGameObject.name);
        InputField_Content.GetComponent<InputField>().text = transferStation.CurrentCheckedNotEliminateItem.Content;
        InputField_Odds.GetComponent<InputField>().text = transferStation.CurrentCheckedNotEliminateItem.Odds.ToString();
    }
    void OnDisable()
    {
        InputField_Content.GetComponent<InputField>().text = "";
        InputField_Odds.GetComponent<InputField>().text = "";
    }
    public void UpdateNotEliminateItem()
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
        transferStation.CurrentCheckedNotEliminateItem.Content = Content;
        transferStation.CurrentCheckedNotEliminateItem.Odds = double.Parse(Odds);
        //不用重新交付，因为全是引用，实际修改的是同一个对象
        //重新概率划分
        NotEliminatePanel.GetComponent<NotEliminateManagement>().SetAllOddsAndFinalOdds();
        //修改伴随存档
        transferStation.CurrentCheckedNotEliminateItem.itemData.SetNotEliminateItemData(transferStation.CurrentCheckedNotEliminateItem);
        //创建新选项的时候，顺便调用存档，就是做一个选项立刻存档
        transferStation.createOrLoadData.SaveData();

        //修改完，调用赋值方法
        transferStation.CurrentCheckedNotEliminateItem.SetContent();

        NotEliminateItemInformationPanel.SetActive(false);
    }
    public void DeleleNotEliminateItem()
    {
        //先删除中转站集合里面的ItemData数据,
        transferStation.ListNotEliminateItemData.Remove(transferStation.CurrentCheckedNotEliminateItem.itemData);
        //然后再删除Item集合里面的对应脚本
        transferStation.ListNotEliminateItem.Remove(transferStation.CurrentCheckedNotEliminateItem);
        //然后再销毁脚本附着的物体，预制件已经拖拽赋值了
        Destroy(transferStation.CurrentCheckedNotEliminateItem.NotEliminateItemPrefab);
        //然后再将父级物体禁用
        FatherGameObject.SetActive(false);
        //然后重新分配最终概率
        NotEliminatePanel.GetComponent<NotEliminateManagement>().SetAllOddsAndFinalOdds();
        //然后保存
        transferStation.createOrLoadData.SaveData();

        //删除成功后，我们关闭当前页面
        NotEliminateItemInformationPanel.SetActive(false);
    }
}
