using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class CurvedGround:MonoBehaviour
{
    
    [Header("範圍")]
    public float size=40f;
    public int resolution=100;

    [Header("丘陵")]
    public float hillHeight=5f;
    public float hillFrequency=0.1f;
    public float seed = 12.0f;

    [Header("河谷")]
    public float riverDepth=3f;
    public float riverWidth = 4f;
    public float riverAmplitude = 8f;
    public float riverFrequency = 0.1f;

    void Awake(){Generate();}

    [ContextMenu("Generate")]//生成地形
    public void Generate()
    {
        int n= resolution +1; // how many vertices per side
        float step = size/resolution; // distance between vertices
        float half = size/2f;

        // Create vertices
        var vertices = new Vector3[n*n];
        var uvs = new Vector2[n*n];
        for(int z = 0; z < n; z++)
        {
            for(int x = 0; x < n; x++)
            {
                int i= z*n+x;
                float px = -half + x*step;
                float pz = -half + z*step;
                vertices[i] = new Vector3(px,Height(px,pz),pz);
                uvs[i] = new Vector2((float)x/resolution,(float)z/resolution);   
            }
        }

        //setp2 create triangles
        var triangles = new int[resolution*resolution*6];
        int t=0;
        for(int z=0;z<resolution;z++)
        {
            for(int x=0;x<resolution;x++)
            {
              // make two triangles for each square
              int a = z*n+x;
              int b= a+1;
              int c= a+n;
              int d= c+1;

              //triangle 1
              triangles[t++] = a;//left bottom
              triangles[t++] = c;//left top
              triangles[t++] = b;//right bottom
              //triangle 2
              triangles[t++] = b;//right bottom
              triangles[t++] = c;//left top
              triangles[t++] = d;//right top
            }
        }

        //step3 unity mesh
        var mesh = new Mesh{name = "CurvedGround"};
        mesh.indexFormat = vertices.Length>65535?IndexFormat.UInt32:IndexFormat.UInt16;
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        var col = GetComponent<MeshCollider>();
        col.sharedMesh = null;//reset physic material
        col.sharedMesh = mesh;
    }
    //get height of the terrain at a given point
    float Height(float x, float z)
    {
        float hills = Mathf.PerlinNoise((x+seed)*hillFrequency,
        (z+seed)*hillFrequency)*hillHeight+Mathf.PerlinNoise((x+seed)*hillFrequency*3f,(z+seed)*hillFrequency
        *3f)*hillHeight*0.25f;

        //pivot point of the river
        float zc = Mathf.Sin(x*riverFrequency)*riverAmplitude;
        float d = z-zc;//distance from the river
        float valley = riverDepth*Mathf.Exp(-d*d/(riverWidth*riverWidth));

        return hills - valley;
    }
}
