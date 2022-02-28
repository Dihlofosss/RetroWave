using UnityEngine;

public class Spectrum : MonoBehaviour
{
    private float[] rt_spectrum_L = new float[256];
    private float[] rt_spectrum_R = new float[256];
    private float[] draw_spectrum_L = new float[128];
    private float[] draw_spectrum_R = new float[128];

    [SerializeField]
    private AudioSource source;
    [SerializeField]
    private ColorPalette palette;

    private Renderer mRender;
    private MaterialPropertyBlock mBlock;
    private Texture2D texture;
    private int textureID, boolID, baseColor, peakColor;
    [SerializeField, Range(0,1)]
    private float fade = 0.9f;

    private GameObject[] spectrumParts;


    private void Awake()
    {
        boolID = Shader.PropertyToID("_Hexagonal");
        textureID = Shader.PropertyToID("_MainTex");
        baseColor = Shader.PropertyToID("_Color_Base");
        peakColor = Shader.PropertyToID("_Color_Peak");
        mRender = GetComponent<Renderer>();
        texture = new Texture2D(1, 256, TextureFormat.R8, true);
        mBlock = new MaterialPropertyBlock();
        mRender.GetPropertyBlock(mBlock);
        mBlock.SetColor(baseColor, palette.getDefaultRingColor());
        mBlock.SetColor(peakColor, palette.getPeakRingColor());
        mBlock.SetTexture(textureID, texture);
        mRender.SetPropertyBlock(mBlock);
    }

    private void Start()
    {
        GenQuad();
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
        mBlock.SetTexture(textureID, texture);
        mRender.SetPropertyBlock(mBlock);
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

    private void GenQuad()
    {
        spectrumParts = new GameObject[2];
        Vector3[] vertsLeft = new Vector3[4];
        Vector3[] vertsRight = new Vector3[4];
        Vector2[] uvLeft = new Vector2[4];
        Vector2[] uvRight = new Vector2[4];

        int[] tris = new int[6];

        for (short i = 0, y = 0; y < 2; y++)
        {
            for(short x = 0; x < 2; x++)
            {
                vertsLeft[i] = new Vector3(x * 0.5f - 0.5f, y - 0.5f, 0f);
                vertsRight[i] = new Vector3(x * 0.5f, y - 0.5f, 0f);
                uvLeft[i] = new Vector2(x * 0.5f, y);
                uvRight[i] = new Vector2(x * 0.5f + 0.5f, y);
            }
        }
        Vector3[][] verts = new Vector3[][] { vertsLeft, vertsRight };
        Vector2[][] uvs = new Vector2[][] { uvLeft, uvRight };

        tris[0] = 0;
        tris[3] = tris[2] = 1;
        tris[4] = tris[1] = 2;
        tris[5] = 3;
        
        for (short i = 0; i < (short)transform.childCount; i++)
        {
            spectrumParts[i] = transform.GetChild(i).gameObject;
            MeshFilter filter = spectrumParts[i].AddComponent(typeof(MeshFilter)) as MeshFilter;
            MeshRenderer renderer = spectrumParts[i].AddComponent(typeof(MeshRenderer)) as MeshRenderer;

            filter.mesh = new Mesh();
            filter.mesh.vertices = verts[i];
            filter.mesh.triangles = tris;
            filter.mesh.uv = uvs[i];
            //filter.mesh.RecalculateNormals();
            //filter.mesh.RecalculateBounds();



        }
    }
}