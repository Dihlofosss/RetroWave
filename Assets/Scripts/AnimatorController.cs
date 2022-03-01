using UnityEngine;
using UnityEditor.Animations;

public class AnimatorController : MonoBehaviour
{
    private Audio _audio;
    private MeshGen _meshGen;
    private Animator _animator;
    [SerializeField]
    private UnityEditor.Animations.AnimatorController _controller;
    private void Awake()
    {
        _audio = GetComponentInChildren<Audio>();
        _meshGen = GetComponentInChildren<MeshGen>();
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
    }

    private void Start()
    {
        _animator.runtimeAnimatorController = _controller;
        _animator.enabled = true;
    }

    private void PlayPause()
    {
        _audio.PlayPause();
        _meshGen.PlayPause();
    }
}
