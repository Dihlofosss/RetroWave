using UnityEngine;

[CreateAssetMenu(fileName = "SceneStatus", menuName = "ScriptableObjects/SceneStatus", order = 1)]

public class SceneStatus : ScriptableObject
{
    [SerializeField]
    private float _sceneSpeed;
    [SerializeField]
    private float _pauseFade;
    [SerializeField]
    private bool _isPaused, _isSpectrumDivided;
    [SerializeField]
    public bool IsUIShown { get; set; }

    public AudioTrack CurrentTrack { get; private set; }
    [SerializeField]
    private float _currentTrackPlayback;
    [SerializeField]
    private float _trackDisplayTime;
    [SerializeField]
    private float _sunRise;

    private void Awake()
    {
        _sunRise = 0f;
        _currentTrackPlayback = 0f;
        _isSpectrumDivided = false;
        IsUIShown = false;
    }

    private void OnEnable()
    {
        PlayerEvents.PlayPauseTrack += PauseToggle;
        PlayerEvents.TrackUpdate += UpdateCurrentTrack;
    }

    private void OnDisable()
    {
        PlayerEvents.PlayPauseTrack -= PauseToggle;
        PlayerEvents.TrackUpdate -= UpdateCurrentTrack;
    }

    public float GetTrackDisplayTime()
    {
        return _trackDisplayTime;
    }

    public void DivideTrigger()
    {
        _isSpectrumDivided = !_isSpectrumDivided;
    }

    public bool IsSpectrumDivided()
    {
        return _isSpectrumDivided;
    }

    public float GetSpeed()
    {
        return _sceneSpeed;
    }

    public float GetPauseFade()
    {
        return _pauseFade;
    }

    public void SetPause(bool value)
    {
        _isPaused = value;
    }

    private void PauseToggle()
    {
        _isPaused = !_isPaused;
    }

    public bool IsPaused()
    {
        return _isPaused;
    }

    public void SetSunrise(float value)
    {
        _sunRise = value;
    }

    public float GetSunrise()
    {
        return _sunRise;
    }

    public void UpdatePlaybackTime(float value)
    {
        _currentTrackPlayback = value;
    }

    public float GetPlaybackTime()
    {
        return _currentTrackPlayback;
    }

    private void UpdateCurrentTrack(AudioTrack newTrack)
    {
        CurrentTrack = newTrack;
    }
}
