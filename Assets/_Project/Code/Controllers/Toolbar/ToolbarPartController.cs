using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ToolbarPartController : ToolbarControllerBase
{
    [SerializeField] ButtonAnimator _header;

    [Header("Search Part")]
    [SerializeField] GameObject _searchPart;
    [SerializeField] string _titleSearch;

    [Header("Details Release Part")]
    [SerializeField] GameObject _releasePart;
    [SerializeField] string _titleRelease;
    [SerializeField] ToolbarButtonAnimator _releaseToggle;
    [SerializeField] ToolbarButtonAnimator _artistToggle;
    [SerializeField] ToolbarButtonAnimator _catalogToggle;

    internal RectTransform m_rectTransform;
    internal GameObject m_activePart;

    DetailsToggle _toggled;
    Part _currentPart;

    enum Part { search, details };
    enum DetailsToggle { release, artist, catalog }

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
        _currentPart = Part.search;
    }

    protected override void RegisterEventHandlers()
    {
        base.RegisterEventHandlers();

        Manager.AppState.SubscribeToMusicMateStateChanged(OnMusicMateStateChanged);

        _releaseToggle.OnButtonClick.AddListener(OnReleaseToggleClicked);
        _artistToggle.OnButtonClick.AddListener(OnArtistToggleClicked);
        _catalogToggle.OnButtonClick.AddListener(OnCatalogToggleClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        base.UnregisterEventHandlers();

        Manager.AppState.UnsubscribeFromMusicMateStateChangedd(OnMusicMateStateChanged);

        _releaseToggle.OnButtonClick.RemoveListener(OnReleaseToggleClicked);
        _artistToggle.OnButtonClick.RemoveListener(OnArtistToggleClicked);
        _catalogToggle.OnButtonClick.RemoveListener(OnCatalogToggleClicked);
    }
    #endregion

    #region ToolbarController Base Class Methods
    protected override void SetElementStates()
    {
        _releaseToggle.SetToggle(_toggled == DetailsToggle.release);
        _artistToggle.SetToggle(_toggled == DetailsToggle.artist);
        _catalogToggle.SetToggle(_toggled == DetailsToggle.catalog);
    }
    #endregion

    public void SetHeader(string title) => _header.SetHeader(title);

    void OnReleaseToggleClicked()
    {
        _toggled = DetailsToggle.release;
        SetElementStates();

        Manager.AppState.InvokeStateChanged(MusicMateStateDetails.Release);
    }

    void OnArtistToggleClicked()
    {
        _toggled = DetailsToggle.artist;
        SetElementStates();

        Manager.AppState.InvokeStateChanged(MusicMateStateDetails.Artist);
    }
    
    void OnCatalogToggleClicked()
    {
        _toggled = DetailsToggle.catalog;
        SetElementStates();

        Manager.AppState.InvokeStateChanged(MusicMateStateDetails.Catalog);
    }

    void OnMusicMateStateChanged(MusicMateState state)
    {
        if (state.Change != MusicMateStateChange.Details)
           return;

        var part = state.ShowDetails ? Part.details : Part.search;

        if (part == _currentPart)
            return;

        _currentPart = part;

        string title = default;
        GameObject hidePart = m_activePart;
        GameObject showPart = default;

        if (part == Part.search)
        {
            title = _titleSearch;
            showPart = _searchPart;
        }
        else if (part == Part.details)
        {
            title = _titleRelease;
            showPart = _releasePart;
        }

        if (title != default)
            Animations.Toolbar.PlayPartRotate(this, title, showPart, hidePart);
    }
}