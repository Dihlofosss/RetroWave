using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class SoundStream : MonoBehaviour
{
    public string artwork_url;
    public int duration;
    public int id;
    public string label_name;
    public string title;
    public string uri;
    public string stream_url;
    
    public bool streamable;
    

    private string _client_id = "?client_id=qmXWU1yUsrcP4HnYUegkRIvVYARqxR16";

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetRequest("https://api-v2.soundcloud.com/tracks/1233088231"));
        //StartCoroutine(GetRequest("https://api-v2.soundcloud.com/tracks/1233088231/related"));
    }

    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(uri + _client_id);
        yield return webRequest.SendWebRequest();
        
        if(webRequest.isNetworkError)
        {
            Debug.Log("Error: " + webRequest.error);
        }
        else
        {
            Debug.Log("Recieved: " + webRequest.result + "\n" + "Message: " + webRequest.downloadHandler.text);
        }

        JsonUtility.FromJsonOverwrite(webRequest.downloadHandler.text, this);

        Debug.Log("ArtworkURL: " + artwork_url);
        Debug.Log("Duration: " + duration);
        Debug.Log("Track ID: " + id);
        Debug.Log("Label name: " + label_name);
        Debug.Log("Title: " + title);
        Debug.Log("Stream URL: " + stream_url);
    }
}
