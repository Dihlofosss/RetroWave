using UnityEngine;
using System.Collections;

public class SkyController : MonoBehaviour
{
    [SerializeField]
    private ColorPalette colorPalette;
    [SerializeField]
    private SceneStatus sceneStatus;
    [SerializeField]
    private float _sunSpeed;
    [SerializeField]
    private AnimationCurve sunriseCurve;

    private MeshRenderer mRenderer;
    private MaterialPropertyBlock mBlock;

    private float _sunRise, _sunTimer;
    private int _mainSkyColorID, _fadeSkyColorID, _SunColor, _SunriseID;
    private bool _sunDirection;

    private void Awake()
    {
        _mainSkyColorID = Shader.PropertyToID("_MainSkyColor");
        _fadeSkyColorID = Shader.PropertyToID("_FadeSkyColor");
        _SunColor = Shader.PropertyToID("_SunColor");
        _SunriseID = Shader.PropertyToID("_SunRise");

        mRenderer = GetComponent<MeshRenderer>();
        mBlock = new MaterialPropertyBlock();
        mRenderer.GetPropertyBlock(mBlock);

        mBlock.SetColor(_mainSkyColorID, colorPalette.getMainSkyColor());
        mBlock.SetColor(_fadeSkyColorID, colorPalette.getFadeSkyColor());
        mBlock.SetColor(_SunColor, colorPalette.getSunColor());
        mBlock.SetFloat(_SunriseID, _sunRise);

        mRenderer.SetPropertyBlock(mBlock);
    }

    private void Update()
    {
        if (sceneStatus.IsPaused())
            return;

        _sunTimer -= Time.deltaTime;

        if(_sunTimer < 0)
        {
            _sunTimer = Random.Range(3f, 5f) * 60f;
            StartCoroutine(SunMove(_sunDirection));
            _sunDirection = !_sunDirection;
        }

        if (_sunRise == 0 || _sunRise == 1f)
            return;

        mBlock.SetFloat(_SunriseID, _sunRise);
        mRenderer.SetPropertyBlock(mBlock);
    }

    IEnumerator SunMove(bool reverse)
    {
        float counter = 0f;
        while (counter < 1f)
        {
            if(sceneStatus.IsPaused())
            {
                yield return new WaitWhile(() => sceneStatus.IsPaused());
            }
            counter += Time.deltaTime * _sunSpeed;
            _sunRise = !reverse ? sunriseCurve.Evaluate(counter) : sunriseCurve.Evaluate(1 - counter);
            sceneStatus.SetSunrise(_sunRise);
            yield return null;
        }
        _sunRise = !reverse ? 1f : 0f;
    }
}