using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaylistView : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerEvents.PlaylistReady += FillPlaylistWithTracks;
        PlayerEvents.SelectTrack += SwitchActiveTrack;
    }

    private void OnDisable()
    {
        PlayerEvents.PlaylistReady -= FillPlaylistWithTracks;
        PlayerEvents.SelectTrack -= SwitchActiveTrack;
    }

    [SerializeField]
    private OnlinePlayList _playlist;
    [SerializeField]
    private GameObject _viewport;
    [SerializeField]
    private GameObject _trackPrefab;

    //private List<TrackView> _tvList;
    private AudioTrack _selectedTrack;

    private void FillPlaylistWithTracks()
    {
        //_tvList = new();
        _trackPrefab.SetActive(false);
        int trackPosition = 1;
        foreach(AudioTrack track in _playlist.tracks)
        {
            GameObject newTrack = Instantiate(_trackPrefab, _viewport.transform);
            TrackView tv = newTrack.GetComponent<TrackView>();
            tv.Track = track;
            track.TrackView = tv;
            tv.TrackPosition = trackPosition++;
            //_tvList.Add(tv);
            newTrack.SetActive(true);
        }
        _selectedTrack = _playlist.GetCurrentTrack();
        _selectedTrack.TrackView.IsActiveTrack = true;
    }

    private void SwitchActiveTrack(AudioTrack newTrack)
    {
        Debug.Log("SwitchActiveTrack");
        if(_selectedTrack != null && !_selectedTrack.Equals(newTrack))
        {
            Debug.Log(_selectedTrack.Equals(newTrack));
            //_tvList[_playlist.tracks.IndexOf(_selectedTrack)].IsActiveTrack = false;
            _selectedTrack.TrackView.IsActiveTrack = false;
        }
        _selectedTrack = newTrack;
        //_tvList[_playlist.tracks.IndexOf(_selectedTrack)].IsActiveTrack = true;
        _selectedTrack.TrackView.IsActiveTrack = true;
    }
}
