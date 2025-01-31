using UnityEngine.UI;

public class AppState : IAppState
{
    readonly AppSetings _config;
    MusicMateMode _currentMode = MusicMateMode.Collection;

    event MusicMateStateChangedHandler StateChanged;
    public event MusicMateModeChangedHandler ModeChanged;

    public AppState(MusicMateManager manager)
    {
        _config = manager.AppConfiguration;

        CurrentColors = _config.Colors;
    }

    public MusicMateMode CurrentMode
    {
        get => _currentMode;
        private set
        {
            if (_currentMode == value) return;

            _currentMode = value;

            if (value == MusicMateMode.Edit)
                CurrentColors = _config.ColorsEditMode;
            else
                CurrentColors = _config.Colors;

            ModeChanged?.Invoke(_currentMode);
        }
    }

    public IColorSettings CurrentColors { get; private set; }

    public bool ProvidersVisible { get; private set; }

    public void NotifyModeChanged(MusicMateMode newMode) => CurrentMode = newMode;

    public void ChangePlayButtonState(ButtonAnimator button, bool enabled, bool? isPlaying)
    {
        button.SetInteractable(enabled);

        if (isPlaying.HasValue)
        {
            var image = button.GetComponent<Image>();
            image.sprite = isPlaying.Value ? _config.PauseSprite : _config.PlaySprite;
        }
    }

    public void ChangePlayButtonsState(ButtonAnimator[] buttons, bool enabled, bool? isPlaying = null)
    {
        foreach (var item in buttons)
            ChangePlayButtonState(item, enabled, isPlaying);
    }
 
    /// <summary>
    /// Notify subscribed controllers the visibility of a particular part has changed.
    /// </summary>
    public void InvokeStateChanged(MusicMateStatePart part) => StateChanged?.Invoke(new MusicMateState(part));

    /// <summary>
    /// Notify subscribed controllers the visibility of the providers-panel has changed.
    /// </summary>
    public void InvokeStateChanged(bool showProviders) => StateChanged?.Invoke(new MusicMateState(showProviders));

    public void SubscribeToMusicMateStateChanged(MusicMateStateChangedHandler handler) => StateChanged += handler;

    public void UnsubscribeFromMusicMateStateChangedd(MusicMateStateChangedHandler handler) => StateChanged -= handler;
}
