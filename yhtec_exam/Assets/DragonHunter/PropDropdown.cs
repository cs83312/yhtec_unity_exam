using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PropDropdown:MonoBehaviour
{
    
    [Tooltip("道具清單的容器,收和隱藏，展開顯示")]
    public GameObject listRoot;
    [Tooltip("標題上的按鈕文字")]
    public Text headerLabel;
    public string title = "道具";
    public bool startOpen = false;
    bool isOpen = false;    
    void Start()
    {
        SetOpen(startOpen);
    }

    public void Toggle()
    {
        SetOpen(!isOpen);
        if(EventSystem.current!=null) EventSystem.current.SetSelectedGameObject(null);
    }
    public void SetOpen(bool open)
    {
        isOpen = open;
        if(listRoot!=null) listRoot.SetActive(isOpen);
        if(headerLabel!=null) headerLabel.text = isOpen? "收起":"展開";
    }
}
