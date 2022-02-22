using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class SpriteSwap : MonoBehaviour
{
    [SerializeField]
    private Sprite sprite_01, sprite_02;
    private Image _image;
    private bool _isSwapped;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void SpriteBlend()
    {
        if (_isSwapped)
            _image.sprite = sprite_01;
        else
            _image.sprite = sprite_02;
        _isSwapped = !_isSwapped;
    }
}
