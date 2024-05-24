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
    [SerializeField]
    private float _displayTime, _fadeTime;
    private float _countdown;
    private bool _isHidden;

    private IEnumerator _switchTrack;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
        _textMesh = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
    void Start()
    {
        _switchTrack = SwitchTrackname();
        _displayTime = sceneStatus.GetTrackDisplayTime();
        _textMesh.text = sceneStatus.GetCurrentTrackName();
        _currentTrack = sceneStatus.GetCurrentTrackID();
        _isHidden = true;
        _isPaused = sceneStatus.IsPaused();
    }

    private void OnEnable()
    {
        PlayerEvents.TrackNameUpdate += UpdateTrackName;
        PlayerEvents.PlayPauseTrack += ShowTrackName;
    }

    private void OnDisable()
    {
        PlayerEvents.TrackNameUpdate -= UpdateTrackName;
        PlayerEvents.PlayPauseTrack -= ShowTrackName;
    }

    private void ShowTrackName()
    {
        Debug.Log("Display track toggle");
        if(!sceneStatus.IsPaused())
        {
            if(!_isHidden) StopAllCoroutines();
            Debug.Log("Playback is paused: " + sceneStatus.IsPaused());
            //
            StartCoroutine(SwitchTrackname());
        }
    }

    private void UpdateTrackName()
    {
        _textMesh.text = sceneStatus.GetCurrentTrackName();
        //StopCoroutine(_switchTrack);
        //_switchTrack.Reset();
        StopAllCoroutines();
        StartCoroutine(SwitchTrackname());
    }

    IEnumerator SwitchTrackname()
    {
        if(_isHidden)
        {
            _countdown = _displayTime;
            yield return StartCoroutine(TrackNameFade(_isHidden, value => _isHidden = value));
        }
        yield return new WaitForSeconds(_displayTime);

        yield return StartCoroutine(TrackNameFade(_isHidden, value => _isHidden = value));
    }

    IEnumerator TrackNameFade(bool isHidden, System.Action<bool> callback)
    {
        float fade = _fadeTime;
        while(fade > 0)
        {
            fade -= Time.deltaTime / _fadeTime;
            _canvasGroup.alpha = isHidden ? 1 - fade : fade;
            yield return null;
        }
        callback(!isHidden);
    }
}