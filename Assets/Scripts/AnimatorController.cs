using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator _animator;

    [SerializeField]
    private SceneStatus sceneStatus;
    [SerializeField]
    private RuntimeAnimatorController _controller;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
        sceneStatus.SetPause(true);
        sceneStatus.SetSunrise(0f);
    }

    private void OnEnable()
    {
        PlayerEvents.DivideSpectrum += DivideTrigger;
        PlayerEvents.AppStart += OnAppStart;
    }

    private void OnDisable()
    {
        PlayerEvents.DivideSpectrum -= DivideTrigger;
        PlayerEvents.AppStart -= OnAppStart;
    }

    private void OnAppStart()
    {
        _animator.runtimeAnimatorController = _controller;
        _animator.enabled = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void AnimatorStartPlayback()
    {
        PlayerEvents.OnPlayPauseTrack();
    }

    public void DivideTrigger()
    {
        _animator.SetTrigger("Divide");
    }
}