using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTrack : MonoBehaviour
{
    [SerializeField]
    private SceneStatus sceneStatus;

    private CanvasGroup _canvasGroup;
    private TMPro.TextMeshProUGUI _textMesh;
    [SerializeField]
    private float _displayTime, _fadeTime;
    private float _delay;
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
        //_textMesh.text = sceneStatus.CurrentTrack.TrackName + " - " + sceneStatus.CurrentTrack.ArtistName;
        _isHidden = true;
    }

    private void OnEnable()
    {
        PlayerEvents.TrackUpdate += UpdateTrackName;
        PlayerEvents.PlayPauseTrack += ShowTrackName;
    }

    private void OnDisable()
    {
        PlayerEvents.TrackUpdate -= UpdateTrackName;
        PlayerEvents.PlayPauseTrack -= ShowTrackName;
    }

    private void ShowTrackName()
    {
        Debug.Log("Display track toggle");
        if(!sceneStatus.IsPaused())
        {
            if(!_isHidden) StopAllCoroutines();
            StartCoroutine(SwitchTrackname());
        }
    }

    private void UpdateTrackName(AudioTrack newTrack)
    {
        _textMesh.text = newTrack.TrackName + " - " + newTrack.ArtistName;
        if (sceneStatus.IsPaused() || sceneStatus.IsUIShown)
            return;
        if(!_isHidden)
        {
            _delay = _fadeTime;
            return;
        }
        StopAllCoroutines();
        StartCoroutine(SwitchTrackname());
    }

    IEnumerator SwitchTrackname()
    {
        if(_isHidden)
        {
            yield return StartCoroutine(FadeInOut(_isHidden, value => _isHidden = value));
        }
        yield return DisplayTimer(_displayTime);
    }

    IEnumerator FadeInOut(bool isHidden, System.Action<bool> callback)
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

    IEnumerator DisplayTimer(float time)
    {
        _delay = time;
        while (_delay > 0)
        {
            _delay -= Time.deltaTime;
            yield return null;
        }
        yield return StartCoroutine(FadeInOut(_isHidden, value => _isHidden = value));
    }
}