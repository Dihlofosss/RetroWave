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
        sceneStatus.SetPuase(true);
        sceneStatus.SetSunrise(0f);
        _animator.runtimeAnimatorController = _controller;
    }

    private void Start()
    {
        _animator.enabled = true;
    }

    private void Update()
    {
        if(_isSpectrumDivided != sceneStatus.IsSpectrumDivided())
        {
            DivideTrigger();
            _isSpectrumDivided = !_isSpectrumDivided;
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void PlayPause()
    {
        _audio.PlayPause();
    }

    public void NextTrack()
    {
        _audio.PlayNext();
    }

    public void PreviousTrack()
    {
        _audio.PlayPrevious();
    }

    public void DivideTrigger()
    {
        _animator.SetTrigger("Divide");
    }
}