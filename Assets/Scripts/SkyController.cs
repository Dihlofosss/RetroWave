using UnityEngine;

public class SkyController : MonoBehaviour
{
    [SerializeField]
    private ColorPalette colorPalette;
    [SerializeField]
    private SceneStatus sceneStatus;
    [SerializeField]
    private AnimationCurve sunriseCurve;

    private MeshRenderer mRenderer;
    private MaterialPropertyBlock mBlock;

    private float _sunRise = 0;
    private int _mainSkyColorID, _fadeSkyColorID, _SunColor, _SunriseID;

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
        mBlock.SetFloat(_SunriseID, sceneStatus.GetSunrise());

        mRenderer.SetPropertyBlock(mBlock);
    }

    private void Update()
    {
        if (_sunRise == sunriseCurve.Evaluate(sceneStatus.GetPlaybackTime()))
            return;

        //_sunRise = sceneStatus.GetSunrise();
        _sunRise = sunriseCurve.Evaluate(sceneStatus.GetPlaybackTime());
        mBlock.SetFloat(_SunriseID, _sunRise);
        mRenderer.SetPropertyBlock(mBlock);
    }
}
