using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class SceneFader : MonoBehaviour
{
    private UnityEngine.UI.Image _image;
    private Color _color;

    private void Awake()
    {
        _image = GetComponent<UnityEngine.UI.Image>();
    }

    private void OnEnable()
    {
        PlayerEvents.SceneFade += OnScreenFade;
    }

    private void OnDisable()
    {
        PlayerEvents.SceneFade -= OnScreenFade;
    }

    private void OnScreenFade(bool condition)
    {
        StartCoroutine(ScreenFade(condition));
    }

    private IEnumerator ScreenFade(bool condition)
    {
        float fade = 1f;
        while (fade > 0f)
        {
            fade -= Time.deltaTime;
            if (fade < 0f) fade = 0f;
            SetImageAlpha(_image, condition ? 1f - fade : fade);
            yield return null;
        }
    }

    private void SetImageAlpha(UnityEngine.UI.Image img, float alpha)
    {
        _color = img.color;
        _color.a = alpha;
        img.color = _color;
    }
}
