using UnityEngine;
using System.Collections;

public class UI_Fader : MonoBehaviour
{
    [SerializeField]
    private PlayList playList;
    [SerializeField]
    private float hideDelay, transitionSpeed;
    private bool isActive = false;
    private float timeBeforeHide;
    private CanvasGroup canvasGroup;
    private UnityEngine.UI.Image[] images;
    [SerializeField]
    //private UnityEngine.Audio.AudioMixerGroup _mixerGroup;
    private UnityEngine.Audio.AudioMixerSnapshot[] _snapshots;
    private IEnumerator timer;

    void Start()
    {
        //_mixerGroup = playList.GetMixer();
        canvasGroup = GetComponent<CanvasGroup>();
        timeBeforeHide = hideDelay;
        images = GetComponentsInChildren<UnityEngine.UI.Image>();
        timer = AliveTimer();
        //Hide();
    }

    private IEnumerator AliveTimer()
    {
        //timeBeforeHide = hideDelay;
        while(timeBeforeHide > 0)
        {
            timeBeforeHide -= Time.deltaTime;
            yield return null;
        }
        isActive = !isActive;
        StartCoroutine(HideUnhide(isActive));
    }

    private IEnumerator HideUnhide(bool isUnhide)
    {
        if (isUnhide)
            _snapshots[1].TransitionTo(transitionSpeed);
        else
            _snapshots[0].TransitionTo(transitionSpeed);

        for(float transition = 0f; transition < 1f; transition += Time.deltaTime / transitionSpeed)
        {
            canvasGroup.alpha = isUnhide ? transition : 1f - transition;
            yield return null;
        }
        canvasGroup.alpha = isUnhide ? 1f : 0f;

        if(isUnhide)
        {
            timeBeforeHide = hideDelay;
            StartCoroutine(timer);
        }

        foreach (var image in images)
        {
            image.raycastTarget = isUnhide;
        }
        images[0].raycastTarget = true;

        yield return null;
    }

    public void ToggleActive()
    {
        isActive = !isActive;
        if(!isActive)
        {
            StopCoroutine(timer);
            Debug.Log("coroutineStop");
        }
        StartCoroutine(HideUnhide(isActive));
    }

    public void KeepActive()
    {
        isActive = true;
        timeBeforeHide = hideDelay;
    }
}
