using System.Collections;
using UnityEngine;

public abstract class MusicMateBehavior : MonoBehaviour
{
    AnimationManager _animations;
    IMusicMateManager _musicMateManager;
    IAudioPlayerService _playerService;
    IMusicMateApiService _apiService;

    MusicMateMode? _currentMode = null;
    protected bool IsInitioalizing { get; private set; }

    /// <summary>
    /// Access the AnimationManager instance.
    /// </summary>
    protected IAnimationManager Animations
    {
        get
        {
            if (_animations == null)
                _animations = AnimationManager.Instance;

            return _animations;
        }
    }

    /// <summary>
    /// Access the MusicMateManager instance.
    /// </summary>
    protected IMusicMateManager Manager
    {
        get
        {
            _musicMateManager ??= MusicMateManager.Instance;

            return _musicMateManager;
        }
    }

    /// <summary>
    /// Access the Audio Player Service.
    /// </summary>
    protected IAudioPlayerService PlayerService
    {
        get
        {
            _playerService ??= AudioPlayerService.Instance;

            return _playerService;
        }
    }

    /// <summary>
    /// Access the MusicMate API service.
    /// </summary>
    protected IMusicMateApiService ApiService
    {
        get
        {
            _apiService ??= MusicMateApiService.Instance;

            return _apiService;
        }
    }

    protected virtual void OnEnable()
    {
        if (Manager?.AppState != null)
        {
            Manager.AppState.ModeChanged += OnMusicMateModeChanged;

            if (_currentMode != Manager.AppState.CurrentMode)
            {
                IsInitioalizing = true;
                ApplyColors();
                IsInitioalizing=false;
            }
        }
        RegisterEventHandlers();
    }

    protected virtual void OnDisable()
    {
        if (Manager?.AppState != null)
            Manager.AppState.ModeChanged -= OnMusicMateModeChanged;

        UnregisterEventHandlers();
        StopAllCoroutines();
    }

    protected virtual void Awake() => InitializeComponents();
    protected virtual void Start() => InitializeValues();

    /// <summary>
    /// Called during Awake to initialize required components or references.
    /// Override this in derived classes for additional behavior.
    /// </summary>
    protected virtual void InitializeComponents() { }

    /// <summary>
    /// Called during Start to set up initial values or states.
    /// Override this in derived classes for additional behavior.
    /// </summary>
    protected virtual void InitializeValues() { }

    /// <summary>
    /// Registers event handlers for this component.
    /// </summary>
    /// <remarks>
    /// This method is called automatically when the GameObject is enabled (`OnEnable`).
    /// Override this method in derived classes to subscribe to events specific to the component.
    /// Always ensure that event sources are not null before subscribing to avoid null reference exceptions.
    /// </remarks>
    /// <example>
    /// Example usage in a derived class:
    /// <code>
    /// protected override void RegisterEventHandlers()
    /// {
    ///     myEventSource.SomeEvent += OnSomeEvent;
    /// }
    /// </code>
    /// </example>
    protected virtual void RegisterEventHandlers() { }

    /// <summary>
    /// Unregisters event handlers for this component.
    /// </summary>
    /// <remarks>
    /// This method is called automatically when the GameObject is disabled (`OnDisable`).
    /// Override this method in derived classes to unsubscribe from events specific to the component.
    /// Always ensure that event sources are not null before unsubscribing to avoid errors or unintended behavior.
    /// </remarks>
    /// <example>
    /// Example usage in a derived class:
    /// <code>
    /// protected override void UnregisterEventHandlers()
    /// {
    ///     myEventSource.SomeEvent -= OnSomeEvent;
    /// }
    /// </code>
    /// </example>
    protected virtual void UnregisterEventHandlers() { }

     void OnMusicMateModeChanged(MusicMateMode mode) 
    {
        if (_currentMode != mode)
        {
            _currentMode = Manager.AppState.CurrentMode;

            MusicMateModeChanged();
            ApplyColors();
        }
    }

    /// <summary>
    /// Invoked when the application's mode changes. 
    /// Override this method in derived classes to implement behavior specific to the newly activated mode.
    /// </summary>
    /// <remarks>
    /// Use this method to define custom logic or state changes that should occur when the mode switches, 
    /// such as enabling/disabling certain UI elements or modifying functionality specific to a mode.
    /// </remarks>
    /// <example>
    /// Example implementation in a derived class:
    /// <code>
    /// protected override void MusicMateModeChanged()
    /// {
    ///     if (Manager.AppState.CurrentMode == MusicMateMode.Edit)
    ///         EnableEditModeFeatures();
    ///     else
    ///         DisableEditModeFeatures();
    /// }
    /// </code>
    /// </example>
    protected virtual void MusicMateModeChanged() { }

    /// <summary>
    /// Applies color settings to the component based on the current theme or mode.
    /// Override this method in derived classes to customize how colors are applied to UI elements within the component.
    /// </summary>
    /// <remarks>
    /// This method is called automatically when the component is enabled or when the application's mode changes.
    /// It provides a centralized way to update the component's visual appearance to reflect the current theme or mode.
    /// </remarks>
    /// <example>
    /// Example implementation in a derived class:
    /// <code>
    /// protected override void ApplyColors()
    /// {
    ///     _headerText.color = Manager.AppColors.TextColor;
    ///     _backgroundPanel.color = Manager.AppColors.BackgroundColor;
    /// }
    /// </code>
    /// </example>
    protected virtual void ApplyColors() { }
}
