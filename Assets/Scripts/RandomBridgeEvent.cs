using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBridgeEvent : MonoBehaviour
{
    public GameObject laserShots;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        float rnd = Random.Range(0f, 1f);
        if (rnd > 0.8f)
        {
            laserShots.SetActive(true);
        }
    }

    private void OnDisable()
    {
        laserShots.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
