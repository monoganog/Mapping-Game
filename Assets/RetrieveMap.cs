using System.IO;
using System.Net;
using UnityEngine;
public class RetrieveMap : MonoBehaviour
{
    public Material mapMaterial;

    public MeshFilter mesh;

    public Texture2D tex;
    // Start is called before the first frame update
    void Start()
    {
        RetrieveElevation(14, 9, 4);

        tex = (Texture2D)mapMaterial.mainTexture;

        CreateMesh(tex, 64, 64, 0.001f);
        RetrieveAMap(14, 9, 4);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RetrieveAMap(int x, int y, int zoom)
    {
        // https://tiles.wmflabs.org/hillshading/${z}/${x}/${y}.png
        //https://stamen-tiles.a.ssl.fastly.net/toner/{z}/{x}/{y}.png
        string url = "https://tiles.wmflabs.org/hillshading/$" + zoom + "/$" + x + "/$" + y + ".png";
        WebRequest www = WebRequest.Create(url);
        ((HttpWebRequest)www).UserAgent = "jeff";
        var response = www.GetResponse();
        Debug.Log("got a rsponse");
        Texture2D texture = new Texture2D(2, 2);
        ImageConversion.LoadImage(texture, new BinaryReader(response.GetResponseStream()).ReadBytes(100000));
        mapMaterial.mainTexture = texture;
    }

    public void CreateMesh(Texture2D tex, int xgrid, int zgrid, float yscale)
    {
        Vector3[] vertices = new Vector3[xgrid * zgrid];
        Vector2[] UVs = new Vector2[xgrid * zgrid];

        int[] triangles = new int[(xgrid - 1) * (zgrid - 1) * 2 * 3];

        // Set up vertices
        // vertices
        for (int x = 0; x < xgrid; x++)
        {
            for (int z = 0; z < zgrid; z++)
            {
                float height = 0.0f;
                Color a = tex.GetPixel((int)(tex.width * ((float)(x - 1)) / xgrid), (int)(tex.width * ((float)(z - 1)) / zgrid));


                height = (a.r * 255.0f * 256) +
                    (a.g * 255f) +
                    (a.b * 255.0f / 256) - 32768.0f;



                vertices[x + (z * xgrid)] = new Vector3(x, yscale * height, z);
                UVs[x + (z * xgrid)] = new Vector2((float)(x - 1) / xgrid, (float)(z - 1) / zgrid);
            }
        }
        // triangles
        for (int x = 0; x < xgrid - 1; x++)
        {
            for (int z = 0; z < zgrid - 1; z++)
            {
                triangles[(x + z * (xgrid - 1)) * 6 + 0] = (x + 1) + (z * xgrid);
                triangles[(x + z * (xgrid - 1)) * 6 + 1] = x + (z * xgrid);
                triangles[(x + z * (xgrid - 1)) * 6 + 2] = x + ((z + 1) * xgrid);



                triangles[(x + z * (xgrid - 1)) * 6 + 3] = (x + 1) + ((z + 1) * xgrid);
                triangles[(x + z * (xgrid - 1)) * 6 + 4] = (x + 1) + (z * xgrid);
                triangles[(x + z * (xgrid - 1)) * 6 + 5] = x + ((z + 1) * xgrid);
            }
        }



        mesh.mesh.Clear();
        mesh.mesh.vertices = vertices;
        mesh.mesh.uv = UVs;

        mesh.mesh.triangles = triangles;
        mesh.mesh.RecalculateNormals();


    }

    public void RetrieveElevation(int x, int y, int zoom)
    {
        string url = "https://s3.amazonaws.com/elevation-tiles-prod/terrarium/" + zoom + "/" + x + "/" + y + ".png";
        WebRequest www = WebRequest.Create(url);
        ((HttpWebRequest)www).UserAgent = "jeff";
        var response = www.GetResponse();
        Debug.Log("got a rsponse");
        Texture2D texture = new Texture2D(2, 2);
        ImageConversion.LoadImage(texture, new BinaryReader(response.GetResponseStream()).ReadBytes(100000));
        mapMaterial.mainTexture = texture;
    }
}
