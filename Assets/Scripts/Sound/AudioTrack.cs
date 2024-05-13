using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrack
{
    public bool isReadyForPlay { get; private set; } = false;
    public long trackID { get; }
    public float trackDuration { get; }
    public string trackName { get; }
    public string artistName { get; }
    public string mediaURL { get; }

    private AudioClip _audioClip;

    public AudioClip AudioClip
    {
        get
        {
            return _audioClip;
        }
        set
        {
            if(value != _audioClip)
            {
                Debug.Log(value.name);
                _audioClip = value;
                isReadyForPlay = true;
            }            
        }
    }
    public Texture2D trackCover { get; }

    public AudioTrack(long trackID, float trackDuration, string trackName, string artistName, Texture2D trackCover, string mediaURL)
    {
        this.trackID = trackID;
        this.trackDuration = trackDuration;
        this.trackName = trackName;
        this.artistName = artistName;
        this.trackCover = trackCover;
        this.mediaURL = mediaURL;
    }
}
