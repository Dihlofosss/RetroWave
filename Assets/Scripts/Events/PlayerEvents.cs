using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{  
    public static event Action PlaylistReady;
    public static event Action DivideSpectrum;
    public static event Action TrackNameUpdate;
    public static event Action PlayPauseTrack;
    public static event Action PauseTrack;

    public static event Action<AudioTrack> SelectTrack;
    public static event Action<bool> SwitchTrack;

    public static void OnSelectTrack(AudioTrack newTrack)
    {
        SelectTrack?.Invoke(newTrack);
    }

    public static void OnPlaylistReady()
    {
        PlaylistReady?.Invoke();
    }

    public static void OnPlayPauseTrack()
    {
        PlayPauseTrack?.Invoke();
    }

    public static void OnSwitchTrack(bool isNextTrack = true)
    {
        SwitchTrack?.Invoke(isNextTrack);
    }

    public static void OnTrackNameUpdate()
    {
        TrackNameUpdate?.Invoke();
    }

    public static void OnDivideSpectrum()
    {
        DivideSpectrum?.Invoke();
    }
}
