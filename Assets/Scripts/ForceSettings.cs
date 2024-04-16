using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ForceSettings : MonoBehaviour
{
    public UniversalRenderPipelineAsset asset;


    public short minFPS;


    [Range(0.01f, 0.2f)]
    public float renderScaleStep;

    //[Range(0.5f, 1.0f)][SerializeField]
    //private float _initialRenderScale;

    private float _avarageFps, _avarageFrameTime;


    private int _framesCounter;
    /*
    private void Awake()
    {
        asset.renderScale = _initialRenderScale;
        Debug.Log("initialRenderScale on start is: " + _initialRenderScale);
    }*/
    
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //QualitySettings.vSyncCount = 0;
        //QualitySettings.maxQueuedFrames = 2;
        Application.targetFrameRate = 72;
        //QualitySettings.antiAliasing = 0;
        //asset.msaaSampleCount = 1;
    }

    private void Update()
    {
        _avarageFrameTime += Time.deltaTime;
        _framesCounter += 1;
        if(_framesCounter >= 10f)
        {
            //_avarageFrameTime /= 5f;
            _avarageFps = (_framesCounter + 1) / _avarageFrameTime;

            bool conditionOne = _avarageFps < minFPS - 3 && asset.renderScale > 0.2f;
            bool conditionTwo = _avarageFps > minFPS + 6 && asset.renderScale < 1.5f;

            if (conditionOne || conditionTwo)
            {
                asset.renderScale += renderScaleStep * Mathf.Round((_avarageFps - minFPS) * 0.5f);
            }

            _framesCounter = 0;
            _avarageFrameTime = 0;
            //save current upscale for the next app start
            //_initialRenderScale = asset.renderScale;
        }
    }
    /*
    private void OnApplicationPause(bool pause)
    {
        if (!pause)
            return;

        _initialRenderScale = asset.renderScale;
        Debug.Log("initialRenderScale on pause is: " + _initialRenderScale);
    }

    private void OnApplicationQuit()
    {
        _initialRenderScale = asset.renderScale;
        Debug.Log("initialRenderScale on exit is: " + _initialRenderScale);
    }

    private void OnApplicationFocus(bool focus)
    {
        _initialRenderScale = asset.renderScale;
        Debug.Log("initialRenderScale on focus true is: " + _initialRenderScale);
    }*/
}
