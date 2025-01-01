public enum PlayerState
{
    None,
    Failed,
    Ready,
    Playing,
    Paused
}

public enum PlayerAction
{
    Play,
    Pause,
    /// <summary>
    /// The playlist contains all the tracks of the release,
    /// The playlist index is set to zero. 
    /// The first track must be played.
    /// </summary>
    PlayRelease,
    PlaylistChanged,
    IndexChanged
}

public enum PlaylistAction { None, NewList, IndexChanged }

public enum VisiblePart
{
    Previous,
    ReleaseResult,
    ReleaseDetails,
    ArtistDetails
}

public enum ButtonAnimationType
{
    TextButton,
    DefaultImageButton,
    LargeImageButton,
    ToolbarButton
}