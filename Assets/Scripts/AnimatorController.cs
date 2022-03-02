using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Audio _audio;
    private Animator _animator;

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
    }

    private void Start()
    {
        _animator.runtimeAnimatorController = _controller;
        _animator.enabled = true;
    }

    public void PlayPause()
    {
        //sceneStatus.PauseToggle();
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
}