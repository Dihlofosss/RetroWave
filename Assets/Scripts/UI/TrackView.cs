using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrackView : MonoBehaviour, IPointerClickHandler
{
    public AudioTrack Track { get; set; }
    public int TrackPosition { get; set; }
    private bool _isActiveTrack = false;
    public bool IsActiveTrack
    {
        get
        {
            return _isActiveTrack;
        }
        set
        {
            if(value != _isActiveTrack)
            {
                _isActiveTrack = value;
                StartCoroutine(SelectionFrameFade(value));
            }
        }
    }

    [SerializeField]
    private UnityEngine.UI.Image _selectionFrame;
    [SerializeField]
    private TMPro.TextMeshProUGUI _trackPositionTM;
    [SerializeField]
    private UnityEngine.UI.Image _artwork;
    [SerializeField]
    private TMPro.TextMeshProUGUI _trackNameTM;
    [SerializeField]
    private TMPro.TextMeshProUGUI _artistNameTM;
    [SerializeField]
    private TMPro.TextMeshProUGUI _playbackCountTM;
    [SerializeField]
    private TMPro.TextMeshProUGUI _durationTM;

    private void OnEnable()
    {
        _selectionFrame.gameObject.SetActive(false);
        _trackPositionTM.text = string.Format("{0:D2}", TrackPosition);
        _artwork.sprite = Track.TrackCover;
        _trackNameTM.text = Track.TrackName;
        _artistNameTM.text = Track.ArtistName;
        _playbackCountTM.text = Track.PlaybackCount.ToString();
        _durationTM.text = LongToTime(Track.TrackDurationMS);
        this.name = TrackPosition.ToString() + " - " + Track.ArtistName + " - " + Track.TrackName;
    }

    private string LongToTime(long timeMS)
    {
        System.TimeSpan t = System.TimeSpan.FromMilliseconds(timeMS);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!eventData.dragging)
            PlayerEvents.OnSelectTrack(Track);
    }

    IEnumerator SelectionFrameFade(bool fadeIn)
    {
        if (fadeIn)
            _selectionFrame.gameObject.SetActive(true);

        float scale = 4f;
        float duration = 1f;
        Color canvasColor;
        while(duration > 0)
        {
            duration -= Time.deltaTime * scale;
            canvasColor = _selectionFrame.color;
            canvasColor.a = fadeIn ? 1- duration : duration;
            _selectionFrame.color = canvasColor;
            yield return null;
        }

        if (!fadeIn)
            _selectionFrame.gameObject.SetActive(false);
    }
}
