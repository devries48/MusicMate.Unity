using System.Collections;
using TMPro;
using UnityEngine;

public class ActionPanelController : MusicMateBehavior
{
    [SerializeField] ActionPanelType _type;
    [SerializeField] ButtonAnimator _playPauseButton;
    [SerializeField] ButtonAnimator _playlistButton;
    [SerializeField] ButtonAnimator _showReleaseButton;

    TrackResult _trackModel;
    ReleaseResult _releaseModel;

    public delegate void ActionClickHandler(ActionPanelButton buttonClicked);
    public event ActionClickHandler OnActionClicked;

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        PlayerService.SubscribeToStateChanged(OnPlayerStateChanged);

        _playPauseButton.OnButtonClick.AddListener(OnPlayOrPauseClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        PlayerService.UnsubscribeFromStateChanged(OnPlayerStateChanged);

        _playPauseButton.OnButtonClick.RemoveListener(OnPlayOrPauseClicked);
    }
    #endregion

    public void Initialize(RowTrackAnimator track)
    {
        _trackModel = track.m_track;
        InitializePlayState();
    }

    public void Initialize(CellReleaseAnimator release)
    {
        _releaseModel = release.m_release;
        InitializePlayState();
    }

    void InitializePlayState() => Manager.AppState.ChangeState(_playPauseButton, PlayerService.IsActive, PlayerService.IsPlaying);

    void OnPlayerStateChanged(object sender, StateChangedEventArgs e) => StartCoroutine(SetPlayerState());

    void OnPlayOrPauseClicked()
    {
        if (!PlayerService.IsPlaying)
        {
            if (_type == ActionPanelType.Release)
                PlayerService.Play(_releaseModel);
            else
                PlayerService.Play(_trackModel);

            OnActionClicked?.Invoke(ActionPanelButton.Play);
        }
        else
        {
            PlayerService.Pause();
            OnActionClicked?.Invoke(ActionPanelButton.Pause);
        }
    }

    IEnumerator SetPlayerState()
    {
        Manager.AppState.ChangeState(_playPauseButton, PlayerService.IsActive, PlayerService.IsPlaying);

        yield return null;
    }

}
