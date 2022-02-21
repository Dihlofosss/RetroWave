using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ForceSettings : MonoBehaviour
{
    public UniversalRenderPipelineAsset asset;
    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        QualitySettings.maxQueuedFrames = 2;
        Application.targetFrameRate = 30;
        QualitySettings.antiAliasing = 1;
        asset.msaaSampleCount = 1;
    }
}
