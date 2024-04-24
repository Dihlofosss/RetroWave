using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;

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
    

    public string client_id;

    private AudioSource audioSource;
    private Audio audio;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(GetRequest("https://api-v2.soundcloud.com/tracks/"));
        audioSource = GetComponent<AudioSource>();
        audio = GetComponent<Audio>();
    }

    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(uri + id + "?client_id=" + client_id);
        yield return webRequest.SendWebRequest();

        var values = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(webRequest.downloadHandler.text);
        foreach (KeyValuePair<string, dynamic> value in values)
        {
            Debug.Log(value);
        }    

        webRequest = UnityWebRequest.Get((string)values["media"]["transcodings"][1]["url"] + "?client_id=" + client_id);
        yield return webRequest.SendWebRequest();
        var streamURL = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);

        using (var uwr = UnityWebRequestMultimedia.GetAudioClip(streamURL["url"], AudioType.MPEG))
        {
            DownloadHandlerAudioClip dHandler = (DownloadHandlerAudioClip)uwr.downloadHandler;
            dHandler.streamAudio = true;
            var downloadAudio = uwr.SendWebRequest();
            yield return downloadAudio;

            while (uwr.downloadedBytes < 10000)
                yield return null;
            AudioClip clip = dHandler.audioClip;


            Debug.Log("Clip load state: " + clip.loadState);
            //if(clip.loadState == AudioDataLoadState.Loading) 
            //{
            //    yield return new WaitWhile(() => clip.loadState == AudioDataLoadState.Loading);
            //}
            //Debug.Log("Clip state: " + clip.loadState);
            Debug.Log("Downloaded audis size: " + uwr.downloadedBytes + " bytes");
            clip.name = values["publisher_metadata"]["artist"] + " - " + values["title"];

            PlayList playList = audio.GetPlaylist();
            playList.clips.Clear();
            playList.clips.Add(clip);
            audio.PlayListUpdate();
            //audio.SetPlayList(playList);
            //audioSource.clip = clip;
            Debug.Log(clip.name);


            //audioSource.Play();
            yield return downloadAudio;
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
