using System.Collections.Generic;
using UnityEngine;

public class LocationsDictionary : MonoBehaviour
{

    public Dictionary<string, string> locationsDictionary;

    // Start is called before the first frame update
    void Start()
    {
        locationsDictionary.Add("x,y,z", "London");
        CheckDictionary("x,y,z");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CheckDictionary(string key)
    {
        if (locationsDictionary.ContainsKey(key))
        {
            Debug.Log(locationsDictionary[key]);
        }
    }
}
