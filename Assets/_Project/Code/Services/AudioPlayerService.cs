using System.Collections.Generic;
using System.Linq;

public class AudioPlayerService : SceneSingleton<AudioPlayerService>, IAudioPlayerService
{
    event ExpandedChangedEventHandler ExpandedChanged;
    event StateChangedEventHandler StateChanged;
    event ActionChangedEventHandler ActionChanged;

    readonly List<TrackResult> _playlist = new();
    PlayerState _playerState = PlayerState.None;

    public float PlayerWidth { get; set; }
    public bool IsActive => _playerState != PlayerState.None && _playerState != PlayerState.Failed;
    public bool IsPlaying => _playerState == PlayerState.Playing;
    public bool IsPaused => _playerState == PlayerState.Paused;
    public bool CanMoveBack { get => IsActive && _playlist.Count > 0 && CurrentIndex > 0; }
    public bool CanMoveForward { get => IsActive && CurrentIndex < _playlist.Count - 1; }
    public int CurrentIndex { get; private set; } = -1;
    public ReleaseResult CurrentRelease { get; private set; }
    public TrackResult CurrentTrack { get; private set; }
    public List<TrackResult> GetPlaylist() => _playlist;
    public void ChangeExpandedState(bool isExpanded) => ExpandedChanged?.Invoke(this, new ExpandedChangedEventArgs(isExpanded));
    public void ChangeState(PlayerState state) => RaiseChangeStateEvent(state);

    public void Play(ReleaseResult release)
    {
        // If release is already selected, send a Play PlayerAction
        if (CurrentRelease == release)
        {
            RaiseChangeActionEvent(PlayerAction.Play);
            return;
        }

        CurrentIndex = 0;
        CurrentRelease = release;
        CurrentTrack = null;

        var allTracks = release.GetAllTracks();

        if (allTracks.Any())
        {
            CurrentTrack = allTracks.First();

            _playlist.Clear();
            _playlist.AddRange(allTracks);

            RaiseChangeActionEvent(PlayerAction.PlaylistChanged, PlaylistAction.NewList);
            RaiseChangeActionEvent(PlayerAction.PlayRelease);
        }
    }

    public void Play(TrackResult track)
    {
        CurrentIndex = 0;
        CurrentRelease = track.Release;
        CurrentTrack = track;

        _playlist.Clear();
        _playlist.AddRange(new List<TrackResult> { track });

        RaiseChangeActionEvent(PlayerAction.PlaylistChanged, PlaylistAction.NewList);
        RaiseChangeActionEvent(PlayerAction.PlayRelease);
    }

    public void Pause()
    {
        if (_playerState == PlayerState.Playing)
            RaiseChangeActionEvent(PlayerAction.Pause);
    }

    #region Event Subscribe & Unsubscribe
    public void SubscribeToExpandedChanged(ExpandedChangedEventHandler handler) => ExpandedChanged += handler;
    public void SubscribeToStateChanged(StateChangedEventHandler handler) => StateChanged += handler;
    public void SubscribeToActionChanged(ActionChangedEventHandler handler) => ActionChanged += handler;

    public void UnsubscribeFromExpandedChanged(ExpandedChangedEventHandler handler) => ExpandedChanged -= handler;
    public void UnsubscribeFromStateChanged(StateChangedEventHandler handler) => StateChanged -= handler;
    public void UnsubscribeFromActionChanged(ActionChangedEventHandler handler) => ActionChanged -= handler;
    #endregion

    void RaiseChangeStateEvent(PlayerState state)
    {
        _playerState = state;
        StateChanged?.Invoke(this, new StateChangedEventArgs(state));
    }
    void RaiseChangeActionEvent(PlayerAction action, PlaylistAction playlist = PlaylistAction.None) => ActionChanged?.Invoke(this, new ActionChangedEventArgs(action, playlist));
}


