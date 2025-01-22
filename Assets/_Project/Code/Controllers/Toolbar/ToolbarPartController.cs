using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ToolbarPartController : ToolbarControllerBase
{
    [SerializeField] ButtonAnimator _header;

    [Header("Search Part")]
    [SerializeField] GameObject _searchPart;
    [SerializeField] string _titleSearch;

    [Header("Release Part")]
    [SerializeField] GameObject _releasePart;
    [SerializeField] string _titleRelease;
    [SerializeField] ToolbarButtonAnimator _releaseToggle;
    [SerializeField] ToolbarButtonAnimator _artistToggle;

    internal RectTransform m_rectTransform;
    internal GameObject m_activePart;

    DetailsToggle _toggled;
    VisiblePart _currentPart;

    enum DetailsToggle { release, artist }

    #region MusicMate Base Class Methods
    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        m_rectTransform = GetComponent<RectTransform>();
    }

    protected override void InitializeValues()
    {
        base.InitializeValues();

        _searchPart.SetActive(true);
        _releasePart.SetActive(false);
        m_activePart = _searchPart;
        _currentPart = VisiblePart.ReleaseResult;
    }

    protected override void RegisterEventHandlers()
    {
        base.RegisterEventHandlers();

        Manager.AppState.SubscribeToVisiblePartChanged(OnVisiblePartChanged);

        _releaseToggle.OnButtonClick.AddListener(OnReleaseToggleClicked);
        _artistToggle.OnButtonClick.AddListener(OnArtistToggleClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        base.UnregisterEventHandlers();

        Manager.AppState.UnsubscribeFromVisiblePartChanged(OnVisiblePartChanged);

        _releaseToggle.OnButtonClick.RemoveListener(OnReleaseToggleClicked);
        _artistToggle.OnButtonClick.RemoveListener(OnArtistToggleClicked);
    }
    #endregion

    #region ToolbarController Base Class Methods
    protected override IEnumerator SetElementStates()
    {
        _releaseToggle.SetToggle(_toggled == DetailsToggle.release);
        _artistToggle.SetToggle(_toggled == DetailsToggle.artist);

        yield return null;
    }
    #endregion

    public void SetHeader(string title) => _header.SetHeader(title);

    void OnReleaseToggleClicked()
    {
        _toggled = DetailsToggle.release;
        ChangeElementStates();

        Manager.AppState.ChangeVisiblePart(VisiblePart.ReleaseDetails);
    }

    void OnArtistToggleClicked()
    {
        _toggled = DetailsToggle.artist;
        ChangeElementStates();

        Manager.AppState.ChangeVisiblePart(VisiblePart.ArtistDetails);
    }

    void OnVisiblePartChanged(object sender, VisiblePartChangedEventArgs e)
    {
        if (e.Part != VisiblePart.ReleaseResult && e.Part != VisiblePart.ReleaseDetails || e.Part == _currentPart)
            return;

        _currentPart = e.Part;

        string title = default;
        GameObject hidePart = m_activePart;
        GameObject showPart = default;

        if (e.Part == VisiblePart.ReleaseResult)
        {
            title = _titleSearch;
            showPart = _searchPart;

            // reset toolbar
            _toggled = DetailsToggle.release;
            ChangeElementStates();
        }
        else if (e.Part == VisiblePart.ReleaseDetails)
        {
            title = _titleRelease;
            showPart = _releasePart;
            _toggled = DetailsToggle.release;
        }

        if (title != default)
            Animations.Toolbar.PlayPartRotate(this, title, showPart, hidePart);
    }
}