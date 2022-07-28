using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _positionDelta;
    private Vector3 _rotationDelta;

    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    public float amplitude;
    // Start is called before the first frame update
    void Start()
    {
        //transform.rot
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Shake()
    {
        yield return null;
    }

    private void LateUpdate()
    {
        
    }
}
