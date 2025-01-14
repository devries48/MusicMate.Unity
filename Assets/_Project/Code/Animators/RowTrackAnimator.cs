using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RowTrackAnimator : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI _nrText;
    [SerializeField] Marquee _titleMarque;
    [SerializeField] TextMeshProUGUI _durationText;

    public delegate void TrackClickHandler(TrackResult track, int position);
    public event TrackClickHandler OnTrackClicked;
    public event TrackClickHandler OnTrackDoubleClicked;

    Image _backgroundImage;
    bool _isSelected;
    bool _isHover;

    TrackResult _track;
    Coroutine _clickCoroutine;


    const float DoubleClickThreshold = 0.3f; // Time in seconds to detect a double-click

    public bool IsSelected { get { return _isSelected; } set { _isSelected = value; SetColors(); } }

    protected override void InitializeComponents() => _backgroundImage = GetComponent<Image>();

    public void Initialize(TrackResult track, int pos)
    {
        _track = track;

        _nrText.text = pos.ToString();
        _titleMarque.SetText(track.Title);
        _durationText.text = track.DurationString;
    }

    void SetColors()
    {
        var textcolor = _isHover ? Manager.AccentTextColor : IsSelected ? Manager.AccentColor : Manager.TextColor;

        _nrText.color = textcolor;
        _titleMarque.SetColor(textcolor);
        _durationText.color = textcolor;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHover = true;
        _backgroundImage.color = IsSelected ? Manager.AccentColor : Manager.TextColor;
        _backgroundImage.enabled = true;

        SetColors();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHover = false;
        _backgroundImage.enabled = false;

        SetColors();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            if (_clickCoroutine  != null)
                StopCoroutine(_clickCoroutine);

            HandleDoubleClick();
        }
        else
            _clickCoroutine = StartCoroutine(HandleSingleClick());
    }

    IEnumerator HandleSingleClick()
    {
        yield return new WaitForSeconds(DoubleClickThreshold);
        Debug.Log("SINGLE");
    }

    void HandleDoubleClick()
    {
        PlayerService.Play(_track);
    }
}
