using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ForceSettings : MonoBehaviour
{
    public UniversalRenderPipelineAsset URP_Asset;

    public UnityEngine.Rendering.VolumeProfile postProcessPfrofile;


    public short minFPS;


    [Range(0.01f, 0.2f)]
    public float renderScaleStep;

    //[Range(0.5f, 1.0f)][SerializeField]
    //private float _initialRenderScale;

    private float _avarageFps, _avarageFrameTime;
    [SerializeField]
    private float _nearBlur = 5, _farBlur = 100, _blurSize = 1, _transitionScale = 2f;
    private DepthOfField dof;

    private int _framesCounter;
    /*
    private void Awake()
    {
        asset.renderScale = _initialRenderScale;
        Debug.Log("initialRenderScale on start is: " + _initialRenderScale);
    }*/

    private void OnEnable()
    {
        PlayerEvents.UITrigger += TriggerMaxBlur;
    }

    private void OnDisable()
    {
        PlayerEvents.UITrigger -= TriggerMaxBlur;
    }

    void Start()
    {
        foreach (UnityEngine.Rendering.VolumeComponent component in postProcessPfrofile.components)
        {
            if (component.GetType().Equals(typeof(DepthOfField)))
            {
                dof = (DepthOfField)component;
                break;
            }
        }
        if (dof != null)
        {
            ((UnityEngine.Rendering.MinFloatParameter)dof.parameters[1]).value = _nearBlur;
            ((UnityEngine.Rendering.MinFloatParameter)dof.parameters[2]).value = _farBlur;
            ((UnityEngine.Rendering.ClampedFloatParameter)dof.parameters[3]).value = _blurSize;
            ((UnityEngine.Rendering.ClampedFloatParameter)dof.parameters[3]).max = 4;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //QualitySettings.vSyncCount = 0;
        //QualitySettings.maxQueuedFrames = 2;
        Application.targetFrameRate = 60;
        //QualitySettings.antiAliasing = 0;
        //asset.msaaSampleCount = 1;
    }

    private void Update()
    {
        _avarageFrameTime += Time.deltaTime;
        _framesCounter += 1;
        if(_framesCounter >= 10f)
        {
            float renderScale = URP_Asset.renderScale;
            //_avarageFrameTime /= 5f;
            _avarageFps = (_framesCounter + 1) / _avarageFrameTime;

            bool conditionOne = _avarageFps < minFPS - 3 && URP_Asset.renderScale > 0.5f;
            bool conditionTwo = _avarageFps > minFPS + 6 && URP_Asset.renderScale < 1.0f;

            _framesCounter = 0;
            _avarageFrameTime = 0;

            if (conditionOne || conditionTwo)
            {
                renderScale += renderScaleStep * Mathf.Round((_avarageFps - minFPS) * 0.5f);
                URP_Asset.renderScale = Mathf.Clamp(renderScale, 0.5f, 1.0f);
            }
            //save current upscale for the next app start
            //_initialRenderScale = asset.renderScale;
        }
    }

    private IEnumerator DoFTransition(bool state)
    {
        float transitionTime = 0;
        while(transitionTime < 1)
        {
            transitionTime += Time.deltaTime * _transitionScale;
            if (transitionTime > 1) transitionTime = 1;

            ((UnityEngine.Rendering.MinFloatParameter)dof.parameters[1]).value = FloatRemap(_nearBlur, 0.1f, state ? transitionTime : 1 - transitionTime);
            ((UnityEngine.Rendering.MinFloatParameter)dof.parameters[2]).value = FloatRemap(_farBlur, 0.2f, state ? transitionTime : 1 - transitionTime);
            ((UnityEngine.Rendering.ClampedFloatParameter)dof.parameters[3]).value = FloatRemap(_blurSize, 4f, state ? transitionTime : 1 - transitionTime);
            yield return null;
        }
    }

    private float FloatRemap(float min, float max, float value)
    {
        return value * (max - min) + min;
    }    

    private void TriggerMaxBlur(bool maxBlur)
    {
        StopAllCoroutines();
        StartCoroutine(DoFTransition(maxBlur));
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
