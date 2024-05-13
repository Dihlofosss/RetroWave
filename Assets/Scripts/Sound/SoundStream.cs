using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;

public class SoundStream : MonoBehaviour
{
    private readonly string _soundCloudAPI_url = "https://api-v2.soundcloud.com/";
    public string artwork_url;
    public int duration;
    public int id;
    public string label_name;
    public string title;
    public string uri;
    public string stream_url;
    
    public bool streamable;
    

    public string client_id;

    private AudioSource audioSource;
    private Audio audio;

    public TMPro.TextMeshProUGUI log;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(GetRequest("https://api-v2.soundcloud.com/tracks/"));
        audioSource = GetComponent<AudioSource>();
        audio = GetComponent<Audio>();
    }

    private void Start()
    {
        StartCoroutine(CreateTrack(1657565193));
    }

    IEnumerator CreateTrack(int trackID)
    {
        Debug.Log("Starting 1st coroutine");
        Dictionary<string, dynamic> data = null;
        yield return StartCoroutine(GetTrackData(trackID, value => data = value));
        Debug.Log(data);
    }

    IEnumerator GetTrackData(int trackID, System.Action<Dictionary<string, dynamic>> trackData)
    {
        Debug.Log("Starting 2nd coroutine");
        Dictionary<string, dynamic> data = null;
        yield return StartCoroutine(GetWebJSON(_soundCloudAPI_url + "tracks/" + trackID, value => data = value));
        trackData(data);
    }

    IEnumerator GetWebJSON(string uri, System.Action<Dictionary<string, dynamic>> json)
    {
        Debug.Log("Starting webRequest coroutine");
        UnityWebRequest webRequest = UnityWebRequest.Get(uri + "?client_id=" + client_id);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.result);
        //TODO:
        //Add normal error check
        if (webRequest.result != UnityWebRequest.Result.Success)
            json(null);
        else
            json(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(webRequest.downloadHandler.text));
    }

    IEnumerator GetRelatedTracks(int TrackID)
    {
        yield return null;
    }

    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(uri + id + "?client_id=" + client_id);
        yield return webRequest.SendWebRequest();

        string textlog = "";

        var values = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(webRequest.downloadHandler.text);
        //foreach (KeyValuePair<string, dynamic> value in values)
        //{
        //    textlog+= value + "\n";
        //}

        textlog += "Track webrequest is done\n";
        log.text = textlog;
        textlog += "Webrequest media attempt\n" + values.TryGetValue("media", out dynamic value);
        log.text = textlog;
        textlog += "Webrequest media value\n";
        log.text = textlog;

        //webRequest.Dispose();


        string link = values["media"]["transcodings"][1]["url"] + "?client_id=" + client_id;
        webRequest = UnityWebRequest.Get(link);
        textlog += "Creating new webrequest\n";
        log.text = textlog;
        yield return webRequest.SendWebRequest();
        var streamURL = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);

        textlog += webRequest.result + "\n";
        log.text = textlog;

        using (var uwr = UnityWebRequestMultimedia.GetAudioClip(streamURL["url"], AudioType.MPEG))
        {
            ((DownloadHandlerAudioClip)uwr.downloadHandler).compressed = false;
            ((DownloadHandlerAudioClip)uwr.downloadHandler).streamAudio = true;
            uwr.SendWebRequest();
            while (uwr.downloadedBytes < 1000000)
            {
                Debug.Log(uwr.downloadedBytes);
                yield return null;
            }

            Debug.Log(((DownloadHandlerAudioClip)uwr.downloadHandler).streamAudio);
                
            /*

            DownloadHandlerAudioClip dHandler = (DownloadHandlerAudioClip)uwr.downloadHandler;
            dHandler.streamAudio = true;
            var downloadAudio = uwr.SendWebRequest();

            textlog += "Sending request for downlaoding audio\n";
            log.text = textlog;

            while (uwr.downloadedBytes < 10000)
                yield return null;
            yield return downloadAudio;

            textlog += "Asigning dHandler as clip\n";
            log.text = textlog;

            AudioClip clip = dHandler.AudioClip;
            */
            //AudioClip clip = ((DownloadHandlerAudioClip)uwr.downloadHandler).AudioClip;

            //Debug.LogError("Clip load state: " + clip.loadState);
            //if(clip.loadState == AudioDataLoadState.Loading) 
            //{
            //    yield return new WaitWhile(() => clip.loadState == AudioDataLoadState.Loading);
            //}
            //Debug.Log("Clip state: " + clip.loadState);
            Debug.LogError("Downloaded audis size: " + uwr.downloadedBytes + " bytes");
            //clip.name = values["publisher_metadata"]["artist"] + " - " + values["title"];

            //textlog += "Clip load state: " + clip.loadState;
            textlog += "\nDownloaded audis size: " + uwr.downloadedBytes + " bytes";

            log.text = textlog;

            PlayList playList = audio.GetPlaylist();
            //playList.clips.Clear();
            //playList.clips.Add(clip);
            //audio.PlayListUpdate();
            audioSource.clip = ((DownloadHandlerAudioClip)uwr.downloadHandler).audioClip;
            Debug.Log(audioSource.clip.loadType);
            audioSource.Play();
            //audioSource.volume = 1f;
            yield return null;
        }
        

        /*
        Debug.Log("ArtworkURL: " + artwork_url);
        Debug.Log("Duration: " + duration);
        Debug.Log("Track ID: " + id);
        Debug.Log("Label name: " + label_name);
        Debug.Log("Title: " + title);
        Debug.Log("Stream URL: " + stream_url);
        */
    }
}
