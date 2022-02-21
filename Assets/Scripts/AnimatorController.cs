using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Audio _audio;
    private void Awake()
    {
        _audio = GetComponentInChildren<Audio>();
    }

    private void PlayPause()
    {
        _audio.PlayPause();
    }
}
