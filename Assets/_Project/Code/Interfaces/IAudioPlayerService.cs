using System;
using System.Collections.Generic;

public interface IAudioPlayerService
{
    float PlayerWidth { get; set; }
    /// <summary>
    /// CurrentRelease and Track are available.
    /// </summary>
    bool IsActive { get; }
    bool IsPlaying { get; }
    bool IsPaused { get; }
    bool CanMoveBack { get; }
    bool CanMoveForward { get; }
    int CurrentIndex { get; }
    ReleaseResult CurrentRelease { get; }
    TrackResult CurrentTrack { get; }

    List<TrackResult> GetPlaylist();
    void Play(ReleaseResult release);
    void Play(TrackResult track);
    void Pause();
    void ChangeExpandedState(bool isExpanded);
    void ChangeState(PlayerState state);
    void SubscribeToExpandedChanged(ExpandedChangedEventHandler handler);
    void SubscribeToStateChanged(StateChangedEventHandler handler);
    void SubscribeToActionChanged(ActionChangedEventHandler handler);
    void UnsubscribeFromExpandedChanged(ExpandedChangedEventHandler handler);
    void UnsubscribeFromStateChanged(StateChangedEventHandler handler);
    void UnsubscribeFromActionChanged(ActionChangedEventHandler handler);
}

#region EventHandlers & EventArgs
public delegate void ExpandedChangedEventHandler(object sender, ExpandedChangedEventArgs e);
public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);
public delegate void ActionChangedEventHandler(object sender, ActionChangedEventArgs e);

public class ExpandedChangedEventArgs : EventArgs
{
    public ExpandedChangedEventArgs(bool isExpanded) => IsExpanded = isExpanded;

    public bool IsExpanded { get; }
}

public class StateChangedEventArgs : EventArgs
{
    public StateChangedEventArgs(PlayerState state) => State = state;

    public PlayerState State { get; }
}

public class ActionChangedEventArgs : EventArgs
{
    public ActionChangedEventArgs(PlayerAction action, PlaylistAction playlist)
    {
        Action = action;
        PlaylistAction = playlist;
    }

    public PlayerAction Action { get; }
    public PlaylistAction PlaylistAction { get; }
}
#endregion
