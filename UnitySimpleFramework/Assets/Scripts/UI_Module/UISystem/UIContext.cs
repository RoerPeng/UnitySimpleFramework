using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UI界面 上下文 ，保存场景中现存的UI界面
/// </summary>
public class UIContext : MonoBehaviour
{
    public static UIContext Instance = null;

    private readonly Dictionary<EUIPanel, GameObject> curUIPanels = new Dictionary<EUIPanel, GameObject>();
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameObject ShowPanel(EUIPanel eUIPanel)
    {
        if (curUIPanels.ContainsKey(eUIPanel))
        {
            GameObject panel = curUIPanels[eUIPanel];

            if (panel != null && !panel.activeSelf)
            {
                panel.SetActive(true);
            }

            return curUIPanels[eUIPanel];
        }

        string panelName = string.Format("UIPanel/{0}", eUIPanel.ToString()); //枚举名与Resources下文件名 一致

        GameObject uiPanel = GameObject.Instantiate(Resources.Load<GameObject>(panelName));

        Canvas canvas = uiPanel.GetComponent<Canvas>();
        if (canvas == null)
        {
            uiPanel.transform.SetParent(transform, false);
        }

        curUIPanels.Add(eUIPanel, uiPanel);

        uiPanel.gameObject.SetActive(true);

        return uiPanel;

    }

    public void ClosePanel(EUIPanel eUIPanel)
    {
        if (!curUIPanels.ContainsKey(eUIPanel))
        {
#if UNITY_EDITOR
            Debug.LogWarning(" 该UI界面 不存在 ! No " + eUIPanel);
#endif
            return;
        }

        //TODO  可以考虑 延迟5秒销毁
        GameObject.Destroy(curUIPanels[eUIPanel]);
        curUIPanels.Remove(eUIPanel);
    }

    public GameObject GetPanel(EUIPanel eUIPanel)
    {
        if (!curUIPanels.ContainsKey(eUIPanel))
        {
#if UNITY_EDITOR
            Debug.LogWarning(" GetPanel Error ! No " + eUIPanel);
#endif
            return null;
        }

        return  curUIPanels[eUIPanel];
        
    }

    public bool DisablePanel(EUIPanel eUIPanel)
    {
        if (!curUIPanels.ContainsKey(eUIPanel))
        {
#if UNITY_EDITOR
            Debug.LogWarning(" GetPanel Error ! No " + eUIPanel);
#endif
            return false;
        }

        curUIPanels[eUIPanel].SetActive(false);
        return true;

    }

    public GameObject InstantiatePartItem(string partName, Transform parent = null)
    {
        string panelName = string.Format("UIPart/{0}", partName);
        GameObject uiPart = GameObject.Instantiate(Resources.Load<GameObject>(panelName));

        if (parent != null)
        {
            uiPart.transform.SetParent(parent, false);
            uiPart.transform.localScale = Vector3.one;
            uiPart.transform.localPosition = Vector3.zero;
        }

        return uiPart;
    }
    
    /// <summary>
    /// 宽度不变的界面 ， 高度相比原有设计高度的差值
    /// </summary>
    public float GetDifferenceHeight()
    {
        float realWidth = Screen.width;
        float realHeight = Screen.height;

        float designWidth = 750;    // 设计宽度
        float designHeight = 1334;

        //保持宽度不变 , 形成的高度差值
        float diffH = ( realHeight * designWidth / realWidth ) - designHeight;

        return  diffH;
    }

}

