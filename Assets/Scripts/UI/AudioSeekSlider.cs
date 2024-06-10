using UnityEngine;
using UnityEngine.EventSystems;

public class AudioSeekSlider : UnityEngine.UI.Slider
{
    [SerializeField]
    private TMPro.TextMeshProUGUI _currentTime, _totalTime;
    [SerializeField]
    private SceneStatus _sceneStatus;
    private bool _isInterracted = false;

    private new void OnEnable()
    {
        PlayerEvents.PlaylistReady += Init;
        PlayerEvents.SelectTrack += OnTrackChange;
        base.OnEnable();
    }

    private new void OnDisable()
    {
        PlayerEvents.PlaylistReady -= Init;
        PlayerEvents.SelectTrack -= OnTrackChange;
        base.OnDisable();
    }

    private void Init()
    {
        OnTrackChange(_sceneStatus.CurrentTrack);
    }

    private void OnTrackChange(AudioTrack newTrack)
    {
        _currentTime.text = "00:00";
        _totalTime.text = LongToTime(newTrack.TrackDurationMS);
        base.value = 0;
    }

    private string LongToTime(long length)
    {
        System.TimeSpan t = System.TimeSpan.FromMilliseconds(length);
        return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
    }

    private new void Update()
    {
        if (!_isInterracted)
            base.value = _sceneStatus.GetPlaybackTime();

        if (!UI_Fader.IsUIActive)
            return;

        if (_sceneStatus.CurrentTrack == null)
            return;
        _currentTime.text = LongToTime((long)(_sceneStatus.CurrentTrack.TrackDurationMS * base.value));
        base.Update();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!UI_Fader.IsUIActive)
            return;
        _isInterracted = true;
        base.OnPointerDown(eventData);
    }

    public override void OnMove(AxisEventData eventData)
    {
        if (!UI_Fader.IsUIActive)
            return;
        _isInterracted = true;
        base.OnMove(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!UI_Fader.IsUIActive)
            return;
        _isInterracted = true;
        base.OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!UI_Fader.IsUIActive)
            return;
        PlayerEvents.OnAudioSeek(base.value);
        _isInterracted = false;
        base.OnPointerUp(eventData);
    }
}
