using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour
{
    private float _pauseFade;
    [SerializeField]
    private PlayList playList;
    [SerializeField]
    private SceneStatus sceneStatus;

    private AudioSource audioSource;
    private float _currentTrackLength;
    private float _playtime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
        audioSource.clip = playList.GetCurrentTrack();
        audioSource.outputAudioMixerGroup = playList.GetMixer();
        //_currentTrackLength = audioSource.clip.length;
        _playtime = 0;
        _pauseFade = sceneStatus.GetPauseFade();
        //sceneStatus.SetCurrentTrackID(playList.GetCurrentTrackID());
        //sceneStatus.SetCurrentTrackName(playList.GetCurrentTrackName());
    }
    
    private void Update()
    {
        if (audioSource.isPlaying)
        {
            _playtime += Time.deltaTime;
            //sceneStatus.UpdatePlaybackTime(_playtime / _currentTrackLength);
        }
        else if (sceneStatus.GetPlaybackTime() > 0.95f)
        {
            StartCoroutine(SwitchTrack(true));
        }
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

    public PlayList GetPlaylist()
    {
        return this.playList;
    }

    public void SetPlayList(PlayList newPlaylist)
    {
        this.playList = newPlaylist;
    }

    public void PlayListUpdate()
    {
        audioSource.clip = playList.GetCurrentTrack();
    }

    IEnumerator Play()
    {
        audioSource.Play();
        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / _pauseFade;
            yield return null;
        }
        audioSource.volume = 1f;
    }

    IEnumerator Pause()
    {
        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime / _pauseFade;
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Pause();
    }

    IEnumerator SwitchTrack(bool isForward)
    {
        if (audioSource.isPlaying)
        {
            StartCoroutine(Pause());
            yield return new WaitWhile(() => audioSource.isPlaying);
        }
        audioSource.Stop();

        audioSource.clip = isForward ? playList.GetNext() : playList.GetPrevious();
        _currentTrackLength = audioSource.clip.length;
        _playtime = 0;
        sceneStatus.SetCurrentTrackID(playList.GetCurrentTrackID());
        sceneStatus.SetCurrentTrackName(playList.GetCurrentTrackName());

        StartCoroutine(Play());
    }
}