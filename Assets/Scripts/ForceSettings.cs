using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ForceSettings : MonoBehaviour
{
    public UniversalRenderPipelineAsset asset;

    public short minFPS;

    [Range(0.01f, 0.2f)]
    public float renderScaleStep;
    [Range(0.5f, 1.0f)]
    public float initialRenderScale;

    private float _avarageFps;
    private float[] _lastTenFps;

    private int _framesCounter;

    private void Awake()
    {
        asset.renderScale = initialRenderScale;
    }

    void Start()
    {
        _lastTenFps = new float[10];
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //QualitySettings.vSyncCount = 0;
        //QualitySettings.maxQueuedFrames = 2;
        //Application.targetFrameRate = 60;
        //QualitySettings.antiAliasing = 0;
        //asset.msaaSampleCount = 1;
    }

    private void Update()
    {
        _lastTenFps[_framesCounter] = 1f / Time.deltaTime;
        _framesCounter += 1;
        if(_framesCounter >= 10)
        {
            _framesCounter -= 10;

            for (short i = 0; i < _lastTenFps.Length; i++)
                _avarageFps += _lastTenFps[i];

            _avarageFps /= 10f;

            if(_avarageFps < minFPS - 5 && asset.renderScale > 0.2f)
            {
                asset.renderScale -= renderScaleStep;
            }
            else if (_avarageFps > minFPS && asset.renderScale < 1f)
            {
                asset.renderScale += renderScaleStep;
            }

            _avarageFps = 0;
        }
    }
}
