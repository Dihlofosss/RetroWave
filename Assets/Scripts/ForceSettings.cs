using UnityEngine;
using UnityEngine.Rendering;

public class ForceSettings : MonoBehaviour
{
    //public ScriptableRendererData asset;
    void Awake()
    {
        //QualitySettings.vSyncCount = 0;
        QualitySettings.maxQueuedFrames = 2;
        Application.targetFrameRate = 120;
        QualitySettings.antiAliasing = 2;
        
    }
}
