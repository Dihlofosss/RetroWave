using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTrack : MonoBehaviour
{
    [SerializeField]
    private SceneStatus sceneStatus;

    private float _displayTime;
    // Start is called before the first frame update
    void Start()
    {
        _displayTime = sceneStatus.GetTrackDisplayTime();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
