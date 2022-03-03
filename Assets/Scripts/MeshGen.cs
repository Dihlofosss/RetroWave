using static Unity.Mathematics.math;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Mesh), typeof(MeshFilter))]
public class MeshGen : MonoBehaviour
{
    private Mesh mesh;

    private Texture2D _texture;
    private List<Vector3> _verts = new List<Vector3>();
    private List<Color> _colors = new List<Color>();
    private int[] _tris;
    private Vector2[] _uv;
    private float[] _sound = new float[64];
    private bool _pauseToggle = true;
    private bool _isPaused;
    private float _pauseScale = 0;
    private float _globalPausingSpeed;

    [SerializeField]
    private ColorPalette colorPalette;
    [SerializeField]
    private SceneStatus sceneStatus;
    [SerializeField]
    private int width, length;
    [SerializeField]
    private bool useTexture;
    private float counter;

    private void Awake()
    {
        mesh = new Mesh
        {
            name = "Procedural Grid"
        };
        GetComponent<MeshFilter>().mesh = mesh;

        _tris = new int[4 * width * length * 6];
        _uv = new Vector2[(width * 2 + 1) * (length * 2 + 1)];
        _texture = new Texture2D(128, 128, TextureFormat.R8, true);
        _texture.anisoLevel = 8;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock mBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mBlock);
        mBlock.SetColor("_MainSkyColor", colorPalette.getMainSkyColor());
        mBlock.SetColor("_FadeSkyColor", colorPalette.getFadeSkyColor());
        renderer.SetPropertyBlock(mBlock);
    }

    void Start()
    {
        for(short i = 0, z = (short)-length; z <= length; z++)
        {
            for(short x = (short)-width; x <= width; x++, i++)
            {
                _verts.Add(new Vector3(x, 0, z));
                _uv[i] = new Vector2(x, z);
                _colors.Add(colorPalette.getDefaultGridColor());
            }
        }
        mesh.vertices = _verts.ToArray();
        mesh.uv = _uv;
        mesh.colors = _colors.ToArray();
        
        rebuildTris();
        if(useTexture)
            generateGridTexture();
    }

    void Update()
    {
        if (_pauseToggle != sceneStatus.IsPaused())
        {
            _pauseToggle = sceneStatus.IsPaused();
            PlayPause();
        }

        if (_isPaused)
            return;

        counter += Time.deltaTime * _pauseScale;
        //counter *= _pauseScale;
        if(counter >= 1)
        {
            counter = frac(counter);
            rebuildVerts(counter);
        }
        moveGrid();
        rebuildTris();
    }

    private void rebuildVerts(float shift)
    {
        float height;
        AudioListener.GetSpectrumData(_sound, 0, FFTWindow.Rectangular);
        //remove last poly row
        for(int i = _verts.Count; i <= _verts.Count - (2 * width); i--)
        {
            _verts.RemoveAt(i - 1);
            _colors.RemoveAt(i - 1);
        }
        //add vert for 1st poly row
        for(int i = width; i >= -width; i--)
        {
            height = Mathf.Abs(i) - width / 4;
            height = 1 - Mathf.Pow(1 - _sound[Mathf.Abs(Mathf.Abs((int)height) - (width / 2))], 2);
            height *= 3f;

            if (i < 2 && i > -2)
                height *= 0.1f;
            _verts.Insert(0, new Vector3(i, height , shift - length));
            
            _colors.Insert(0, Color.Lerp(colorPalette.getDefaultGridColor(), colorPalette.getPeakGridColor(), height / 3f));
        }
        _verts.TrimExcess();
        mesh.vertices = _verts.ToArray();
        mesh.colors = _colors.ToArray();
    }

    private void moveGrid()
    {
        for(short i = 0; i < _verts.Count; i++)
        {
            //verts[i].z -= Time.deltaTime;
            _verts[i] = new Vector3(_verts[i].x, _verts[i].y, _verts[i].z + Time.deltaTime * _pauseScale);
        }
        mesh.vertices = _verts.ToArray();
    }

    private void rebuildTris()
    {
        for (short ti = 0, vi = 0, y = 0; y < length * 2; y++, vi++)
        {
            for (short x = 0; x < width * 2; x++, ti += 6, vi++)
            {
                _tris[ti] = vi;
                _tris[ti + 3] = _tris[ti + 2] = vi + 1;
                _tris[ti + 4] = _tris[ti + 1] = vi + width * 2 + 1;
                _tris[ti + 5] = vi + width * 2 + 2;
            }
        }
        mesh.triangles = _tris;
        mesh.RecalculateNormals();
        //mesh.no
        mesh.RecalculateBounds();
    }

    private void generateGridTexture()
    {
        float value = (float) 1 / _texture.width;
        float modW;
        float modH;
        for(short height = 0; height < _texture.height; height++)
        {
            modH = abs(((float)height * 2) - _texture.height) * value;
            modH = pow(modH, 10);
            modH = 1 - modH;
            for (short width = 0; width < _texture.width; width++)
            {
                modW = abs(((float)width * 2) - _texture.width) * value;
                modW = pow(modW, 10);
                modW = 1 - modW;
                
                _texture.SetPixel(height, width, floatToColor(smoothstep(0.65f, 0.8f, 1 - (modH * modW))));
            }
        }
        _texture.Apply();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_MainTex", _texture);
        propertyBlock.SetFloat("_UseTexture", 1);
        meshRenderer.SetPropertyBlock(propertyBlock);
        //GetComponent<MeshRenderer>().material.SetFloat("_UseTexture", 1);
        //GetComponent<MeshRenderer>().material.mainTexture = texture;
    }

    private Color floatToColor(float value)
    {
        return new Color(value, value, value);
    }

    IEnumerator Pause()
    {
        while (_pauseScale > 0)
        {
            _pauseScale -= Time.deltaTime / _globalPausingSpeed;
            yield return null;
        }
        _pauseScale = 0;
        _isPaused = true;
    }

    IEnumerator Play()
    {
        _isPaused = false;
        while (_pauseScale < 1)
        {
            _pauseScale += Time.deltaTime / _globalPausingSpeed;
            yield return null;
        }
        _pauseScale = 1;
    }

    public void PlayPause()
    {
        StopCoroutine(Pause());
        StopCoroutine(Play());
        if (sceneStatus.IsPaused())
            StartCoroutine(Pause());
        else
            StartCoroutine(Play());
    }
}