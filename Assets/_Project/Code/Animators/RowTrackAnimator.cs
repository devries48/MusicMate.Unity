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

    public bool IsActive { get { return _isActive; } set { _isActive = value; SetColors(); } }
    public bool IsSelected { get; set; } = false;

    #region Fields
    Image _backgroundImage;
    bool _isActive;
    bool _isHover;

    GridTrackController _parent;
    Coroutine _clickCoroutine;

    internal TrackResult m_track;
    #endregion

    protected override void InitializeComponents() => _backgroundImage = GetComponent<Image>();

    public void Initialize(TrackResult track, int pos, GridTrackController parent)
    {
        m_track = track;
        _parent = parent;

        _nrText.text = pos.ToString();
        _titleMarque.SetText(track.Title);
        _durationText.text = track.DurationString;
    }

    void SetColors()
    {
        var textcolor = _isHover ? Manager.AccentTextColor : IsActive ? Manager.AccentColor : Manager.TextColor;

        _nrText.color = textcolor;
        _titleMarque.SetColor(textcolor);
        _durationText.color = textcolor;
    }

    #region Pointer Event Handlers (Handles pointer hover and click events)

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHover = true;
        _backgroundImage.color = IsActive ? Manager.AccentColor : Manager.TextColor;
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
            if (_clickCoroutine != null)
                StopCoroutine(_clickCoroutine);

            HandleDoubleClick();
        }
        else
            _clickCoroutine = StartCoroutine(HandleSingleClick());
    }

    IEnumerator HandleSingleClick()
    {
        yield return new WaitForSeconds(Constants.DoubleClickThreshold);

        IsSelected = !IsSelected;

        Animations.Grid.PlayRowClick(this);
        _parent.ChangeSelection(this); 
    }

    void HandleDoubleClick()
    {
        _parent.ClearSelection();

        Animations.Grid.PlayRowClick(this);
        PlayerService.Play(m_track);
    }
    #endregion
}
