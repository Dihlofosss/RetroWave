using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTrack : MonoBehaviour
{
    [SerializeField]
    private SceneStatus sceneStatus;

    private CanvasGroup _canvasGroup;
    private TMPro.TextMeshProUGUI _textMesh;
    private bool _isPaused = true;

    private short _currentTrack;
    private float _displayTime, _fadeTime, _countdown;
    private bool _isHidden;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _textMesh = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
    void Start()
    {
        _displayTime = sceneStatus.GetTrackDisplayTime();
        _fadeTime = 1f;
        _displayTime = 3f;
        _textMesh.text = sceneStatus.GetCurrentTrackName();
        _currentTrack = sceneStatus.GetCurrentTrackID();
        _isHidden = true;
        _isPaused = sceneStatus.IsPaused();
    }

    void Update()
    {
        if (_countdown > 0)
        {
            _countdown -= Time.deltaTime;
            if (_currentTrack != sceneStatus.GetCurrentTrackID() || _isPaused != sceneStatus.IsPaused())
            {
                _currentTrack = sceneStatus.GetCurrentTrackID();
                _isPaused = sceneStatus.IsPaused();
                _textMesh.text = sceneStatus.GetCurrentTrackName();
                _countdown = _displayTime;
                return;
            }
        }

        if (_currentTrack != sceneStatus.GetCurrentTrackID() || _isPaused != sceneStatus.IsPaused())
        {
            _countdown = _displayTime;
            _currentTrack = sceneStatus.GetCurrentTrackID();
            _isPaused = sceneStatus.IsPaused();
            _textMesh.text = sceneStatus.GetCurrentTrackName();
            StartCoroutine(SwitchTrackname());
        }    
    }

    IEnumerator SwitchTrackname()
    {
        if(_isHidden)
        {
            _countdown = _displayTime;
            StartCoroutine(TrackNameFade(_isHidden));
        }
        yield return new WaitWhile(() => _countdown > 0);

        StartCoroutine(TrackNameFade(_isHidden));
    }

    IEnumerator TrackNameFade(bool isHidden)
    {
        float fade = _fadeTime;
        while(fade > 0)
        {
            fade -= Time.deltaTime / _fadeTime;
            _canvasGroup.alpha = isHidden ? 1 - fade : fade;
            yield return null;
        }
        _isHidden = !_isHidden;
    }
}
