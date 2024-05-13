using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour
{
    private float _pauseFade;
    [SerializeField]
    public PlayList playList;
    [SerializeField]
    private SceneStatus sceneStatus;
    [SerializeField]
    private long _relatedTrack;

    private AudioSource audioSource;
    private PlaylistManager playlistManager;
    private float _currentTrackLength;
    private float _playtime;

    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        audioSource = GetComponent<AudioSource>();
        playlistManager = GetComponent<PlaylistManager>();
        playlistManager.PreparePlaylistForPlayback(_relatedTrack);
        yield return new WaitWhile(() => playList.GetCurrentTrack() == null);
        playlistManager.DownloadTrack(playList.GetCurrentTrack());
        yield return new WaitWhile(() => !playList.GetCurrentTrack().isReadyForPlay);
        //just in case
        DownloadTracks();

        audioSource.volume = 0;
        audioSource.clip = playList.GetCurrentTrack().AudioClip;
        audioSource.outputAudioMixerGroup = playList.GetMixer();
        _currentTrackLength = audioSource.clip.length;
        _playtime = 0;
        _pauseFade = sceneStatus.GetPauseFade();
        sceneStatus.SetCurrentTrackID(playList.GetCurrentTrackNumber());
        sceneStatus.SetCurrentTrackName(playList.GetCurrentTrackName());
    }

    private void DownloadTracks()
    {
        playlistManager.DownloadTrack(playList.GetCurrentTrack());
        playlistManager.DownloadTrack(playList.GetNextTrack());
    }
    
    private void Update()
    {
        if (audioSource.isPlaying)
        {
            _playtime += Time.deltaTime;
            sceneStatus.UpdatePlaybackTime(_playtime / _currentTrackLength);
        }
        else if (sceneStatus.GetPlaybackTime() > 0.99f)
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

        Debug.Log(playList.GetCurrentTrack().trackDuration);
        Debug.Log(playList.GetCurrentTrack().AudioClip.length);
    }

    public PlayList GetPlaylist()
    {
        return this.playList;
    }

    public void SetPlayList(PlayList newPlaylist)
    {
        this.playList = newPlaylist;
    }

    IEnumerator Play()
    {
        if (!playList.GetCurrentTrack().isReadyForPlay)
            DownloadTracks();

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

        AudioTrack track = isForward ? playList.SwitchToNextAudio() : playList.SwitchToPreviousAudio();
        DownloadTracks();
        yield return new WaitUntil(() => track.isReadyForPlay);

        audioSource.clip = track.AudioClip;
        _currentTrackLength = track.trackDuration;
        _playtime = 0;
        sceneStatus.SetCurrentTrackID(playList.GetCurrentTrackNumber());
        sceneStatus.SetCurrentTrackName(playList.GetCurrentTrackName());
        DownloadTracks();

        StartCoroutine(Play());
    }
}