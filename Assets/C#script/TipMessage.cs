using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//该类与其面板，是独立的，共用一个面板，只需要修改里面文本即可，节省资源，
public class TipMessage:MonoBehaviour
{

    public Text TipMessageText;
    //
    GlobalTransferStation transferStation;
    private void OnEnable()
    {
        transferStation = GlobalTransferStation.Instance;
        TipMessageText.text = transferStation.TipMessageText;

    }

    private void OnDisable()
    {
        transferStation.TipMessageText = "如果显示此句，意味着出错，中转单例类变量未被赋值！";
        TipMessageText.fontSize = transferStation.InititalFontSize;
    }
}
