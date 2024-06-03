using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrack
{
    public bool IsReadyForPlay { get; private set; } = false;
    public long TrackID { get; }
    public long TrackDurationMS { get; }
    public string TrackName { get; }
    public string ArtistName { get; }
    public string MediaURL { get; }
    public long PlaybackCount { get; private set; }
    public TrackView TrackView { get; set; }

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
                _audioClip = value;
                IsReadyForPlay = true;
            }            
        }
    }
    public Sprite TrackCover { get; }

    public AudioTrack(long trackID, long trackDuration, long playbackCount, string trackName, string artistName, Sprite trackCover, string mediaURL)
    {
        this.TrackID = trackID;
        this.PlaybackCount = playbackCount;
        this.TrackDurationMS = trackDuration;
        this.TrackName = trackName;
        this.ArtistName = artistName;
        this.TrackCover = trackCover;
        this.MediaURL = mediaURL;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        AudioTrack track = obj as AudioTrack;

        return this.TrackID == track.TrackID;
    }

    public override int GetHashCode()
    {
        return (int)(this.TrackID * this.TrackDurationMS);
    }
}
