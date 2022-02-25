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
        StartCoroutine(SwitchTrack(true));
    }

    public void PlayPrevious()
    {
        StartCoroutine(SwitchTrack(false));
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

    IEnumerator SwitchTrack(bool isForward)
    {
        if (audioSource.isPlaying)
        {
            while (audioSource.volume > 0f)
            {
                audioSource.volume -= Time.deltaTime / audioFade;
                yield return null;
            }
            audioSource.volume = 0f;
        }
        audioSource.Stop();

        audioSource.clip = isForward ? playList.GetNext() : playList.GetPrevious();

        audioSource.Play();
        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / audioFade;
            yield return null;
        }
        audioSource.volume = 1f;
    }
}