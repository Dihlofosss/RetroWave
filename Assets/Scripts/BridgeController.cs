using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [SerializeField]
    private PlayList _sfx;
    [SerializeField]
    private ColorPalette _colorPalette;
    [SerializeField]
    private SceneStatus _sceneStatus;
    [SerializeField]
    private Material _bridgeMaterial;
    [SerializeField]
    private Mesh _bridgeBlock, _pillar, _lamps;
    [SerializeField]
    private short _bridgeBlockSize;
    [SerializeField]
    private Cubemap reflectionProbe;

    private GameObject _bridgeTile;
    private GameObject[] _bridgesPool;

    private short _bridgeLength;
    private float _timer, _speed, _pauseScale, _pauseFade;
    private bool _pauseToggle, _isPaused;

    private void Awake()
    {
        _bridgesPool = new GameObject[8];
        //_bridgeLength = (short)(GetComponent<MeshGen>().GetWidth() / _bridgeBlockSize);
        _bridgeMaterial.SetColor("_ElementsColor", _colorPalette.getDefaultGridColor());
        _bridgeLength = GetComponent<MeshGen>().GetWidth();
        _speed = 8f;
        _pauseToggle = true;
        _isPaused = false;
        _pauseFade = _sceneStatus.GetPauseFade();
        _pauseScale = 1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (short i = 0; i < _bridgesPool.Length; i++)
        {
            _bridgesPool[i] = GenerateBridgeTile();
            _bridgesPool[i].transform.SetParent(transform);
            _bridgesPool[i].SetActive(false);
        }
        _timer = 2.5f;
        ActivateNewObject(_bridgesPool);
        StartCoroutine(UnfakeSpeed());
    }

    // Update is called once per frame
    void Update()
    {
        if (_pauseToggle != _sceneStatus.IsPaused())
        {
            _pauseToggle = _sceneStatus.IsPaused();
            PlayPause();
        }

        if (_isPaused)
            return;

        if (_timer <= 0)
        {
            ActivateNewObject(_bridgesPool);
            _timer = Random.Range(5f, 15f);
        }
        MoveObjects(_bridgesPool);
        _timer -= Time.deltaTime;
    }

    private GameObject GenerateBridgeTile()
    {
        _bridgeTile = new GameObject
        {
            name = "BridgeTile"
        };
        int xPosition;
        for(short i = 0; i < _bridgeLength / 2; i++)
        {
            xPosition = ((i - _bridgeLength / 4) * 4) + 2;
            if (i%3 == 0)
            {
                GetGameobject(_pillar, xPosition).transform.SetParent(_bridgeTile.transform);
            }
            GetGameobject(_bridgeBlock, xPosition).transform.SetParent(_bridgeTile.transform);
            if (i % 2 == 0)
            {
                GetGameobject(_lamps, xPosition).transform.SetParent(_bridgeTile.transform);
            }
        }

        _bridgeTile.transform.SetPositionAndRotation(new Vector3(0f, 0f, -10f), Quaternion.Euler(-90f, 0f, 0f));
        _bridgeTile.AddComponent<AudioSource>();
        AudioSource audio = _bridgeTile.GetComponent<AudioSource>();
        {
            audio.bypassEffects = true;
            audio.loop = true;
            audio.bypassListenerEffects = true;
            audio.mute = false;
            audio.playOnAwake = true;
            audio.minDistance = .5f;
            audio.maxDistance = 3f;
            audio.spatialBlend = 1f;
            audio.outputAudioMixerGroup = _sfx.GetMixer();
            audio.clip = _sfx.GetNext();
        }
        /*
        _bridgeTile.AddComponent<ReflectionProbe>();
        ReflectionProbe probe = _bridgeTile.GetComponent<ReflectionProbe>();
        {
            probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Custom;
            probe.customBakedTexture = reflectionProbe;
            probe.boxProjection = true;
            probe.hdr = true;
            probe.size = new Vector3(10f, 2f, 50f);
            probe.blendDistance = 2f;
            probe.intensity = 2f;
        }
        */
        return _bridgeTile;
    }

    private GameObject GetGameobject(Mesh mesh, float offset)
    {
        GameObject gameObject = new GameObject()
        {
            name = mesh.name
        };
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = _bridgeMaterial;
        gameObject.transform.position = new Vector3(offset, 0f, 0f);
        return gameObject;
    }

    private void ActivateNewObject(GameObject[] gameObjects)
    {
        for(short i = 0; i < gameObjects.Length; i++)
        {
            if (!gameObjects[i].activeSelf)
            {
                gameObjects[i].SetActive(true);
                gameObjects[i].transform.rotation = Quaternion.Euler(-90f, Random.Range(-30f, 30f), 0f);
                return;
            }
        }
    }

    private void MoveObjects(GameObject[] gameObjects)
    {
        for(short i = 0; i < gameObjects.Length; i++)
        {
            if(gameObjects[i].activeSelf)
            {
                gameObjects[i].transform.position = new Vector3(0f, 0f, gameObjects[i].transform.position.z + Time.deltaTime * _speed * _pauseScale);
                if (gameObjects[i].transform.position.z > 100f)
                {
                    gameObjects[i].SetActive(false);
                    gameObjects[i].transform.position = new Vector3(0f, 0f, -5f);
                }
            }
        }
    }

    private IEnumerator UnfakeSpeed()
    {
        yield return new WaitForSeconds(6f);
        _speed = _sceneStatus.GetSpeed();
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
}
