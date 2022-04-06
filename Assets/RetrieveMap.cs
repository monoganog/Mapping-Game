using System.IO;
using System.Net;
using UnityEngine;
public class RetrieveMap : MonoBehaviour
{
    public Material mapMaterial;
    // Start is called before the first frame update
    void Start()
    {
        RetrieveElevation(14, 9, 4);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RetrieveAMap(int x, int y, int zoom)
    {
        string url = "https://tile.openstreetmap.org/" + zoom + "/" + x + "/" + y + ".png";
        WebRequest www = WebRequest.Create(url);
        ((HttpWebRequest)www).UserAgent = "jeff";
        var response = www.GetResponse();
        Debug.Log("got a rsponse");
        Texture2D texture = new Texture2D(2, 2);
        ImageConversion.LoadImage(texture, new BinaryReader(response.GetResponseStream()).ReadBytes(100000));
        mapMaterial.mainTexture = texture;
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
