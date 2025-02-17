using UnityEngine;

public class MainWindowAnimator : MusicMateBehavior
{
    [Header("Panel Controllers")]
    [SerializeField] AudioPlayerController _audioPlayer;
    [SerializeField] GridReleaseController _releaseResult;
    [SerializeField] DetailsAnimator _showDetails;
    [SerializeField] ProvidersController _providers;

    [Header("Toolbar Controllers")]
    [SerializeField] ToolbarApplicationController _applicationToolbar;
    [SerializeField] ToolbarModeController _modeToolbar;
    [SerializeField] ToolbarPartController _searchToolbar;
    [SerializeField] ToolbarImportController _importToolbar;

    State _state;
    float _fadeTime = 0;

    #region Base Class Methods
    protected override void InitializeComponents() => _state = new State();

    protected override void InitializeValues() => ActivatePanels(false);

    protected override void RegisterEventHandlers()
    {
        PlayerService.SubscribeToExpandedChanged(OnAudioPlayerExpandedChanged);
        Manager.AppState.SubscribeToMusicMateStateChanged(OnMusicMateStateChanged);
    }

    protected override void UnregisterEventHandlers()
    {
        PlayerService.UnsubscribeFromExpandedChanged(OnAudioPlayerExpandedChanged);
        Manager.AppState.UnsubscribeFromMusicMateStateChangedd(OnMusicMateStateChanged);
    }

    protected override void MusicMateModeChanged(MusicMateMode mode)
    {
        if (mode == MusicMateMode.Collection && _state.ProvidersVisible)
            SetStateProviders(false);
    }
    #endregion

    public void ConnectionChanged(bool connected)
    {
        if (connected && _fadeTime == 0)
        {
            _fadeTime = 1f;
            ActivatePanels(true);
        }

        Animations.Panel.PlayPanelVisibility(connected, _fadeTime, 0f,
            _audioPlayer.m_canvasGroupExpanded,
            _applicationToolbar.m_CanvasGroup,
            _modeToolbar.m_CanvasGroup,
            _searchToolbar.m_CanvasGroup,
            _importToolbar.m_CanvasGroup);

        // Delay the display of the result for a better experience.
        Animations.Panel.PlayPanelVisibility(connected, _fadeTime, 1.5f, _releaseResult.m_canvasGroup);

        if (connected)
            ApiService.GetInitialReleases(GetInitialReleasesCallback);
    }

    public void ShowRelease(ReleaseResult release)
    {
        if (!_showDetails.isActiveAndEnabled)
        {
            Animations.Panel.PlayDetailsVisibility(true, _showDetails);
            Manager.AppState.InvokeStateChanged(MusicMateStateChange.Details, true);
        }

        _showDetails.SetRelease(release);
    }

    void ActivatePanels(bool activate)
    {
        _audioPlayer.gameObject.SetActive(activate);
        _releaseResult.gameObject.SetActive(activate);
        _applicationToolbar.gameObject.SetActive(activate);
        _modeToolbar.gameObject.SetActive(activate);
        _searchToolbar.gameObject.SetActive(activate);
        _importToolbar.gameObject.SetActive(activate);
    }

    /// <summary>
    /// Initial result on startup.
    /// </summary>
    void GetInitialReleasesCallback(PagedResult<ReleaseResult> result)
    {
        _releaseResult.SetResult(result);
        Manager.HideSpinner();
    }

    /// <summary>
    /// Change the right margin of the result when the audio players expanded/collapsed state changes.
    /// <remmarks>
    /// - Do not collapse when the providers panel is visible.
    /// - Hide providers panel when the audio player is expanded.
    /// </remmarks>
    /// </summary>
    void OnAudioPlayerExpandedChanged(object sender, ExpandedChangedEventArgs e)
    {
        if (!_state.ProvidersVisible)
            _releaseResult.SetRightMargin(e.IsExpanded);
    }

    /// <summary>
    /// Show or hide the result when a visible part has changed.
    /// </summary>
    void OnMusicMateStateChanged(MusicMateState state)
    {
        switch (state.Change)
        {
            case MusicMateStateChange.Providers:
                SetStateProviders(state.ShowProviders);
                break;
            case MusicMateStateChange.Details:
                SetStatePart(state);
                break;
        }
    }

    void SetStatePart(MusicMateState state)
    {
        if (state.ShowDetails)
        {
            switch (state.Details)
            {
                case MusicMateStateDetails.Release:
                case MusicMateStateDetails.Artist:
                case MusicMateStateDetails.Catalog:
                    if (_state.ResultVisible)
                        VisiblityResult(false);

                    _showDetails.Show(state.Details);
                    break;

                default:
                    break;
            }
        }
        else
        {
            VisiblityResult(true);
        }
    }

    void SetStateProviders(bool showProviders)
    {
        _state.ProvidersVisible = showProviders;

        if (showProviders)
        {
            // Save the state of the Audio Player
            _state.AudioPlayerExpanded = _audioPlayer.IsPlayerExpanded;

            if (_audioPlayer.IsPlayerExpanded)
                _audioPlayer.CollapsePlayer();
            else
                _releaseResult.SetRightMargin(true);

            Animations.Panel.PlayProvidersVisibility(true, _providers, true);
        }
        else
        {
            Animations.Panel.PlayProvidersVisibility(false, _providers);

            if (_state.AudioPlayerExpanded)
                _audioPlayer.ExpandPlayer(true);
            else
                _releaseResult.SetRightMargin(false, .5f);
        }
    }

    void VisiblityResult(bool show)
    {
        if (_state.ResultVisible == show)
            return;

        Animations.Panel.PlayReleaseGridVisiblity(show, _releaseResult);
        _state.ResultVisible = show;
    }

    class State
    {
        public State()
        {
            AudioPlayerExpanded = true;
            ResultVisible = true;
        }

        public bool ResultVisible { get; set; }
        public bool AudioPlayerExpanded { get; set; }
        public bool ProvidersVisible { get; set; }
    }
}
