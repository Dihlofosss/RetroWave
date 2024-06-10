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

    private void OnEnable()
    {
        PlayerEvents.PlayPauseTrack += PlayPause;
    }

    private void OnDisable()
    {
        PlayerEvents.PlayPauseTrack -= PlayPause;
    }

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
        
        BuildTris();
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
        if (_isPaused)
            return;

        _counter += Time.deltaTime * _pauseScale * _speed;
        
        if(_counter >= 1)
        {
            _counter = frac(_counter);
            UpdateGrid(_counter);
            //_gridMesh.RecalculateBounds();
        }
        MoveGrid();
    }

    private void UpdateGrid(float shift)
    {
        Vector3[] grid = _gridMesh.vertices;
        Color[] colors = _gridMesh.colors;
        for (int i = grid.Length - 1; i > (2 * _width); i--)
        {
            grid[i].Set(grid[i - (2 * _width + 1)].x, grid[i - (2 * _width + 1)].y, grid[i - (2 * _width + 1)].z);
            colors[i] = colors[i - (2 * _width + 1)];
        }
        
        AudioListener.GetSpectrumData(_sound, 0, FFTWindow.Rectangular);
        float height;
        
        for (int i = -_width; i <= _width; i++)
        {
            height = Mathf.Abs(i) - _width / 4;
            height = 1 - Mathf.Pow(1 - _sound[Mathf.Abs(Mathf.Abs((int)height) - (_width / 2))], 4);
            height *= 4f;
            //generate random grid height before playback
            if (_sceneStatus.IsPaused())
            {
                height = (Mathf.Abs(i) / ((float)_width));
                height = (1f - Mathf.Abs((height - 0.5f) * 2f)) * Random.Range(0f, 4f);
            }

            if (i > -2 && i < 2)
                height = 0f;

            grid[i + _width].Set(i, height, shift - _length);
            colors[i + _width] = Color.Lerp(_colorPalette.getDefaultGridColor(), _colorPalette.getPeakGridColor(), height / 4f);
        }
        _gridMesh.vertices = grid;
        _gridMesh.colors = colors;
    }

    private void MoveGrid()
    {
        Vector3[] grid = _gridMesh.vertices;
        for (int i = 0; i < grid.Length; i++)
        {
            grid[i].Set(grid[i].x, grid[i].y, grid[i].z + Time.deltaTime * _pauseScale * _speed);
        }
        _gridMesh.vertices = grid;
    }

    private void BuildTris()
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

    //draw tile texture
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
        StartCoroutine(_sceneStatus.IsPaused() ? Pause() : Play());
    }

    public short GetWidth()
    {
        return _width;
    }
}