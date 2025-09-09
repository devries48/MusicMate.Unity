using System;
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

    [Header("Import Part")]
    [SerializeField] GameObject _importPart;
    [SerializeField] string _titleImport;

    internal RectTransform m_rectTransform;
    internal GameObject m_activePart;

    DetailsToggle _toggled;
    Part _currentPart;

    enum Part { search, details, import };
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

    protected override void MusicMateModeChanged(MusicMateMode mode)
    {
        ActivatePart(mode == MusicMateMode.Import ? Part.import : _currentPart);
    }
    #endregion

    #region ToolbarController Base Class Methods
    protected override void SetElementStates()
    {
        _releaseToggle.SetToggleState(_toggled == DetailsToggle.release);
        _artistToggle.SetToggleState(_toggled == DetailsToggle.artist);
        _catalogToggle.SetToggleState(_toggled == DetailsToggle.catalog);
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
        ActivatePart(part);
    }

    void ActivatePart(Part part)
    {
        string title;
        var hidePart = m_activePart;
        GameObject showPart;

        switch (part)
        {
            case Part.search:
                title = _titleSearch;
                showPart = _searchPart;
                break;
            case Part.details:
                title = _titleRelease;
                showPart = _releasePart;
                break;
            case Part.import:
                title = _titleImport;
                showPart = _importPart;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(part), part, null);
        }

        if (title != null)
            Animations.Toolbar.PlayPartRotate(this, title, showPart, hidePart);
    }
}