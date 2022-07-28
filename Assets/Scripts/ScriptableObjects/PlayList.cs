using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "Playlist", menuName = "ScriptableObjects/Audio/Playlist", order = 1)]
public class PlayList : ScriptableObject
{
    public UnityEngine.Audio.AudioMixerGroup mixer;

    public List<AudioClip> clips;

    private short _currentTrack;

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

    public AudioClip GetNext()
    {
        _currentTrack++;
        if (_currentTrack >= clips.Count)
            _currentTrack = 0;
        File.WriteAllText(_jsonSaveFile, JsonUtility.ToJson(this));
        return clips[_currentTrack];
    }

    public AudioClip GetPrevious()
    {
        _currentTrack--;
        if (_currentTrack < 0)
            _currentTrack = (short)(clips.Count - 1);
        File.WriteAllText(_jsonSaveFile, JsonUtility.ToJson(this));
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
