using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleChange:MonoBehaviour
{
    public GameObject EliminatePanel;
    public GameObject NotEliminatePanel;

    public void SetEliminatePanelActive()
    {

        if (GetComponent<Toggle>().isOn)//由于两个toggle引用的方法不同，并且两个物体附带的脚本独立，所获取的ison独立，不冲突
        {
            EliminatePanel.SetActive(true);
            NotEliminatePanel.SetActive(false);
        }

    }
    public void SetNotEliminatePanelActive()
    {

        if (GetComponent<Toggle>().isOn)
        {
            NotEliminatePanel.SetActive(true);
            EliminatePanel.SetActive(false);
        }
    }    
}
