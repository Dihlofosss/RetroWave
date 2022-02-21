using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour
{
    [SerializeField]
    private float audioFade;
    [SerializeField]
    private List<AudioClip> clips;
    private AudioSource audioSource;

    private short currentTrack;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
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
            StartCoroutine(Pause());
        else
            StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        audioSource.Play();
        while (audioSource.volume < 1f )
        {
            audioSource.volume += Time.deltaTime / audioFade;
            yield return null;
        }
        audioSource.volume = 1f;
    }

    IEnumerator Pause()
    {
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime / audioFade;
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Pause();
    }
}