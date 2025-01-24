public enum MusicMateMode
{
    Collection,
    Edit
}

public enum MusicMateColor
{
    Accent,
    Default,
    Text,
    Panel,
    Background,
    AccentText,
    Icon,
    DisabledIcon
}

public enum VisiblePart
{
    Previous,
    ReleaseResult,
    ReleaseDetails,
    ArtistDetails
}

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

public enum ButtonAnimationType
{
    TextButton,
    DefaultImageButton,
    LargeImageButton,
    StateImageButton,
    ExpandCollapseButton
}

public enum ThumbnailSize { Tiny, Small, Medium, Large };

public enum ActionPanelType
{
    Release,
    Track,
}

public enum ActionPanelButton
{
    Show,
    Play,
    Pause,
    AddToPlaylist
}

