using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ForceSettings : MonoBehaviour
{
    public UniversalRenderPipelineAsset asset;
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        QualitySettings.maxQueuedFrames = 2;
        Application.targetFrameRate = 60;
        QualitySettings.antiAliasing = 1;
        asset.msaaSampleCount = 1;
    }
}
