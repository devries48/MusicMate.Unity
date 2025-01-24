using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IAppState
{
    event MusicMateModeChangedHandler ModeChanged;

    IColorSettings CurrentColors { get; }
    MusicMateMode CurrentMode { get; }

    void NotifyModeChanged(MusicMateMode newMode);
    void ChangeState(Button button, bool enabled, bool? isPlaying);
    void ChangeState(ButtonAnimator button, bool enabled, bool? isPlaying);
    void ChangeState(Image image, bool enabled, bool? isPlaying = null);
    void ChangeState(TextMeshProUGUI text, bool enabled);
    void ChangeStates(Button[] buttons, bool enabled, bool? isPlaying = null);
    void ChangeStates(ButtonAnimator[] buttons, bool enabled, bool? isPlaying = null);
    void ChangeStates(Image[] images, bool enabled, bool isPlaying);
    void ChangeStates(TextMeshProUGUI[] texts, bool enabled);
    void ChangeStates(Slider[] sliders, bool enabled);
    void ChangeVisiblePart(VisiblePart part);
    void SubscribeToVisiblePartChanged(VisiblePartChangedEventHandler handler);
    void UnsubscribeFromVisiblePartChanged(VisiblePartChangedEventHandler handler);
}

#region EventHandlers & EventArgs
public delegate void VisiblePartChangedEventHandler(object sender, VisiblePartChangedEventArgs e);
public delegate void MusicMateModeChangedHandler(MusicMateMode mode);

public class VisiblePartChangedEventArgs : EventArgs
{
    public VisiblePartChangedEventArgs(VisiblePart part)
    {
        Part = part;
    }

    public VisiblePart Part { get; }
}
#endregion
