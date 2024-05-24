
using UnityEngine;

public class RandomBridgeEvent : MonoBehaviour
{
    public GameObject laserShots;

    private void OnEnable()
    {
        if (Random.Range(0f, 1f) > 0.8f)
        {
            laserShots.SetActive(true);
        }
    }

    private void OnDisable()
    {
        laserShots.SetActive(false);
    }
}
