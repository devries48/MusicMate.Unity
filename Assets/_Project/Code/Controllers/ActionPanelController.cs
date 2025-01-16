using System.Collections;
using UnityEngine;

public class ActionPanelController : MusicMateBehavior
{
    [SerializeField] ButtonAnimator _playPauseButton;

    TrackResult _trackModel;

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
    }

    void OnPlayOrPauseClicked()
    {
        if (!PlayerService.IsPlaying)
            PlayerService.Play(_trackModel);
        else
            PlayerService.Pause();
    }

    void OnPlayerStateChanged(object sender, StateChangedEventArgs e) => StartCoroutine(SetPlayerState());

    IEnumerator SetPlayerState()
    {
        Manager.AppState.ChangeState(_playPauseButton, PlayerService.IsActive, PlayerService.IsPlaying);

        yield return null;
    }

}
