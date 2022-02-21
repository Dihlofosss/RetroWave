using UnityEngine;

public class Spectrum : MonoBehaviour
{
    private float[] rt_spectrum_L = new float[256];
    private float[] rt_spectrum_R = new float[256];
    private float[] draw_spectrum_L = new float[128];
    private float[] draw_spectrum_R = new float[128];

    [SerializeField]
    private AudioSource source;

    private Renderer mRender;
    private MaterialPropertyBlock mBlock;
    private Texture2D texture;
    private int textureID;
    private int boolID;
    private int graphFadeInID;
    [SerializeField, Range(0,1)]
    private float fade = 0.9f;
    private float graphFadeIn = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        boolID = Shader.PropertyToID("_Hexagonal");
        textureID = Shader.PropertyToID("_MainTex");
        graphFadeInID = Shader.PropertyToID("_FadeIn");
        mRender = GetComponent<Renderer>();
        texture = new Texture2D(1, 256, TextureFormat.R8, true);
        mBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        float soft = fade * Mathf.Clamp01(1 - Time.deltaTime);

        mRender.GetPropertyBlock(mBlock);

        //AudioListener.GetSpectrumData(rt_spectrum_L, 0, FFTWindow.Rectangular);
        //AudioListener.GetSpectrumData(rt_spectrum_R, 1, FFTWindow.Rectangular);
        source.GetSpectrumData(rt_spectrum_L, 0, FFTWindow.Rectangular);
        source.GetSpectrumData(rt_spectrum_R, 1, FFTWindow.Rectangular);

        for (short i = 0; i < 128; i++)
        {
            if (rt_spectrum_L[i] > draw_spectrum_L[i])
                draw_spectrum_L[i] = rt_spectrum_L[i];
            else
                draw_spectrum_L[i] *= soft;

            if (rt_spectrum_R[i] > draw_spectrum_R[i])
                draw_spectrum_R[i] = rt_spectrum_R[i];
            else
                draw_spectrum_R[i] *= soft;

            texture.SetPixel(0, i, getColor(draw_spectrum_R[i]));
            texture.SetPixel(0, 255 - i, getColor(draw_spectrum_L[i]));
        }
        texture.Apply();
        //if (graphFadeIn < 1)
        //{
        //    graphFadeIn += Time.deltaTime;
        //    mBlock.SetFloat(graphFadeInID, graphFadeIn);
        //}
        //mRender.material.mainTexture = texture;
        mBlock.SetTexture(textureID, texture);
        mRender.SetPropertyBlock(mBlock);

        //Debug.Log(AudioSettings.dspTime);
    }

    private Color getColor(float value)
    {
        value = (1 - Mathf.Pow(1 - value, 6));
        return new Color(value, 0, 0);
    }

    public void ToggleHexagon()
    {
        mRender.GetPropertyBlock(mBlock);
        mBlock.SetFloat(boolID, 1 - mBlock.GetFloat(boolID));
        mRender.SetPropertyBlock(mBlock);
    }
}