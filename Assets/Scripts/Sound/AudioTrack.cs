using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrack : MonoBehaviour
{
    public bool isReadyForPlay { get; private set; } = false;
    public int trackID { get; }
    public int trackDuration { get; }
    public string trackName { get; }
    public string artistName { get; }
    public AudioClip audioClip
    {
        get
        {
            return audioClip;
        }
        set
        {
            if(value != audioClip)
            {
                audioClip = value;
                isReadyForPlay = true;
            }            
        }
    }
    public Texture2D trackCover { get; }

    public AudioTrack(int trackID, int trackDuration, string trackName, string artistName, Texture2D trackCover)
    {
        this.trackID = trackID;
        this.trackDuration = trackDuration;
        this.trackName = trackName;
        this.artistName = artistName;
        this.trackCover = trackCover;
    }
}
