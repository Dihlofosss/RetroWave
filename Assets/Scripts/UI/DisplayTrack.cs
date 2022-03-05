using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTrack : MonoBehaviour
{
    [SerializeField]
    private SceneStatus sceneStatus;

    private CanvasGroup canvasGroup;
    private TMPro.TextMeshProUGUI textMesh;

    private short _currentTrack;
    private float _displayTime, _fadeTime, _countdown;
    private bool _isHidden;
    private string _trackName;
    // Start is called before the first frame update

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        textMesh = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        _currentTrack = sceneStatus.GetCurrentTrackID();
    }
    void Start()
    {
        _displayTime = sceneStatus.GetTrackDisplayTime();
        _fadeTime = 1f;
        _displayTime = 3f;
        textMesh.text = sceneStatus.GetCurrentTrackName();
        _isHidden = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(_countdown);
        if (_countdown > 0)
        {
            _countdown -= Time.deltaTime;
            if (_currentTrack != sceneStatus.GetCurrentTrackID())
            {
                _currentTrack = sceneStatus.GetCurrentTrackID();
                textMesh.text = sceneStatus.GetCurrentTrackName();
                _countdown = _displayTime;
                Debug.Log("new track");
                return;
            }
        }
            
        if (_currentTrack != sceneStatus.GetCurrentTrackID())
        {
            _countdown = _displayTime;
            _currentTrack = sceneStatus.GetCurrentTrackID();
            textMesh.text = sceneStatus.GetCurrentTrackName();
            StartCoroutine(SwitchTrackname());
        }    
    }

    IEnumerator SwitchTrackname()
    {
        if(_isHidden)
        {
            _countdown = _displayTime;
            StartCoroutine(TrackNameFade(_isHidden));
            Debug.Log("NrextTrack C");
        }
        yield return new WaitWhile(() => _countdown > 0);

        StartCoroutine(TrackNameFade(_isHidden));
    }

    IEnumerator TrackNameFade(bool isHidden)
    {
        float fade = _fadeTime;
        while(fade > 0)
        {
            fade -= Time.deltaTime / _fadeTime;
            canvasGroup.alpha = isHidden ? 1 - fade : fade;
            yield return null;
        }
        _isHidden = !_isHidden;
    }
}
