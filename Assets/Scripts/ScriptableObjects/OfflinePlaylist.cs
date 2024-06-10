using System.Collections.Generic;
using UnityEngine;
//using System.IO;

[CreateAssetMenu(fileName = "OfflinePlaylist", menuName = "ScriptableObjects/AudioPlaylist/OfflinePlaylist", order = 1)]
public class OfflinePlaylist : ScriptableObject
{
    public UnityEngine.Audio.AudioMixerGroup mixer;

    public List<AudioClip> tracks;

    [SerializeField]
    private short _currentTrack;
    /*
    private string _jsonSaveFile;

    private void Awake()
    {
        _jsonSaveFile = Application.persistentDataPath + name + "playlist.json";

        if (Application.isEditor)
            return;

        if (File.Exists(_jsonSaveFile))
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(_jsonSaveFile), this);
        }

    }
    */
    public AudioClip SwitchToNextAudio()
    {
        _currentTrack++;
        if (_currentTrack >= tracks.Count)
            _currentTrack = 0;
        //File.WriteAllText(_jsonSaveFile, JsonUtility.ToJson(this));
        return tracks[_currentTrack];
    }

    public AudioClip SwitchToPreviousAudio()
    {
        _currentTrack--;
        if (_currentTrack < 0)
            _currentTrack = (short)(tracks.Count - 1);
        //File.WriteAllText(_jsonSaveFile, JsonUtility.ToJson(this));
        return tracks[_currentTrack];
    }

    public AudioClip GetCurrentTrack()
    {
        return tracks.Count == 0 || _currentTrack >= tracks.Count ? null : tracks[_currentTrack];
    }

    public AudioClip GetNextTrack()
    {
        return _currentTrack + 1 >= tracks.Count ? tracks[0] : tracks[_currentTrack + 1];
    }

    public string GetCurrentTrackName()
    {
        return tracks[_currentTrack].name;
    }

    public short GetCurrentTrackNumber()
    {
        return _currentTrack;
    }

    public UnityEngine.Audio.AudioMixerGroup GetMixer()
    {
        return mixer;
    }
}
