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
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        timeBeforeHide = hideDelay;
    }

    // Update is called once per frame
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
        }
    }

    private void Unhide()
    {
        alpha += Time.deltaTime / transitionSpeed;
        if (alpha >= 1)
        {
            alpha = 1;
            isHidden = false;
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
        Debug.Log("clicked");
    }

    public void KeepActive()
    {
        isActive = true;
        timeBeforeHide = hideDelay;
    }
}
