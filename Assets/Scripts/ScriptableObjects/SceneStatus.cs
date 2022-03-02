using UnityEngine;

[CreateAssetMenu(fileName = "SceneStatus", menuName = "ScriptableObjects/SceneStatus", order = 1)]

public class SceneStatus : ScriptableObject
{
    [SerializeField]
    private bool _isPaused;

    [SerializeField]
    private float _currentTrackPlayback;
    [SerializeField]
    private float _sunRise = 0f;

    public void SetPuase(bool value)
    {
        _isPaused = value;
    }

    public void PauseToggle()
    {
        _isPaused = !_isPaused;
    }

    public bool IsPaused()
    {
        return _isPaused;
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
