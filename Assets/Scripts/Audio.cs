using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour
{
    [SerializeField]
    private float audioFade;
    [SerializeField]
    private PlayList playList;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
        audioSource.clip = playList.GetCurrent();
    }

    public void PlayNext()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        
        audioSource.clip = playList.GetNext();
        audioSource.Play();
    }

    public void PlayPrevious()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        audioSource.clip = playList.GetPrevious();
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