using UnityEngine;

public abstract class MusicMateBehavior : MonoBehaviour
{
    private AnimationManager _animations;
    private IMusicMateManager _musicMateManager;
    private IAudioPlayerService _playerService;
    private IMusicMateApiService _apiService;


    /// <summary>
    /// Access the AnimationManager instance.
    /// </summary>
    protected AnimationManager Animations
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
    protected virtual void OnEnable() => RegisterEventHandlers();
    protected virtual void OnDisable() => UnregisterEventHandlers();
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
}
