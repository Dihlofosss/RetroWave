using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


[RequireComponent(typeof(UnityEngine.UI.Image))]
public class StartButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private UI_Fader ui;

    private Image _thisImg;
    private Color _color;

    private void Awake()
    {
        ui.enabled = false;
        _thisImg = GetComponent<Image>();
        SetImageAlpha(_thisImg, 0f);
    }

    private void Start()
    {
        StartCoroutine(ButtonFadeIn());
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(StartApp());
    }

    private IEnumerator ButtonFadeIn()
    {
        float fade = 0f;
        while (fade < 1f)
        {
            fade += Time.deltaTime;
            SetImageAlpha(_thisImg, fade > 1f ? 1f : fade);
            yield return null;
        }
    }

    private IEnumerator StartApp()
    {
        float fade = 1f;
        while (fade > 0f)
        {
            fade -= Time.deltaTime * 2f;
            SetImageAlpha(_thisImg, fade <= 0f ? 0f : fade);
            yield return null;
        }
        PlayerEvents.OnAppStart();
        PlayerEvents.OnSceneFade(false);
        ui.enabled = true;
        gameObject.SetActive(false);
    }

    private void SetImageAlpha(Image img, float alpha)
    {
        _color = img.color;
        _color.a = alpha;
        img.color = _color;
    }
}
