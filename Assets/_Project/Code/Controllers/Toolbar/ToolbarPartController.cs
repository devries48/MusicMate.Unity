using System;
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
    MusicMateStatePart _currentPart;

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
        _currentPart = MusicMateStatePart.ReleaseResult;
    }

    protected override void RegisterEventHandlers()
    {
        base.RegisterEventHandlers();

        Manager.AppState.SubscribeToMusicMateStateChanged(OnMusicMateStateChanged);

        _releaseToggle.OnButtonClick.AddListener(OnReleaseToggleClicked);
        _artistToggle.OnButtonClick.AddListener(OnArtistToggleClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        base.UnregisterEventHandlers();

        Manager.AppState.UnsubscribeFromMusicMateStateChangedd(OnMusicMateStateChanged);

        _releaseToggle.OnButtonClick.RemoveListener(OnReleaseToggleClicked);
        _artistToggle.OnButtonClick.RemoveListener(OnArtistToggleClicked);

    }
    #endregion

    #region ToolbarController Base Class Methods
    protected override void SetElementStates()
    {
        _releaseToggle.SetToggle(_toggled == DetailsToggle.release);
        _artistToggle.SetToggle(_toggled == DetailsToggle.artist);
    }
    #endregion

    public void SetHeader(string title) => _header.SetHeader(title);

    void OnReleaseToggleClicked()
    {
        _toggled = DetailsToggle.release;
        SetElementStates();

        Manager.AppState.InvokeStateChanged(MusicMateStatePart.ReleaseDetails);
    }

    void OnArtistToggleClicked()
    {
        _toggled = DetailsToggle.artist;
        SetElementStates();

        Manager.AppState.InvokeStateChanged(MusicMateStatePart.ArtistDetails);
    }

    void OnMusicMateStateChanged(MusicMateState state)
    {
        if (state.Part != MusicMateStatePart.ReleaseResult && state.Part != MusicMateStatePart.ReleaseDetails || state.Part == _currentPart)
            return;

        _currentPart = state.Part;

        string title = default;
        GameObject hidePart = m_activePart;
        GameObject showPart = default;

        if (state.Part == MusicMateStatePart.ReleaseResult)
        {
            title = _titleSearch;
            showPart = _searchPart;

            // reset toolbar
            _toggled = DetailsToggle.release;
            SetElementStates();
        }
        else if (state.Part == MusicMateStatePart.ReleaseDetails)
        {
            title = _titleRelease;
            showPart = _releasePart;
            _toggled = DetailsToggle.release;
        }

        if (title != default)
            Animations.Toolbar.PlayPartRotate(this, title, showPart, hidePart);
    }
}