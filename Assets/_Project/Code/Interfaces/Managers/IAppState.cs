using TMPro;
using UnityEngine.UI;

public interface IAppState
{
    event MusicMateModeChangedHandler ModeChanged;

    IColorSettings CurrentColors { get; }
    MusicMateMode CurrentMode { get; }
    bool ProvidersVisible { get; }

    void NotifyModeChanged(MusicMateMode newMode);
    void ChangePlayButtonState(ButtonAnimator button, bool enabled, bool? isPlaying);
    void ChangePlayButtonsState(ButtonAnimator[] buttons, bool enabled, bool? isPlaying = null);

    void InvokeStateChanged(MusicMateStatePart part);
    void InvokeStateChanged(bool showProviders);
    void SubscribeToMusicMateStateChanged(MusicMateStateChangedHandler handler);
    void UnsubscribeFromMusicMateStateChangedd(MusicMateStateChangedHandler handler);
}

#region EventHandlers & EventArgs
public delegate void MusicMateStateChangedHandler(MusicMateState state);
public delegate void MusicMateModeChangedHandler(MusicMateMode mode);

public class MusicMateState
{
    public MusicMateState(MusicMateStatePart part)
    {
        Change = MusicMateStateChange.Part;
        Part = part;
    }

    public MusicMateState(bool showProviders)
    {
        Change = MusicMateStateChange.Providers;
        ShowProviders = showProviders;
    }

    public MusicMateStateChange Change { get; }
    public MusicMateStatePart Part { get; }
    public bool ShowProviders { get; }
}
#endregion
