using System.Collections.Generic;
using UnityEngine;
//using System.IO;

[CreateAssetMenu(fileName = "OnlinePlaylist", menuName = "ScriptableObjects/AudioPlaylist/OnlinePlaylist", order = 1)]
public class OnlinePlayList : ScriptableObject
{
    public UnityEngine.Audio.AudioMixerGroup mixer;

    public List<AudioTrack> tracks;

    [SerializeField]
    public short CurrentTrackNo { get; set; }
    /*
    private string _jsonSaveFile;

    private void Awake()
    {
        _jsonSaveFile = Application.persistentDataPath + name + "playlist.json";

        if(Application.isEditor)
            return;

        if (File.Exists(_jsonSaveFile))
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(_jsonSaveFile), this);
        }

    }
    */
    public AudioTrack GetCurrentTrack()
    {
        return tracks.Count == 0 || CurrentTrackNo >= tracks.Count ? null : tracks[CurrentTrackNo];
    }

    public AudioTrack GetPreviousTrack()
    {
        return CurrentTrackNo - 1 < 0 ? tracks[tracks.Count - 1] : tracks[CurrentTrackNo - 1];
    }

    public AudioTrack GetNextTrack()
    {
        return CurrentTrackNo + 1 >= tracks.Count ? tracks[0] : tracks[CurrentTrackNo + 1];
    }

    public AudioTrack GetTrackByNo(int trackNo)
    {
        return tracks[trackNo];
    }

    public string GetCurrentTrackName()
    {
        return tracks[CurrentTrackNo].ArtistName + " - " + tracks[CurrentTrackNo].TrackName;
    }

    public short GetCurrentTrackNumber()
    {
        return CurrentTrackNo;
    }

    public UnityEngine.Audio.AudioMixerGroup GetMixer()
    {
        return mixer;
    }
}
