using UnityEngine.UI;

public class AppState : IAppState
{
    readonly AppSetings _config;
    readonly MusicMateState _currentState;
    MusicMateMode _currentMode = MusicMateMode.Collection;
    event MusicMateStateChangedHandler StateChanged;

    public event MusicMateModeChangedHandler ModeChanged;

    public AppState(MusicMateManager manager)
    {
        _config = manager.AppConfiguration;
        _currentState = new MusicMateState();
        CurrentColors = _config.Colors;
    }

    public MusicMateMode CurrentMode
    {
        get => _currentMode;
        private set
        {
            if (_currentMode == value) return;

            _currentMode = value;

            CurrentColors = value switch
            {
                MusicMateMode.Edit => _config.ColorsEditMode,
                MusicMateMode.Import => _config.ColorsImportMode,
                _ => _config.Colors
            };

            ModeChanged?.Invoke(_currentMode);
        }
    }

    public IColorSettings CurrentColors { get; private set; }

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
    /// Notify subscribed controllers the visibility of a details part has changed.
    /// </summary>
    public void InvokeStateChanged(MusicMateStateDetails details)
    {
        _currentState.State(details);
        StateChanged?.Invoke(_currentState);
    }

    /// <summary>
    /// Notify subscribed controllers the visibility of the providers-panel has changed.
    /// </summary>
    public void InvokeStateChanged(MusicMateStateChange change, bool value)
    {
        _currentState.State(change,value);
        StateChanged?.Invoke(_currentState);
    }

    public void SubscribeToMusicMateStateChanged(MusicMateStateChangedHandler handler) => StateChanged += handler;

    public void UnsubscribeFromMusicMateStateChangedd(MusicMateStateChangedHandler handler) => StateChanged -= handler;

}
