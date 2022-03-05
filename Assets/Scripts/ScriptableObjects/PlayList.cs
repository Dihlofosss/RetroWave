using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
[CreateAssetMenu(fileName = "Playlist", menuName = "ScriptableObjects/Audio/Playlist", order = 1)]
public class PlayList : ScriptableObject
{
    public List<AudioClip> clips;

    [SerializeField]
    private short _currentTrack;

    private string jsonSaveFile;

    private void Awake()
    {
        jsonSaveFile = Application.persistentDataPath + "playlist.json";

        if (!File.Exists(jsonSaveFile))
        {
            File.Create(jsonSaveFile);            
        }
        else
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

    public string getCurrentTrackName()
    {
        return clips[_currentTrack].name;
    }

    public short GetCurrentTrackID()
    {
        return _currentTrack;
    }
}
