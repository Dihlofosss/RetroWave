using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> clips;
    private AudioSource audioSource;

    private short currentTrack;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayNext()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        currentTrack++;
        if (currentTrack >= clips.Count)
            currentTrack = 0;
        audioSource.clip = clips[currentTrack];
        audioSource.Play();
    }

    public void PlayPrevious()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        currentTrack--;
        if (currentTrack < 0)
            currentTrack = (short)(clips.Count - 1);
        audioSource.clip = clips[currentTrack];
        audioSource.Play();
    }

    public void PlayPause()
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
        else
            audioSource.Play();
    }
}