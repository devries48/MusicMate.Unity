public interface IAppState
{
    event MusicMateModeChangedHandler ModeChanged;

    IColorSettings CurrentColors { get; }
    MusicMateMode CurrentMode { get; }

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
    public void State(MusicMateStateDetails details)
    {
        ShowDetails = true;
        Change = MusicMateStateChange.Details;
        Details = details;
    }

    public void State(MusicMateStateChange change, bool value)
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

    public MusicMateStateChange Change { get; private set; }
    public MusicMateStateDetails Details { get; private set; }
    public bool ShowDetails { get; private set; }
    public bool ShowProviders { get; private set; }
}
#endregion
