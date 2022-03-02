using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour
{
    [SerializeField]
    private float audioFade;
    [SerializeField]
    private PlayList playList;
    [SerializeField]
    private SceneStatus sceneStatus;
    //private bool _pauseToggle = true;

    private AudioSource audioSource;
    private float _currentTrackLength;
    private float _playtime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
        audioSource.clip = playList.GetCurrent();
        _currentTrackLength = audioSource.clip.length;
        _playtime = 0;
        //_pauseToggle = sceneStatus.IsPaused();
    }
    
    private void Update()
    {
        if (audioSource.isPlaying)
        {
            _playtime += Time.deltaTime;
            sceneStatus.UpdatePlaybackTime(_playtime / _currentTrackLength);
        }
            
        /*
        if(_pauseToggle != sceneStatus.IsPaused())
        {
            _pauseToggle = sceneStatus.IsPaused();
            PlayPause();
        }
        */


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
        StopCoroutine(Pause());
        StopCoroutine(Play());
        if (audioSource.isPlaying)
            StartCoroutine(Pause());
        else
            StartCoroutine(Play());
        sceneStatus.PauseToggle();
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
            PlayPause();
            yield return new WaitWhile(() => audioSource.isPlaying);
        }
        audioSource.Stop();

        audioSource.clip = isForward ? playList.GetNext() : playList.GetPrevious();
        _currentTrackLength = audioSource.clip.length;
        _playtime = 0;

        PlayPause();
    }
}