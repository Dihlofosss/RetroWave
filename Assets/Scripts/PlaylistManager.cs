using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;

public class PlaylistManager : MonoBehaviour
{
    private readonly string _soundCloudAPI_url = "https://api-v2.soundcloud.com/";
    
    public string clientID { get; set; }

    IEnumerator JsonWebRequest(string uri, System.Action<Dictionary<string, dynamic>> json)
    {
        Debug.Log("Starting webRequest coroutine");
        UnityWebRequest webRequest = UnityWebRequest.Get(uri + "?client_id=" + clientID);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.result);
        //TODO:
        //Add normal error check
        if (webRequest.result != UnityWebRequest.Result.Success)
            json(null);
        else
            json(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(webRequest.downloadHandler.text));
    }

    IEnumerator AudioClipWebRequest(string uri, System.Action<AudioClip> audioClip)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(uri + "?client_id=" + clientID);
        yield return webRequest.SendWebRequest();

        var streamData = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);
        using (var uwrm = UnityWebRequestMultimedia.GetAudioClip(streamData["url"], AudioType.MPEG))
        {
            //streamAudio = true not working atm, unf.
            //actual stream is not creating, streamAudio ignored and audiclip constructor
            //creating clip with the "decompressOnLoad parapm, istead of stream"
            //thus making FMOD going crazy attempting to use seek on not fully downloaded clip
            //
            //hope it will be fixed in next unity version(s)
            //cuz it was working in unity 2018

            ((DownloadHandlerAudioClip)uwrm.downloadHandler).compressed = false;
            ((DownloadHandlerAudioClip)uwrm.downloadHandler).streamAudio = true;

            yield return uwrm.SendWebRequest();
            audioClip(((DownloadHandlerAudioClip)uwrm.downloadHandler).audioClip);
        }
    }
}
