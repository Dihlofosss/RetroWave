using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Audio _audio;
    private Animator _animator;
    private bool _isSpectrumDivided;

    [SerializeField]
    private SceneStatus sceneStatus;
    [SerializeField]
    private RuntimeAnimatorController _controller;

    private void Awake()
    {
        _audio = GetComponentInChildren<Audio>();
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
        sceneStatus.SetPause(true);
        sceneStatus.SetSunrise(0f);
        _animator.runtimeAnimatorController = _controller;
    }

    private void OnEnable()
    {
        PlayerEvents.DivideSpectrum += DivideTrigger;
    }

    private void OnDisable()
    {
        PlayerEvents.DivideSpectrum -= DivideTrigger;
    }

    private void Start()
    {
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
        //_audio.PlayPause();
        //sceneStatus.PauseToggle();
        PlayerEvents.OnPlayPauseTrack();
    }

    public void DivideTrigger()
    {
        _animator.SetTrigger("Divide");
    }
}