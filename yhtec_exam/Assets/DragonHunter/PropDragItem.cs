using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//用來從清單拖曳物件出來
public class PropDragItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Tooltip("對應3D模型prefab")]
    public GameObject prefab;
    [Tooltip("游標沒有指到曲面時，道具離攝影機多遠的地方")]
    public float floatDistance = 15f;

    GameObject ghost;
    Camera cam;
    Collider ground;
    bool onGround;
    Vector3 groundPoint;

    public void OnBeginDrag(PointerEventData e)
    {
      if(prefab==null){Debug.LogWarning("沒有指定prefab");}
      cam = Camera.main;
        if (cam == null)
        {
            cam = FindAnyObjectByType<Camera>();
            if(cam==null)return;
        }

        var curved = FindAnyObjectByType<CurvedGround>();
        ground = curved!=null?curved.GetComponent<Collider>():null;
        ghost = Instantiate(prefab);
        ghost.name = prefab.name;
        SetColliders(ghost,false);
        Follow(e);
    }
    public void OnDrag(PointerEventData e)
    {
        if(ghost!=null)Follow(e);
    }
    public void OnEndDrag(PointerEventData e)
    {
        
        if(ghost == null) return;
        Follow(e);

        if(isOverUI(e) || !onGround)
        {
            Destroy(ghost);
        }
        else
        {
            ghost.transform.position = groundPoint;
            SetColliders(ghost,true);
            var placed = ghost.AddComponent<PlacedProp>();
            var label = GetComponentInChildren<Text>();
            placed.displayName = label !=null?label.text:prefab.name;
            ghost = null;
        }
    }


    void Follow(PointerEventData e)
    {
        Ray ray = cam.ScreenPointToRay(e.position);
        RaycastHit hit = default;
        onGround = ground != null && ground.Raycast(ray, out hit, 1000f);
        if (onGround)
        {
            groundPoint = hit.point;
            ghost.transform.position = hit.point;
        }
        else
        {
            ghost.transform.position = ray.GetPoint(floatDistance);
        }
    }
    static bool isOverUI(PointerEventData e)
    {
        GameObject under = e.pointerCurrentRaycast.gameObject;
        return under != null && under.transform is RectTransform;
    }
    static void SetColliders(GameObject go,bool enabled)
    {
        foreach(var c in go.GetComponentsInChildren<Collider>())
        c.enabled = enabled;
    }
}
