public enum MusicMateMode
{
    Collection,
    Edit,
    Import
}

public enum MusicMateColor
{
    Accent,
    Default,
    Text,
    DisabledText,
    Panel,
    AccentText,
    Icon,
    DisabledIcon,
    Background
}

public enum MusicMateStateChange
{
    Details,
    Providers
}

public enum MusicMateStateDetails
{
    Release,
    Artist,
    Catalog
}

public enum MusicMateZone
{
    ReleaseImage,
    ReleaseTitle,
    ReleaseYear,
    ReleaseGenre,
    ReleaseLabel,
    ReleaseTracks,
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

public enum ButtonType
{
    Text,
    DefaultImage,
    LargeImage,
    StateImage,
    ExpandCollapse
}

public enum ToolbarButtonType
{
    Default,
    Spinner,
    Toggle,
    ToggleText,
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

