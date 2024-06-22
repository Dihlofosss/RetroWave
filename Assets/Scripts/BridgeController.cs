using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [SerializeField]
    private OfflinePlaylist _sfx;
    [SerializeField]
    private ColorPalette _colorPalette;
    [SerializeField]
    private SceneStatus _sceneStatus;
    [SerializeField]
    private Material _bridgeMaterial, _carsMaterial;
    [SerializeField]
    private Mesh _bridgeBlock, _pillar, _lamps;

    [SerializeField]
    private Mesh _policeCar;
    [SerializeField]
    private Mesh[] _ambientCars;
    [SerializeField]
    private short _bridgeBlockSize;
    [SerializeField]
    private Cubemap reflectionProbe;
    [SerializeField]
    private GameObject _laserShotsParticles;

    [HideInInspector]
    public GameObject LaserShots
    {
        get;
        set;
    }
    [HideInInspector]
    public GameObject PolicePresuit
    {
        get;
        set;
    }
    [HideInInspector]
    public GameObject RoadTrafic
    {
        get;
        set;
    }

    private GameObject _bridgeTile;
    private GameObject[] _bridgesPool;

    private short _bridgeLength;
    private float _timer, _speed, _pauseScale, _pauseFade;
    private bool _pauseToggle, _isPaused = true;

    private Vector3 _carsSpawnPosition_01, _carsSpawnPosition_02;

    private void Awake()
    {
        _bridgesPool = new GameObject[8];
        //_bridgeLength = (short)(GetComponent<MeshGen>().GetWidth() / _bridgeBlockSize);
        _bridgeMaterial.SetColor("_ElementsColor", _colorPalette.getDefaultGridColor());
        _bridgeLength = GetComponent<MeshGen>().GetWidth();
        _speed = 8f;
        _pauseToggle = true;
        _pauseFade = _sceneStatus.GetPauseFade();
        _pauseScale = 1f;
        _carsSpawnPosition_01 = new(-_bridgeLength, 1f, 2.8f);
        _carsSpawnPosition_02 = new(_bridgeLength, -1f, 2.8f);
    }

    private void OnEnable()
    {
        PlayerEvents.AppStart += OnAppStart;
    }

    private void OnDisable()
    {
        PlayerEvents.AppStart -= OnAppStart;
    }

    private void OnAppStart()
    {
        _isPaused = false;
        _timer = 2.5f;
        ActivateNewObject(_bridgesPool);
        StartCoroutine(BringSpeedToNormal());
    }

    private void Start()
    {
        for (short i = 0; i < _bridgesPool.Length; i++)
        {
            _bridgesPool[i] = GenerateBridgeTile();
            _bridgesPool[i].transform.SetParent(transform);
            _bridgesPool[i].SetActive(false);
        }
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

        if (_timer <= 0)
        {
            ActivateNewObject(_bridgesPool);
            _timer = Random.Range(10f, 30f);
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
        MeshFilter filter = _bridgeTile.AddComponent<MeshFilter>();
        MeshRenderer renderer = _bridgeTile.AddComponent<MeshRenderer>();
        renderer.material = _bridgeMaterial;

        int xPosition;
        for (short i = 0; i < _bridgeLength / 2; i++)
        {
            xPosition = ((i - _bridgeLength / 4) * 4) + 2;
            //add flat bridge block
            GetGameObject(_bridgeBlock, _bridgeMaterial, xPosition).transform.SetParent(_bridgeTile.transform);
            //add pillar
            if (i % _bridgeBlockSize == 0)
            {
                GetGameObject(_pillar, _bridgeMaterial, xPosition).transform.SetParent(_bridgeTile.transform);
            }
            //add lamps
            if (i % 2 == 0)
            {
                GetGameObject(_lamps, _bridgeMaterial, xPosition).transform.SetParent(_bridgeTile.transform);
            }
        }
        
        MeshFilter[] bridgeTiles = _bridgeTile.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combines = new CombineInstance[bridgeTiles.Length];
        for (int i = 0; i < bridgeTiles.Length; i++)
        {
            combines[i].mesh = bridgeTiles[i].sharedMesh;
            combines[i].transform = bridgeTiles[i].gameObject.transform.localToWorldMatrix;
        }

        filter.mesh = new();
        filter.mesh.CombineMeshes(combines);


        //GameObject policeCar = GetGameObject(_policeCar, _carsMaterial, -_bridgeLength, 10f, 1f);
        //policeCar.transform.SetParent(_bridgeTile.transform);
        /*
        GameObject laserShotsParent = new()
        {
            name = "Laser Shots"
        };

        laserShotsParent.SetActive(false);
        laserShotsParent.transform.SetParent(_bridgeTile.transform);

        GameObject[] laserShots = new GameObject[2]
        {
            Instantiate(_laserShotsParticles, laserShotsParent.transform),
            Instantiate(_laserShotsParticles, laserShotsParent.transform)

        };
        laserShots[0].transform.SetPositionAndRotation(new Vector3(-_bridgeLength, 0, 3.2f), Quaternion.Euler(90,0,0));
        laserShots[1].transform.SetPositionAndRotation(new Vector3(_bridgeLength, 0, 3.2f), Quaternion.Euler(90, 180, 0));
        laserShots[1].GetComponent<ParticleSystem>().startColor = Color.red;
        laserShotsParent.SetActive(false);
        LaserShots = laserShotsParent;
        RandomBridgeEvent bEvent = _bridgeTile.AddComponent<RandomBridgeEvent>();
        bEvent.laserShots = laserShotsParent;
        */


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
            audio.clip = _sfx.SwitchToNextAudio();
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

        _bridgeTile.SetActive(false);


        return _bridgeTile;
    }

    private GameObject GetGameObject(Mesh mesh, Material material , float offset)
    {
        return GetGameObject(mesh, material, offset, 0f, 0f);
    }

    private GameObject GetGameObject(Mesh mesh, Material material, float offsetX, float offsetY, float offsetZ)
    {
        GameObject gameObject = new()
        {
            name = mesh.name
        };

        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
        gameObject.transform.position = new Vector3(offsetX, offsetY, offsetZ);
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

    private IEnumerator BringSpeedToNormal()
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