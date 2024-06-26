using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour
{
    private float _pauseFade;
    [SerializeField]
    public OnlinePlayList playList;
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
        if (PlayerPrefs.GetInt("Track", 0) != 0)
            _relatedTrack = PlayerPrefs.GetInt("Track");
        StartCoroutine(Init());
    }

    private void OnEnable()
    {
        PlayerEvents.PlayPauseTrack += PlayPause;
        PlayerEvents.AudioSeek += SeekAudio;
        PlayerEvents.SwitchTrack += SelectNextPrevTrack;
        PlayerEvents.SelectTrack += SelectAndPlayTrack;
    }

    private void OnDisable()
    {
        PlayerEvents.PlayPauseTrack -= PlayPause;
        PlayerEvents.AudioSeek -= SeekAudio;
        PlayerEvents.SwitchTrack -= SelectNextPrevTrack;
        PlayerEvents.SelectTrack -= SelectAndPlayTrack;
    }

    IEnumerator Init()
    {
        audioSource = GetComponent<AudioSource>();
        playlistManager = GetComponent<PlaylistManager>();
        playlistManager.PreparePlaylistForPlayback(_relatedTrack);
        yield return new WaitWhile(() => playList.GetCurrentTrack() == null);
        playlistManager.DownloadTrack(playList.GetCurrentTrack());
        PlayerEvents.OnTrackUpdate(playList.GetCurrentTrack());
        yield return new WaitWhile(() => !playList.GetCurrentTrack().IsReadyForPlay);
        //just in case
        DownloadTracks();

        audioSource.volume = 0;
        audioSource.clip = playList.GetCurrentTrack().AudioClip;
        audioSource.outputAudioMixerGroup = playList.GetMixer();
        _currentTrackLength = playList.GetCurrentTrack().TrackDurationMS * 0.001f;
        _playtime = 0;
        _pauseFade = sceneStatus.GetPauseFade();
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
            sceneStatus.UpdatePlaybackTime(0);
            PlayerEvents.OnSwitchTrack(true);
        }
    }

    public void SelectNextPrevTrack(bool isNextTrack)
    {
        AudioTrack newTrack = isNextTrack ? playList.GetNextTrack() : playList.GetPreviousTrack();
        PlayerEvents.OnSelectTrack(newTrack);
    }

    public void SelectAndPlayTrack(AudioTrack newTrack)
    {
        if (newTrack.Equals(playList.GetCurrentTrack()))
            return;
        playList.CurrentTrackNo = (short)playList.tracks.IndexOf(newTrack);
        StartCoroutine(PlaySelectedTrack(newTrack));
    }

    public void PlayPause()
    {
        StopCoroutine(Pause());
        StopCoroutine(Play());
        StartCoroutine(audioSource.isPlaying ? Pause() : Play());
    }

    private void SeekAudio(float seek)
    {
        _playtime = seek * audioSource.clip.length;
        audioSource.time = _playtime;
        sceneStatus.UpdatePlaybackTime(seek);
    }

    public OnlinePlayList GetPlaylist()
    {
        return this.playList;
    }

    public void SetPlayList(OnlinePlayList newPlaylist)
    {
        this.playList = newPlaylist;
    }

    IEnumerator Play()
    {
        if (!playList.GetCurrentTrack().IsReadyForPlay)
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

    IEnumerator PlaySelectedTrack(AudioTrack newTrack)
    {
        if (!newTrack.IsReadyForPlay)
            playlistManager.DownloadTrack(newTrack);

        if (audioSource.isPlaying)
        {
            StartCoroutine(Pause());
            yield return new WaitWhile(() => audioSource.isPlaying);
            audioSource.Stop();
        }
        yield return new WaitUntil(() => newTrack.IsReadyForPlay);
        PlayerEvents.OnTrackUpdate(newTrack);
        audioSource.clip = newTrack.AudioClip;
        _currentTrackLength = newTrack.TrackDurationMS * 0.001f;
        _playtime = 0;
        StartCoroutine(Play());
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Track", (int)sceneStatus.CurrentTrack.TrackID);
    }
}