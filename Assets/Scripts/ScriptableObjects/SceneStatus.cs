using UnityEngine;

[CreateAssetMenu(fileName = "SceneStatus", menuName = "ScriptableObjects/SceneStatus", order = 1)]

public class SceneStatus : ScriptableObject
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float pauseFade;
    [SerializeField]
    private bool isPaused;
    [SerializeField]
    private bool isSpectrumDivided;

    [SerializeField]
    private float _currentTrackPlayback;
    [SerializeField]
    private float _sunRise;
    [SerializeField]
    private float _trackDisplayTime;

    private void Awake()
    {
        _sunRise = 0f;
        _currentTrackPlayback = 0f;
        isSpectrumDivided = false;
    }

    public float GetTrackDisplayTime()
    {
        return _trackDisplayTime;
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
        return speed;
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
