using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UI_Fader : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler
{
    [SerializeField]
    private float _delayBeforeHide, _fadeSpeed;
    private bool _isActive = false;
    private float _displayTime;
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private UnityEngine.Audio.AudioMixerSnapshot[] _snapshots;
    [SerializeField]
    private SceneStatus _sceneStatus;
    
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _displayTime = _delayBeforeHide;
        _canvasGroup.alpha = 0;
    }

    private void UIFadeInOut(bool showUI)
    {
        if(_isActive)
        {
            _displayTime = _delayBeforeHide;
            return;
        }
        StartCoroutine(HideUnhide(showUI));
    }

    

    private IEnumerator DisplayTimer()
    {
        //timeBeforeHide = hideDelay;
        while(_displayTime > 0)
        {
            _displayTime -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(HideUnhide(false));
    }

    private IEnumerator HideUnhide(bool isUnhide)
    {
        if (isUnhide)
            _snapshots[1].TransitionTo(_fadeSpeed);
        else
            _snapshots[0].TransitionTo(_fadeSpeed);

        for(float transition = 0f; transition < 1f; transition += Time.deltaTime / _fadeSpeed)
        {
            _canvasGroup.alpha = isUnhide ? transition : 1f - transition;
            yield return null;
        }
        _canvasGroup.alpha = isUnhide ? 1f : 0f;
        _isActive = !_isActive;
        _sceneStatus.IsUIShown = _isActive;

        if (isUnhide)
        {
            _displayTime = _delayBeforeHide;
            StartCoroutine(DisplayTimer());
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UIFadeInOut(true);
        Debug.Log(_isActive);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        _displayTime = _delayBeforeHide;
    }
}
