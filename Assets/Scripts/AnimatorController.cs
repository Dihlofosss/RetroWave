using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Audio _audio;
    private MeshGen _meshGen;
    private void Awake()
    {
        _audio = GetComponentInChildren<Audio>();
        _meshGen = GetComponentInChildren<MeshGen>();
    }

    private void PlayPause()
    {
        _audio.PlayPause();
        _meshGen.PauseToggle();
    }
}
