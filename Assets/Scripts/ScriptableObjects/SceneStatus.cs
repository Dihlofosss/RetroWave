using UnityEngine;

[CreateAssetMenu(fileName = "SceneStatus", menuName = "ScriptableObjects/SceneStatus", order = 1)]

public class SceneStatus : ScriptableObject
{
    [SerializeField]
    private float sceneSpeed;
    [SerializeField]
    private float pauseFade;
    [SerializeField]
    private bool isPaused, isSpectrumDivided;

    [SerializeField]
    private short _currentTrackID;
    [SerializeField]
    private float _currentTrackPlayback;
    [SerializeField]
    private string _currentTrackName;
    [SerializeField]
    private float _trackDisplayTime;
    [SerializeField]
    private float _sunRise;

    private void Awake()
    {
        _sunRise = 0f;
        _currentTrackPlayback = 0f;
        isSpectrumDivided = false;
    }

    public short GetCurrentTrackID()
    {
        return _currentTrackID;
    }

    public void SetCurrentTrackID(short value)
    {
        _currentTrackID = value;
    }

    public float GetTrackDisplayTime()
    {
        return _trackDisplayTime;
    }

    public string GetCurrentTrackName()
    {
        return _currentTrackName;
    }

    public void SetCurrentTrackName(string value)
    {
        _currentTrackName = value;
    }

    public void DivideTrigger()
    {
        isSpectrumDivided = !isSpectrumDivided;
    }

    public bool IsSpectrumDivided()
    {
        return isSpectrumDivided;
    }

    public float GetSpeed()
    {
        return sceneSpeed;
    }

    public float GetPauseFade()
    {
        return pauseFade;
    }

    public void SetPuase(bool value)
    {
        isPaused = value;
    }

    public void PauseToggle()
    {
        isPaused = !isPaused;
    }

    public bool IsPaused()
    {
        return isPaused;
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
}
