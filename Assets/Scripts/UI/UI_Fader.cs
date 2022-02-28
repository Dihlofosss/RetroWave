using UnityEngine;

public class UI_Fader : MonoBehaviour
{
    [SerializeField]
    private float hideDelay, transitionSpeed;
    private float alpha;
    private bool isHidden;
    private bool isActive = false;
    private float timeBeforeHide;
    private CanvasGroup canvasGroup;
    private UnityEngine.UI.Image[] images;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        timeBeforeHide = hideDelay;
        images = GetComponentsInChildren<UnityEngine.UI.Image>();
        //Hide();
    }
    void Update()
    {
        if (!isActive)
        {
            if (isHidden)
                return;
            else
                Hide();
        }
        else
        {
            Unhide();
            HideCountdown();
        }

        canvasGroup.alpha = alpha;
    }

    private void Hide()
    {
        alpha -= Time.deltaTime / transitionSpeed;
        if (alpha <= 0)
        {
            alpha = 0;
            isHidden = true;
            foreach (var image in images)
            {
                image.raycastTarget = false;
            }
            images[0].raycastTarget = true;
        }
    }

    private void Unhide()
    {
        alpha += Time.deltaTime / transitionSpeed;
        if (alpha >= 1)
        {
            alpha = 1;
            isHidden = false;
            foreach (var image in images)
            {
                image.raycastTarget = true;
            }
        }
    }

    private void HideCountdown()
    {
        timeBeforeHide -= Time.deltaTime;
        if(timeBeforeHide <= 0)
        {
            isActive = false;
            timeBeforeHide = hideDelay;
        }
    }

    public void ToggleActive()
    {
        isActive = !isActive;
    }

    public void KeepActive()
    {
        isActive = true;
        timeBeforeHide = hideDelay;
    }
}
