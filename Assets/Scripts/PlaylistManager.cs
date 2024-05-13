using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;

public class PlaylistManager : MonoBehaviour
{
    private readonly string _soundCloudAPI_url = "https://api-v2.soundcloud.com";

    [SerializeField]
    private string _clientID;
    public PlayList playList { get; private set; }


    private void Awake()
    {
        playList = GetComponent<Audio>().playList;
        if (playList.tracks == null)
            playList.tracks = new();
    }

    public void ClearPlaylist()
    {
        playList.tracks.Clear();
    }

    public void DownloadTrack(AudioTrack track)
    {
        if (track == null || track.isReadyForPlay)
            return;
        StartCoroutine(PrepareTrackForPlay(track));
    }

    public void PreparePlaylistForPlayback(int relatedTrack)
    {
        StartCoroutine(FillPlaylist(relatedTrack, playList, 30));
    }

    IEnumerator FillPlaylist(int relatedTrack, PlayList playList, int tracksAmount)
    {
        string uri = _soundCloudAPI_url + "/tracks/" + relatedTrack + "/related?client_id=" + _clientID + "&limit=" + tracksAmount;
        Dictionary<string, dynamic> relatedTracksData = null;
        StartCoroutine(JsonWebRequest(uri, value => relatedTracksData = value));

        List<long> tracksIDsList = new();

        if(playList.tracks.Count != 0)
        {
            foreach(AudioTrack track in playList.tracks)
            {
                tracksIDsList.Add(track.trackID);
            }
        }

        yield return relatedTracksData;

        for(int i = 0; i < relatedTracksData["collection"].Count; i++)
        {
            Dictionary<string, dynamic> trackData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(relatedTracksData["collection"][i].ToString());
            if (tracksIDsList.Contains(trackData["id"]))
                continue;
            
            AudioTrack newTrack = null;
            StartCoroutine(AudioTrackWebRequest(trackData, value => newTrack = value));
            yield return newTrack;
            tracksIDsList.Add(trackData["id"]);
            playList.tracks.Add(newTrack);
        }
    }

    IEnumerator AudioTrackWebRequest(long TrackID, System.Action<AudioTrack> audioTrack)
    {
        string uri = _soundCloudAPI_url + "/tracks/" + TrackID + "?client_id=" + _clientID;
        Dictionary<string, dynamic> trackData = null;
        StartCoroutine(JsonWebRequest(uri, value => trackData = value));
        yield return trackData;
        AudioTrack newTrack = null;
        StartCoroutine(AudioTrackWebRequest(trackData, value => newTrack = value));
        audioTrack(newTrack);        
    }

    IEnumerator AudioTrackWebRequest(Dictionary<string, dynamic> trackData, System.Action<AudioTrack> audioTrack)
    {
        Texture2D artwork = null;
        StartCoroutine(GetWebTexture((string)trackData["artwork_url"], value => artwork = value));
        long trackID = trackData["id"];
        float duration = ((float)trackData["duration"]) * 0.01f;
        string title = trackData["title"];
        // transcodings:
        // 0 - HLS - MP3
        // 1 - Progressive MP3
        // 2 - HLS - OGG
        string mediaURL = trackData["media"]["transcodings"][1]["url"];
        string artistName = trackData["user"]["username"];
        yield return artwork;
        AudioTrack newTrack = new(trackID, duration, title, artistName, artwork, mediaURL);
        audioTrack(newTrack);
    }

    IEnumerator PrepareTrackForPlay(AudioTrack track)
    {
        Debug.Log("Starting track download " + track.trackName);
        AudioClip audioClip = null;
        yield return StartCoroutine(AudioClipWebRequest(track.mediaURL, value => audioClip = value));
        yield return audioClip;
        audioClip.name = track.trackID.ToString();
        track.AudioClip = audioClip;
        yield return null;
    }

    IEnumerator JsonWebRequest(string uri, System.Action<Dictionary<string, dynamic>> json)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.result);
        Debug.Log(uri);
        //TODO:
        //Add normal error check
        if (webRequest.result != UnityWebRequest.Result.Success)
            json(null);
        else
            json(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(webRequest.downloadHandler.text));
    }

    IEnumerator GetWebTexture(string uri, System.Action<Texture2D> texture)
    {
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri);
        yield return webRequest.SendWebRequest();
        Texture2D texture2D = DownloadHandlerTexture.GetContent(webRequest);
        yield return texture2D;
        texture(texture2D);
    }

    IEnumerator AudioClipWebRequest(string uri, System.Action<AudioClip> audioClip)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(uri + "?client_id=" + _clientID);
        yield return webRequest.SendWebRequest();

        var streamData = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);
        using (var uwrm = UnityWebRequestMultimedia.GetAudioClip(streamData["url"], AudioType.MPEG))
        {
            //streamAudio = true not working atm, unf.
            //actual stream is not creating, streamAudio ignored and audiclip constructor
            //creating  clip with the "decompressOnLoad parapm, istead of stream"
            //thus making FMOD going crazy attempting to use seek on not fully downloaded clip
            //
            //hope it will be fixed in next unity version(s)
            //cuz it was working in unity 2018

            //TODO:
            //implement MP3 files download locally and stream data from local storage instead
            //of creating AudiClip via DownloadHandlerAudioClip 
            ((DownloadHandlerAudioClip)uwrm.downloadHandler).compressed = false;
            ((DownloadHandlerAudioClip)uwrm.downloadHandler).streamAudio = true;
            uwrm.SendWebRequest();
            yield return new WaitWhile(() => !uwrm.downloadHandler.isDone);
            audioClip(((DownloadHandlerAudioClip)uwrm.downloadHandler).audioClip);
        }
    }
}