using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using UnityEngine;


[RequireComponent(typeof(UnityEngine.UI.Image))]
public class SimpleButton : MonoBehaviour, IPointerClickHandler
{
    [Serializable]
    public class CickEvent : UnityEvent { }

    [SerializeField]
    private bool _isToggleable = false;
    private bool _isToggled = false;

    [SerializeField]
    private CickEvent _onClick = new();
    public CickEvent OnClick
    {
        get
        {
            return _onClick;
        }
        set
        {
            _onClick = value;
        }
    }

    [SerializeField]
    private Sprite _mainSprite, _swapSprite;
    private UnityEngine.UI.Image _canvasImage;

    private void Awake()
    {
        _canvasImage = GetComponent<UnityEngine.UI.Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!UI_Fader.IsUIActive)
            return;

        _onClick?.Invoke();
        if (_isToggleable)
            SpriteSwap();
    }

    private void SpriteSwap()
    {
        _canvasImage.sprite = _isToggled ? _mainSprite : _swapSprite;
        _isToggled = !_isToggled;
    }
}