using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtworkUpdate : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image _image;
    [SerializeField]
    private TMPro.TextMeshProUGUI _trackName;
    [SerializeField]
    private TMPro.TextMeshProUGUI _artistName;

    private void OnEnable()
    {
        PlayerEvents.TrackUpdate += UpdateArtwork;
    }

    private void OnDisable()
    {
        PlayerEvents.TrackUpdate -= UpdateArtwork;
    }

    private void UpdateArtwork(AudioTrack newTrack)
    {
        _image.sprite = newTrack.TrackCover;
        _trackName.text = newTrack.TrackName;
        _artistName.text = newTrack.ArtistName;
    }
}
