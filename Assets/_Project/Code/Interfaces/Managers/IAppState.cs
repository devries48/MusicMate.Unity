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

    void InvokeStateChanged(MusicMateStateDetails details);
    void InvokeStateChanged(MusicMateStateChange change, bool value);
    void SubscribeToMusicMateStateChanged(MusicMateStateChangedHandler handler);
    void UnsubscribeFromMusicMateStateChangedd(MusicMateStateChangedHandler handler);
}

#region EventHandlers & EventArgs
public delegate void MusicMateStateChangedHandler(MusicMateState state);
public delegate void MusicMateModeChangedHandler(MusicMateMode mode);

public class MusicMateState
{
    public MusicMateState(MusicMateStateDetails details)
    {
        Change = MusicMateStateChange.Details;
        Details = details;
    }

    public MusicMateState(MusicMateStateChange change, bool value)
    {
        Change = change;

        switch (change)
        {
            case MusicMateStateChange.Details:
                ShowDetails = value;    
                break;
            case MusicMateStateChange.Providers:
                ShowProviders = value;
                break;
            default:
                break;
        }
    }

    public MusicMateStateChange Change { get; }
    public MusicMateStateDetails Details { get; }
    public bool ShowDetails { get; }
    public bool ShowProviders { get; }
}
#endregion
