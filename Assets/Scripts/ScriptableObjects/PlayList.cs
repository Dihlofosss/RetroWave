using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
[CreateAssetMenu(fileName = "Playlist", menuName = "ScriptableObjects/Audio/Playlist", order = 1)]
public class PlayList : ScriptableObject
{
    //[System.NonSerialized]
    public UnityEngine.Audio.AudioMixerGroup mixer;

    //[System.NonSerialized]
    public List<AudioClip> clips;

    [SerializeField]
    private short _currentTrack;

    private string jsonSaveFile;

    private void Awake()
    {
        jsonSaveFile = Application.persistentDataPath + name + "playlist.json";

        if (File.Exists(jsonSaveFile))
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(jsonSaveFile), this);
        }
    }

    public AudioClip GetNext()
    {
        _currentTrack++;
        if (_currentTrack >= clips.Count)
            _currentTrack = 0;
        File.WriteAllText(jsonSaveFile, JsonUtility.ToJson(this));
        return clips[_currentTrack];
    }

    public AudioClip GetPrevious()
    {
        _currentTrack--;
        if (_currentTrack < 0)
            _currentTrack = (short)(clips.Count - 1);
        File.WriteAllText(jsonSaveFile, JsonUtility.ToJson(this));
        return clips[_currentTrack];
    }

    public AudioClip GetCurrentTrack()
    {
        return clips[_currentTrack];
    }

    public string GetCurrentTrackName()
    {
        return clips[_currentTrack].name;
    }

    public short GetCurrentTrackID()
    {
        return _currentTrack;
    }

    public UnityEngine.Audio.AudioMixerGroup GetMixer()
    {
        return mixer;
    }
}
