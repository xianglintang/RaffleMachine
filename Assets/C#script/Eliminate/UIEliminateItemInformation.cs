using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEliminateItemInformation:MonoBehaviour
{
    public GameObject EliminatePanel;//调用它脚本的方法SetAllOddsAndFinalOdds()，重新计算总概率
    public GameObject EliminateItemInformationPanel;//修改完立刻关闭
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
        FatherGameObject = transferStation.CurrentCheckedEliminateItem.gameObject.transform.parent.gameObject;
        //Debug.Log("FatherGameObject.name:" + FatherGameObject.name);
        InputField_Content.GetComponent<InputField>().text = transferStation.CurrentCheckedEliminateItem.Content;
        InputField_Odds.GetComponent<InputField>().text = transferStation.CurrentCheckedEliminateItem.Odds.ToString();
    }
    void OnDisable()
    {
        InputField_Content.GetComponent<InputField>().text = "";
        InputField_Odds.GetComponent<InputField>().text = "";
    }
    public void UpdateEliminateItem()
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
        transferStation.CurrentCheckedEliminateItem.Content = Content;
        transferStation.CurrentCheckedEliminateItem.Odds = double.Parse(Odds);
        //不用重新交付，因为全是引用，实际修改的是同一个对象
        //先取消已选状态
        transferStation.CurrentCheckedEliminateItem.OverChecking();
        //重新概率划分
        EliminatePanel.GetComponent<EliminateManagement>().SetAllOddsAndFinalOdds();
        //修改伴随存档
        transferStation.CurrentCheckedEliminateItem.itemData.SetEliminateItemData(transferStation.CurrentCheckedEliminateItem);
        //创建新选项的时候，顺便调用存档，就是做一个选项立刻存档
        transferStation.createOrLoadData.SaveData();

        //修改完，调用赋值方法
        transferStation.CurrentCheckedEliminateItem.SetContent();
        

        EliminateItemInformationPanel.SetActive(false);
    }
    public void DeleleEliminateItem()
    {
        //先删除中转站集合里面的ItemData数据,
        transferStation.ListEliminateItemData.Remove(transferStation.CurrentCheckedEliminateItem.itemData);
        //然后再删除Item集合里面的对应脚本
        transferStation.ListEliminateItem.Remove(transferStation.CurrentCheckedEliminateItem);
        //然后再销毁脚本附着的物体，预制件已经拖拽赋值了
        Destroy(transferStation.CurrentCheckedEliminateItem.EliminateItemPrefab);
        //然后再将父级物体禁用
        FatherGameObject.SetActive(false);
        //然后重新分配最终概率
        EliminatePanel.GetComponent<EliminateManagement>().SetAllOddsAndFinalOdds();
        //然后保存
        transferStation.createOrLoadData.SaveData();

        //删除成功后，我们关闭当前页面
        EliminateItemInformationPanel.SetActive(false);
    }
}
