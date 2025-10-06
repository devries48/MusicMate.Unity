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

    [Header("Import Part")]
    [SerializeField] GameObject _importPart;
    [SerializeField] string _titleImport;

    internal RectTransform m_rectTransform;
    internal GameObject m_activePart;

    Part _currentPart;

    enum Part { search, details, import };

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
    }

    protected override void UnregisterEventHandlers()
    {
        base.UnregisterEventHandlers();

        Manager.AppState.UnsubscribeFromMusicMateStateChanged(OnMusicMateStateChanged);
    }

    protected override void MusicMateModeChanged(MusicMateMode mode)
    {
        ActivatePart(mode == MusicMateMode.Import ? Part.import : _currentPart);
    }
    #endregion

    public void SetHeader(string title) => _header.SetHeader(title);
    
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