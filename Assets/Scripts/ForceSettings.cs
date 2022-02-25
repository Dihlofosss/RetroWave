using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ForceSettings : MonoBehaviour
{
    public UniversalRenderPipelineAsset asset;
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        QualitySettings.vSyncCount = 0;
        QualitySettings.maxQueuedFrames = 2;
        Application.targetFrameRate = 300;
        QualitySettings.antiAliasing = 0;
        asset.msaaSampleCount = 1;
    }
}
