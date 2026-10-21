using System;
using System.Xml.Serialization;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PropSelectionMenu:MonoBehaviour
{
    [Tooltip("防止微小滑鼠移動造成點擊跟拖曳混淆")]
    public float clickTolerance = 8f;
    [Tooltip("選單在道具右邊顯示")]
    public Vector2 offset = new Vector2(24f,0f);

    Camera cam;
    Canvas canvas;
    RectTransform menu;
    Text title;
    
    PlacedProp selected;
    Vector2 pressPos;
    bool pressOnUI;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        //TODO build menu
        BuildMenu();
        //Default hide
        Hide();

    }

    //update
    void Update()
    {
        var mouse = Mouse.current;
        if(mouse == null) return;
        if(cam == null) cam = Camera.main !=null ?
        Camera.main:FindAnyObjectByType<Camera>();

        Vector2 pos = mouse.position.ReadValue();
        if (mouse.leftButton.wasPressedThisFrame)
        {
            pressPos = pos;
            pressOnUI = EventSystem.current !=null && EventSystem.current.IsPointerOverGameObject();
        }

        if(mouse.leftButton.wasReleasedThisFrame && !pressOnUI
        && (pos-pressPos).sqrMagnitude<= clickTolerance * clickTolerance)
        {
            HandleClick(pos);
        }

        if(Keyboard.current !=null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Hide();
        }

    }
    //late upate
    void LateUpdate()
    {
        if(selected!=null)PlaceMenu();
        else if(menu.gameObject.activeSelf) Hide();
    }

    //handle event
    void HandleClick(Vector2 screenPos)
    {
        if(cam == null) return;

        Ray ray = cam.ScreenPointToRay(screenPos);
        PlacedProp prop = null;
        if(Physics.Raycast(ray,out RaycastHit hit, 1000f))
        {
            prop = hit.collider.GetComponentInParent<PlacedProp>();
        }
        if(prop != null)
        {
            Select(prop);
        }
        else
        {
            Hide();
        }
    }
    void Select(PlacedProp prop)
    {
        selected = prop;
        title.text = prop.displayName;
        menu.gameObject.SetActive(true);
        menu.SetAsLastSibling();
        PlaceMenu();
    }

    //we need to put menu
    void PlaceMenu()
    {
        Bounds b = selected.GetBounds();
        Vector3 world = b.center + cam.transform.right * b.extents.magnitude;
        Vector3 sp = cam.WorldToScreenPoint(world);

        if(sp.z < 0f)
        {
            menu.gameObject.SetActive(false);return;
        }

        if (!menu.gameObject.activeSelf)
        {
            menu.gameObject.SetActive(false);
        }

        float s = canvas.scaleFactor;
        Vector2 p = (Vector2)sp + offset*s;
        Vector2 size = menu.rect.size * s;

        //dont out of screen range
        p.x = Mathf.Clamp(p.x,0f,Screen.width - size.x);
        p.y = Mathf.Clamp(p.y, size.y*0.5f,Screen.height - size.y * 0.5f);

        menu.position = p;

    }
    void BuildMenu()
    {
        var go = new GameObject("PropMenu",typeof(RectTransform),typeof(Image),typeof(VerticalLayoutGroup)
        ,typeof(ContentSizeFitter));

        menu = (RectTransform)go.transform;
        menu.SetParent(canvas.transform,false);
        menu.anchorMin = menu.anchorMax = new Vector2(0.5f,0.5f);
        menu.pivot = new Vector2(0f,0.5f);//left center
        menu.sizeDelta = new Vector2(220f,0f);

        go.GetComponent<Image>().color = new Color(0.08f,0.08f,0.08f,0.92f);

        var layout = go.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(12,12,12,12);
        layout.spacing = 8f;
        layout.childControlWidth=true;
        layout.childControlHeight=true;
        layout.childForceExpandWidth=true;
        layout.childForceExpandHeight=true;

        go.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        title = MakeText(menu,"道具",26,Color.white);//todo make text

        // make delete button
        MakeButton(menu,"刪除",new Color(0.75f,0.20f,0.20f),DeleteSelected);//todo delete action
        // make close button
        MakeButton(menu,"關閉",new Color(0.35f,0.35f,0.35f),Hide);
    }


    public void DeleteSelected()
    {
        if(selected!=null)Destroy(selected.gameObject);
        Hide();
    }


    public void Hide()
    {
        selected = null;
        if(menu != null)menu.gameObject.SetActive(false);
        if(EventSystem.current !=null)
        EventSystem.current.SetSelectedGameObject(null);
    }
    static Text MakeText(Transform parent,string content,int size,Color color)
    {
        var go = new GameObject("Text",typeof(RectTransform),typeof(Text));
        go.transform.SetParent(parent,false);
        var t = go.GetComponent<Text>();
        t.text = content;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size;
        t.color = color;
        t.alignment = TextAnchor.MiddleCenter;
        t.raycastTarget=false;
        return t;
    }
    static void MakeButton(Transform parent,string label,Color color,UnityAction onClick)
    {
        var go = new GameObject("Button_"+label,typeof(RectTransform),typeof(Image),typeof(Button));
        go.transform.SetParent(parent,false);
        go.GetComponent<Image>().color = color;
        go.GetComponent<Button>().onClick.AddListener(onClick);
        go.AddComponent<LayoutElement>().preferredHeight = 48f;

        var text = MakeText(go.transform,label,24,Color.white);
        var rt = (RectTransform)text.transform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;


    }



}
