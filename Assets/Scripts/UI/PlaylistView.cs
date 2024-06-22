using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaylistView : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerEvents.PlaylistReady += FillPlaylistView;
        PlayerEvents.SelectTrack += SwitchActiveTrack;
    }

    private void OnDisable()
    {
        PlayerEvents.PlaylistReady -= FillPlaylistView;
        PlayerEvents.SelectTrack -= SwitchActiveTrack;
    }

    [SerializeField]
    private OnlinePlayList _playlist;
    [SerializeField]
    private GameObject _viewport;
    [SerializeField]
    private GameObject _trackPrefab;

    private AudioTrack _selectedTrack;

    private void FillPlaylistView()
    {
        StartCoroutine(FillPlaylist());
    }

    private IEnumerator FillPlaylist()
    {
        _trackPrefab.SetActive(false);
        int trackPosition = 1;
        foreach (AudioTrack track in _playlist.tracks)
        {
            GameObject newTrack = Instantiate(_trackPrefab, _viewport.transform);
            TrackView tv = newTrack.GetComponent<TrackView>();
            tv.Track = track;
            track.TrackView = tv;
            tv.TrackPosition = trackPosition++;
            newTrack.SetActive(true);
            yield return null;
        }
        _selectedTrack = _playlist.GetCurrentTrack();
        _selectedTrack.TrackView.IsActiveTrack = true;
    }

    private void SwitchActiveTrack(AudioTrack newTrack)
    {
        if(_selectedTrack != null && !_selectedTrack.Equals(newTrack))
        {
            Debug.Log(_selectedTrack.Equals(newTrack));
            _selectedTrack.TrackView.IsActiveTrack = false;
        }
        _selectedTrack = newTrack;
        _selectedTrack.TrackView.IsActiveTrack = true;
    }
}
