using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ToolbarPartController : ToolbarControllerBase
{
    [SerializeField] TextMeshProUGUI _titleText;

    [Header("Search Part")]
    [SerializeField] GameObject _searchPart;
    [SerializeField] string _titleSearch;

    [Header("Release Part")]
    [SerializeField] GameObject _releasePart;
    [SerializeField] string _titleRelease;
    [SerializeField] ToolbarButtonController _releaseToggle;
    [SerializeField] ToolbarButtonController _artistToggle;

    DetailsToggle _toggled;
    RectTransform _rectTransform;
    GameObject _activePart;
    VisiblePart _currentPart;

    enum DetailsToggle { release, artist }

    void OnEnable() => m_Manager.AppState.SubscribeToVisiblePartChanged(OnVisiblePartChanged);

    void OnDisable() => m_Manager.AppState.UnsubscribeFromVisiblePartChanged(OnVisiblePartChanged);

    protected override void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _searchPart.SetActive(true);
        _releasePart.SetActive(false);
        _activePart = _searchPart;
        _currentPart = VisiblePart.ReleaseResult;
        _toggled = DetailsToggle.release;

        base.Start();
    }

    protected override void InitElements()
    {
        _releaseToggle.OnButtonClick.AddListener(() => OnReleaseToggleClicked());
        _artistToggle.OnButtonClick.AddListener(() => OnArtistToggleClicked());
    }

    protected override IEnumerator SetElementStates()
    {
        _releaseToggle.SetToggle(_toggled == DetailsToggle.release);
        _artistToggle.SetToggle(_toggled == DetailsToggle.artist);

        yield return null;
    }

    void OnReleaseToggleClicked()
    {
        _toggled = DetailsToggle.release;
        ChangeElementStates();

        m_Manager.AppState.ChangeVisiblePart(VisiblePart.ReleaseDetails);
    }

    void OnArtistToggleClicked()
    {
        _toggled = DetailsToggle.artist;
        ChangeElementStates();

        m_Manager.AppState.ChangeVisiblePart(VisiblePart.ArtistDetails);
    }

    void OnVisiblePartChanged(object sender, VisiblePartChangedEventArgs e)
    {
        if (e.Part != VisiblePart.ReleaseResult && e.Part != VisiblePart.ReleaseDetails || e.Part == _currentPart)
            return;

        _currentPart = e.Part;

        string title = default;
        GameObject hidePart = _activePart;
        GameObject showPart = default;

        if (e.Part == VisiblePart.ReleaseResult)
        {
            title = _titleSearch;
            showPart = _searchPart;
        }
        else if (e.Part == VisiblePart.ReleaseDetails)
        {
            title = _titleRelease;
            showPart = _releasePart;
        }

        if (title != default)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(
                _rectTransform.DORotate(new Vector3(90, 0, 0), .25f)
                    .SetEase(Ease.Linear)
                    .OnComplete(
                        () =>
                        {
                            hidePart.SetActive(false);
                            showPart.SetActive(true);
                            _titleText.text = title;
                        }));
            sequence.Append(
                _rectTransform.DORotate(new Vector3(0, 0, 0), .25f)
                .SetDelay(.1f)
                .SetEase(Ease.Linear)
                    .OnComplete(
                        () =>
                        {
                            _activePart = showPart;
                        }));
        }
    }
}