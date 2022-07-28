using static Unity.Mathematics.math;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Mesh), typeof(MeshFilter))]
public class MeshGen : MonoBehaviour
{
    private Mesh _gridMesh, _tunnelMesh;

    private Texture2D _texture;
    private List<Vector3> _verts = new List<Vector3>();
    private List<Color> _colors = new List<Color>();
    private int[] _tris;
    private Vector2[] _uv;
    private float[] _sound = new float[64];
    private bool _pauseToggle = true;
    private bool _isPaused = false;
    private float _pauseScale = 1f;
    private float _pauseFade;
    private float _speed;

    //private GameObject[] _tunnel;

    [SerializeField]
    private ColorPalette _colorPalette;
    [SerializeField]
    private SceneStatus _sceneStatus;
    [SerializeField]
    private GameObject _tunnelBlock; 
    [SerializeField]
    private short _width, _length;
    private float _counter;

    private void Awake()
    {
        _gridMesh = new Mesh
        {
            name = "Procedural Grid"
        };
        GetComponent<MeshFilter>().mesh = _gridMesh;

        _pauseFade = _sceneStatus.GetPauseFade();
        _speed = _sceneStatus.GetSpeed();
        _tris = new int[4 * _width * _length * 6];
        _uv = new Vector2[(_width * 2 + 1) * (_length * 2 + 1)];
        _texture = new Texture2D(128, 128, TextureFormat.R8, true)
        {
            anisoLevel = 8
        };
    }

    void Start()
    {
        for (int i = 0, z = -_length; z <= _length; z++)
        {
            for(int x = -_width; x <= _width; x++, i++)
            {
                float ySpikeCoordinate = abs((float)x) / _width;
                ySpikeCoordinate -= 0.5f;
                ySpikeCoordinate *= 2f;
                ySpikeCoordinate = abs(ySpikeCoordinate);
                ySpikeCoordinate = 1f - ySpikeCoordinate;
                ySpikeCoordinate *= Random.Range(0f, 1f);
                ySpikeCoordinate = smoothstep(0f, 1f, ySpikeCoordinate);
                ySpikeCoordinate *= 5f;
                _verts.Add(new Vector3(x, ySpikeCoordinate * ((float)(z  + _length) / (_length)), z));
                _uv[i] = new Vector2(x, z);
                _colors.Add(_colorPalette.getDefaultGridColor());
            }
        }
        _gridMesh.vertices = _verts.ToArray();
        _gridMesh.uv = _uv;
        _gridMesh.colors = _colors.ToArray();
        
        RebuildTris();
        GenerateGridTexture();

        StartCoroutine(Play());
        /*
        _tunnel = new GameObject[_length * 2];
        Mesh mesh = _tunnelBlock.GetComponent<MeshFilter>().sharedMesh;
        _tunnelBlock.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = _texture;
        Color[] colors = new Color[mesh.colors.Length];
        for (short i = 0; i < mesh.colors.Length; i++)
        {
            colors[i] = _colorPalette.getDefaultGridColor();
        }
        //mesh.colors = colors;

        for (short i = 0; i < _tunnel.Length; i++)
        {
            _tunnel[i] = Instantiate(_tunnelBlock);
            _tunnel[i].transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + i - _length);
            _tunnel[i].transform.SetParent(transform);
        }*/
    }

    void Update()
    {
        if (_pauseToggle != _sceneStatus.IsPaused())
        {
            _pauseToggle = _sceneStatus.IsPaused();
            PlayPause();
        }

        if (_isPaused)
            return;

        _counter += Time.deltaTime * _pauseScale * _speed;
        
        if(_counter >= 1)
        {
            _counter = frac(_counter);
            RebuildVerts(_counter);
            _gridMesh.RecalculateBounds();
        }
        MoveGrid();
    }

    private void RebuildVerts(float shift)
    {
        float height;
        AudioListener.GetSpectrumData(_sound, 0, FFTWindow.Rectangular);
        //remove last poly row
        int initialSize = _verts.Count;
        for (int i = initialSize - 1; i >= initialSize - (2 * _width) - 1; i--)
        {
            _verts.RemoveAt(i);
            _colors.RemoveAt(i);
        }
        //add vert for 1st poly row
        for(int i = _width; i >= -_width; i--)
        {
            height = Mathf.Abs(i) - _width / 4;
            height = 1 - Mathf.Pow(1 - _sound[Mathf.Abs(Mathf.Abs((int)height) - (_width / 2))], 3);
            height *= 4f;
            if(_sceneStatus.IsPaused())
            {
                height = (Mathf.Abs(i) / ((float)_width));
                height = (1f - Mathf.Abs((height - 0.5f) * 2f)) * Random.Range(0f, 4f);
            }

            if (i > -2 && i < 2)
                height *= 0.05f;
            _verts.Insert(0, new Vector3(i, height , shift - _length));
            
            _colors.Insert(0, Color.Lerp(_colorPalette.getDefaultGridColor(), _colorPalette.getPeakGridColor(), height / 4f));
        }
        //_verts.TrimExcess();
        _gridMesh.vertices = _verts.ToArray();
        _gridMesh.colors = _colors.ToArray();
    }

    private void MoveGrid()
    {
        for(int i = 0; i < _verts.Count; i++)
        {
            _verts[i] = new Vector3(_verts[i].x, _verts[i].y, _verts[i].z + Time.deltaTime * _pauseScale * _speed);
        }
        _gridMesh.vertices = _verts.ToArray();
    }

    private void RebuildTris()
    {
        for (int ti = 0, vi = 0, y = 0; y < _length * 2; y++, vi++)
        {
            for (int x = 0; x < _width * 2; x++, ti += 6, vi++)
            {
                _tris[ti] = vi;
                _tris[ti + 3] = _tris[ti + 2] = vi + 1;
                _tris[ti + 4] = _tris[ti + 1] = vi + _width * 2 + 1;
                _tris[ti + 5] = vi + _width * 2 + 2;
            }
        }
        _gridMesh.triangles = _tris;
        _gridMesh.RecalculateNormals();
    }

    private void GenerateGridTexture()
    {
        float value = (float) 1 / _texture.width;
        float modW, modH;
        for(short height = 0; height < _texture.height; height++)
        {
            modH = abs(((float)height * 2) - _texture.height) * value;
            modH = pow(modH, 12);
            modH = 1 - modH;
            for (short width = 0; width < _texture.width; width++)
            {
                modW = abs(((float)width * 2) - _texture.width) * value;
                modW = pow(modW, 12);
                modW = 1 - modW;
                
                _texture.SetPixel(height, width, FloatToColor(smoothstep(0.65f, 0.95f, 1 - (modH * modW))));
            }
        }
        _texture.Apply();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_MainTex", _texture);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    private Color FloatToColor(float value)
    {
        return new Color(value, value, value);
    }

    IEnumerator Pause()
    {
        while (_pauseScale > 0)
        {
            _pauseScale -= Time.deltaTime / _pauseFade;
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
            _pauseScale += Time.deltaTime / _pauseFade;
            yield return null;
        }
        _pauseScale = 1;
    }

    public void PlayPause()
    {
        StopCoroutine(Pause());
        StopCoroutine(Play());
        if (_sceneStatus.IsPaused())
            StartCoroutine(Pause());
        else
            StartCoroutine(Play());
    }

    public short GetWidth()
    {
        return _width;
    }
}