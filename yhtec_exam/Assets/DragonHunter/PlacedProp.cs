using UnityEngine;

//drag item
public class PlacedProp:MonoBehaviour
{

    public string displayName = "道具";
    public Bounds GetBounds()
    {
        var col = GetComponent<Collider>();
        if(col!=null) return col.bounds;

        var renderers = GetComponentsInChildren<Renderer>();
        if(renderers.Length == 0) return new Bounds(transform.position,Vector3.one);
        var b=renderers[0].bounds;
        foreach(var r in renderers) b.Encapsulate(r.bounds);
        return b;
    }
    
}
