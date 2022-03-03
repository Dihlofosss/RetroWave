using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Playlist", menuName = "ScriptableObjects/Audio/Playlist", order = 1)]
public class PlayList : ScriptableObject
{
    public List<AudioClip> clips;

    [SerializeField]
    private short _currentTrack;

    public AudioClip GetNext()
    {
        _currentTrack++;
        if (_currentTrack >= clips.Count)
            _currentTrack = 0;
        return clips[_currentTrack];
    }

    public AudioClip GetPrevious()
    {
        _currentTrack--;
        if (_currentTrack < 0)
            _currentTrack = (short)(clips.Count - 1);
        return clips[_currentTrack];
    }

    public AudioClip GetCurrent()
    {
        return clips[_currentTrack];
    }

    public string getCurrentTrackName()
    {
        return clips[_currentTrack].name;
    }
}
