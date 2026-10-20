using UnityEditor;
using UnityEngine;

//build model prefab for prop
public static class PropPrefabBuilder
{
    const string Root = "Assets/DragonHunter";
    const  string prefabDir = Root + "/Prefabs";
    const string MatDir = Root + "/Materials";

    [MenuItem("DragonHunter/建立道具 Prefab")]
    public static void BuildAll()
    {
        EnsureFolder(Root,"Prefabs");
        EnsureFolder(Root,"Materials");

        var leaf = Mat("Leaf",new Color(0.2f,0.5f,0.2f));
        var leaf2 = Mat("Leaf2",new Color(0.3f,0.6f,0.22f));
        var bark = Mat("Bark",new Color(0.4f,0.26f,0.14f));
        var stone = Mat("Stone",new Color(0.55f,0.54f,0.52f));
        var metal = Mat("Metal",new Color(0.7f,0.7f,0.7f));
        var bait = Mat("Bait",new Color(0.35f,0.40f,0.45f));
        var soil = Mat("Soil",new Color(0.3f,0.3f,0.12f));
        var sprout = Mat("Sprout",new Color(0.2f,0.5f,0.2f));

        Save(BuildBush(leaf,leaf2),"Prop_Bush");
        Save(BuildTree(bark,leaf,leaf2),"Prop_Tree");
        Save(BuildRock(stone),"Prop_Rock");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
    }

    //build bush
    static GameObject BuildBush(Material a,Material b)
    {
        var root = new GameObject("Prop_Bush");
        Part(root,PrimitiveType.Sphere,new Vector3(0f,0.65f,0f),new Vector3(1.5f,1.3f,1.5f),Vector3.zero,a);
        Part(root,PrimitiveType.Sphere,new Vector3(0.7f,0.5f,0.2f),new Vector3(1.0f,0.9f,1.0f),Vector3.zero,b);
        Part(root,PrimitiveType.Sphere,new Vector3(-0.6f,0.45f,-0.3f),new Vector3(1.2f,1.0f,1.2f),Vector3.zero,b);
        Part(root,PrimitiveType.Sphere,new Vector3(0.1f,0.1f,-0.4f),new Vector3(1.2f,1.0f,1.2f),Vector3.zero,a);
        return Finish(root);

    }
    static GameObject BuildTree(Material bark,Material a,Material b)
    {
        var root = new GameObject("Prop_Tree");
        Part(root,PrimitiveType.Cylinder,new Vector3(0f,1.5f,0f),new Vector3(0.4f,1.5f,0.4f),Vector3.zero,bark);
        Part(root,PrimitiveType.Sphere,new Vector3(0f,3.5f,0f),new Vector3(2.5f,2.5f,2.5f),Vector3.zero,a);
        Part(root,PrimitiveType.Sphere,new Vector3(-1.2f,2.8f,-0.6f),new Vector3(1.8f,1.8f,1.8f),Vector3.zero,b);
        Part(root,PrimitiveType.Sphere,new Vector3(1.0f,2.6f,0.7f),new Vector3(1.5f,1.5f,1.5f),Vector3.zero,a);
        return Finish(root);
    }

    static GameObject BuildRock(Material mat)
    {
        var root = new GameObject("Prop_Rock");
        Part(root,PrimitiveType.Sphere,new Vector3(0f,0.5f,0f),new Vector3(1.5f,1.0f,1.5f),Vector3.zero,mat);
        Part(root,PrimitiveType.Sphere,new Vector3(-0.6f,0.4f,-0.4f),new Vector3(1.2f,0.8f,1.2f),Vector3.zero,mat);
        Part(root,PrimitiveType.Sphere,new Vector3(0.5f,0.3f,0.6f),new Vector3(1.0f,0.7f,1.0f),Vector3.zero,mat);
        return Finish(root);
    }
    static void Part(GameObject root,PrimitiveType type,Vector3 pos,Vector3 scale,Vector3 euler,Material mat)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = type.ToString();
        go.transform.SetParent(root.transform,false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.transform.localEulerAngles = euler;
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        Object.DestroyImmediate(go.GetComponent<Collider>());
    }
    //According to the bounds of the component
    static GameObject Finish(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>();
        var b = renderers[0].bounds;
        foreach(var r in renderers) b.Encapsulate(r.bounds);
        var box = root.AddComponent<BoxCollider>();
        box.center = b.center - root.transform.position;
        box.size = b.size;
        return root;
    }
    static void Save(GameObject root,string name)
    {
        PrefabUtility.SaveAsPrefabAsset(root,prefabDir+"/"+name+".prefab");
        Object.DestroyImmediate(root);
    }
    static Material Mat(string name,Color color)
    {
        string path = MatDir + "/PrpMat_"+name+".mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if(mat==null)
        {
         var shader = Shader.Find("Universal Render Pipeline/Lit");
         if(shader==null) shader = Shader.Find("Standard");
         mat = new Material(shader);
         AssetDatabase.CreateAsset(mat,path);
        }
        mat.SetColor("_BaseColor",color);
        mat.color = color;
        mat.SetFloat("_Smoothness",0.15f);
        EditorUtility.SetDirty(mat);
        return mat;
    }
    static void EnsureFolder(string parent,string name)
    {
        if(!AssetDatabase.IsValidFolder(parent+"/"+name))
        {
            AssetDatabase.CreateFolder(parent,name);
        }
    }
    
    
}
