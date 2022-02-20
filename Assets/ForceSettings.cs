using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //QualitySettings.vSyncCount = 0;
        QualitySettings.maxQueuedFrames = 2;
        Application.targetFrameRate = 120;
        QualitySettings.antiAliasing = 2;
    }
}
