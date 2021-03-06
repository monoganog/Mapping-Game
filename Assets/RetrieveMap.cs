using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RetrieveMap : MonoBehaviour
{
    public Material mapMaterial;
    public Material mapMiddleMaterial;
    public Material mapTopMaterial;
    public Material mapTopLeftMaterial;
    public Material mapTopRightMaterial;
    public Material mapBottomMaterial;
    public Material mapBottomLeftMaterial;
    public Material mapBottomRightMaterial;
    public Material mapLeftMaterial;
    public Material mapRightMaterial;
    public AudioSource AS;
    public AudioSource ASclick;
    public MeshFilter mesh;

    public Texture2D tex;
    public Vector2 currentLocationVector;

    private int zoom, y, z;

    public LocationsObject[] locations;

    LocationsObject currentLocation;
    private int lastIndex;

    public float increment = 10;

    public Vector2 winningLocation = new Vector2(59158.0f, 40214.0f);

    public int moves = -1;

    public GameObject winPanel;

    public float time;
    private bool shouldTime;

    public Text timeText;
    public Text movesText;

    public Text finalText;

    // Start is called before the first frame update
    void Start()
    {
        //winPanel.SetActive(true);
        NextLocation();

        tex = (Texture2D)mapMaterial.mainTexture;


    }
    public void StartGame()
    {
        shouldTime = true;
    }

    public void UpdateMapUsingSlippy()
    {


        RetrieveAMap(mapMaterial, currentLocationVector.x, currentLocationVector.y, currentLocation.Zoom);
        RetrieveAMap(mapMiddleMaterial, currentLocationVector.x, currentLocationVector.y, currentLocation.Zoom);
        RetrieveAMap(mapTopMaterial, currentLocationVector.x, currentLocationVector.y - 1, currentLocation.Zoom);
        RetrieveAMap(mapBottomMaterial, currentLocationVector.x, currentLocationVector.y + 1, currentLocation.Zoom);
        RetrieveAMap(mapLeftMaterial, currentLocationVector.x - 1, currentLocationVector.y, currentLocation.Zoom);
        RetrieveAMap(mapRightMaterial, currentLocationVector.x + 1, currentLocationVector.y, currentLocation.Zoom);


        RetrieveAMap(mapTopLeftMaterial, currentLocationVector.x - 1, currentLocationVector.y - 1, currentLocation.Zoom);
        RetrieveAMap(mapTopRightMaterial, currentLocationVector.x + 1, currentLocationVector.y - 1, currentLocation.Zoom);

        RetrieveAMap(mapBottomLeftMaterial, currentLocationVector.x - 1, currentLocationVector.y + 1, currentLocation.Zoom);
        RetrieveAMap(mapBottomRightMaterial, currentLocationVector.x + 1, currentLocationVector.y + 1, currentLocation.Zoom);

        moves = moves + 1;
        movesText.text = moves.ToString();
        CheckWinCondition();

    }

    private void CheckWinCondition()
    {
        if (currentLocationVector == winningLocation)
        {
            Debug.Log("you win");
            AS.Play();
            winPanel.SetActive(true);
            finalText.text = "You made it to Flinders in " + moves.ToString() + " moves and " + time.ToString("F1") + " seconds. Well done!";
        }
    }

    public Vector2 WorldToTilePos(double lon, double lat, int zoom)
    {
        Vector2 p = new Vector2();
        p.x = Mathf.Round((float)((lon + 180.0) / 360.0 * (1 << zoom)));
        p.y = Mathf.Round((float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
            1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom)));
        return p;
    }

    public void Retry()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NextLocation()
    {
        int index = 0;


        index = UnityEngine.Random.Range(0, locations.Length);

        currentLocation = locations[index];

        while (!currentLocation.included)
        {
            index = UnityEngine.Random.Range(0, locations.Length);
            currentLocation = locations[index];

        }
        Debug.Log(currentLocation.Name + currentLocation.x + currentLocation.y);
        lastIndex = index;
        currentLocationVector = WorldToTilePos(currentLocation.x, currentLocation.y, currentLocation.Zoom);
        Debug.Log(currentLocationVector);
        UpdateMapUsingSlippy();
    }


    public void Update()
    {
        if (shouldTime)
        {
            time += Time.deltaTime;
            timeText.text = time.ToString("F1");

        }
    }


    public void MoveUp()
    {
        currentLocationVector.y--;
        Debug.Log(currentLocation.x + "," + currentLocation.y);
        UpdateMapUsingSlippy();
    }

    public void MoveDown()
    {
        currentLocationVector.y++;
        Debug.Log(currentLocation.x + "," + currentLocation.y);
        UpdateMapUsingSlippy();
    }

    public void MoveRight()
    {
        currentLocationVector.x++;
        Debug.Log(currentLocation.x + "," + currentLocation.y);
        UpdateMapUsingSlippy();
    }

    public void MoveLeft()
    {
        currentLocationVector.x--;
        Debug.Log(currentLocation.x + "," + currentLocation.y);
        UpdateMapUsingSlippy();
    }

    public void RetrieveAMap(Material mat, float x, float y, int zoom)
    {


        //https://a.tile.openstreetmap.fr/hot/${z}/${x}/${y}.png 
        //string url = "https://a.tile.openstreetmap.fr/hot/${" + zoom + "}/${" + x + "}/${" + y + "}.png";

        string url = "https://tile.openstreetmap.org/" + zoom + "/" + x + "/" + y + ".png";
        //Debug.Log(url);
        WebRequest www = WebRequest.Create(url);
        ((HttpWebRequest)www).UserAgent = "University Assignment";
        var response = www.GetResponse();
        //Debug.Log("got a response");
        Texture2D texture = new Texture2D(2, 2);
        ImageConversion.LoadImage(texture, new BinaryReader(response.GetResponseStream()).ReadBytes(100000));
        mat.mainTexture = texture;
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

    public void RetrieveElevation(float x, float y, int zoom)
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
