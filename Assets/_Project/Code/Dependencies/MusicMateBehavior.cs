using Interfaces.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class MusicMateBehavior : MonoBehaviour
{
    AnimationManager _animations;
    IMusicMateManager _musicMateManager;
    IAudioPlayerService _playerService;
    IMusicMateApiService _apiService;

    bool _initializing;

    /// <summary>
    /// Access the AnimationManager instance.
    /// </summary>
    protected IAnimationManager Animations
    {
        get
        {
            if (!_animations)
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

    /// <summary>
    /// Start a global hosted Coroutine.
    /// </summary>
    protected CoroutineRunner GlobalCoroutine => CoroutineRunner.Instance;

    protected virtual void OnEnable()
    {
        if (Manager?.AppState != null)
        {
            Manager.AppState.ModeChanged += OnMusicMateModeChanged;

            _initializing = true;
            ApplyColors();
            _initializing = false;
        }

        RegisterEventHandlers();
    }

    protected virtual void OnDisable()
    {
        if (Manager?.AppState != null)
            Manager.AppState.ModeChanged -= OnMusicMateModeChanged;

        UnregisterEventHandlers();
    }

    protected virtual void Awake() => InitializeComponents();

    protected virtual void Start() => InitializeValues();

    /// <summary>
    /// Called during Awake to initialize required components or references. Override this in derived classes for
    /// additional behavior.
    /// </summary>
    protected virtual void InitializeComponents()
    {
    }

    /// <summary>
    /// Called during Start to set up initial values or states. Override this in derived classes for additional
    /// behavior.
    /// </summary>
    protected virtual void InitializeValues()
    {
    }

    /// <summary>
    /// Registers event handlers for this component.
    /// </summary>
    /// <remarks>
    /// This method is called automatically when the GameObject is enabled (`OnEnable`). Override this method in derived
    /// classes to subscribe to events specific to the component. Always ensure that event sources are not null before
    /// subscribing to avoid null reference exceptions.
    /// </remarks>
    /// <example>
    /// Example usage in a derived class: <code>protected override void RegisterEventHandlers() {
    /// myEventSource.SomeEvent += OnSomeEvent; }</code>
    /// </example>
    protected virtual void RegisterEventHandlers()
    {
    }

    /// <summary>
    /// Unregisters event handlers for this component.
    /// </summary>
    /// <remarks>
    /// This method is called automatically when the GameObject is disabled (`OnDisable`). Override this method in
    /// derived classes to unsubscribe from events specific to the component. Always ensure that event sources are not
    /// null before unsubscribing to avoid errors or unintended behavior.
    /// </remarks>
    /// <example>
    /// Example usage in a derived class: <code>protected override void UnregisterEventHandlers() {
    /// myEventSource.SomeEvent -= OnSomeEvent; }</code>
    /// </example>
    protected virtual void UnregisterEventHandlers()
    {
    }

    void OnMusicMateModeChanged(MusicMateMode mode)
    {
        MusicMateModeChanged(mode);
        ApplyColors();
    }

    /// <summary>
    /// Invoked when the application's mode changes.  Override this method in derived classes to implement behavior
    /// specific to the newly activated mode.
    /// </summary>
    /// <remarks>
    /// Use this method to define custom logic or state changes that should occur when the mode switches,  such as
    /// enabling/disabling certain UI elements or modifying functionality specific to a mode.
    /// </remarks>
    /// <example>
    /// Example implementation in a derived class: <code>protected override void MusicMateModeChanged() { if
    /// (Manager.AppState.CurrentMode == MusicMateMode.Edit) EnableEditModeFeatures(); else DisableEditModeFeatures();
    /// }</code>
    /// </example>
    protected virtual void MusicMateModeChanged(MusicMateMode mode)
    {
    }

    /// <summary>
    /// Applies color settings to the component based on the current theme or mode. Override this method in derived
    /// classes to customize how colors are applied to UI elements within the component.
    /// </summary>
    /// <remarks>
    /// This method is called automatically when the component is enabled or when the application's mode changes. It
    /// provides a centralized way to update the component's visual appearance to reflect the current theme or mode.
    /// </remarks>
    /// <example>
    /// Example implementation in a derived class: <code>protected override void ApplyColors() { _headerText.color =
    /// Manager.AppColors.TextColor; _backgroundPanel.color = Manager.AppColors.BackgroundColor; }</code>
    /// </example>
    protected virtual void ApplyColors()
    {
    }

    protected void ChangeState<T>(bool enable, params T[] args) where T : class
    {
        foreach (var item in args)
        {
            if (item is ButtonAnimator)
            {
                (item as ButtonAnimator).SetInteractable(enable);
                continue;
            }
            else if (item is Slider)
                (item as Slider).interactable = enable;

            var animate = !_initializing && IsGameObjectActive(item);
            var targetColor = GetItemColor(item, enable);

            ChangeColor(item, targetColor, animate);
        }
    }

    protected void ChangeColor<T>(MusicMateColor color, params T[] args) where T : class
    {
        foreach (var item in args)
        {
            if (item == null)
                continue;

            var animate = !_initializing && IsGameObjectActive(item);
            var targetColor = ColorFromEnum(color);

            ChangeColor(item, targetColor, animate);
        }
    }

    bool IsGameObjectActive<T>(T item) where T : class
    {
        if (item is Component component)
            return component.gameObject.activeInHierarchy;
        return true; // Non-component items are always considered "active"
    }

    Color32 GetItemColor<T>(T target, bool enabled) where T : class
    {
        Color32 fromColor, toColor;
        var clr = enabled ? MusicMateColor.Text : MusicMateColor.DisabledText;

        switch (target)
        {
            case Image image:
                fromColor = image.color;
                break;

            case TextMeshProUGUI text:
                fromColor = text.color;
                break;

            case Marquee marq:
                fromColor = marq.TextColor;
                break;

            case Slider slider:
                var fillImage = slider.fillRect.GetComponent<Image>();
                fromColor = fillImage.color;
                clr = enabled ? MusicMateColor.Accent : MusicMateColor.DisabledText;
                break;

            default:
                Debug.LogWarning($"ChangeColor: Unsupported type {typeof(T)} for color adjustment.");
                return Color.red;
        }

        toColor = ColorFromEnum(clr);
        return EnsureAlphaConsistency(fromColor, toColor);
    }

    void ChangeColor<T>(T target, Color32 toColor, bool animate) where T : class
    {
        if (target == null)
            return;

        // Special handling for sliders, relay to separate method and exit
        if (target is Slider slider)
        {
            ChangeSliderColor(slider, toColor, animate);
            return;
        }

        switch (target)
        {
            case Image image:
                toColor = EnsureAlphaConsistency(image.color, toColor);
                break;
            case TextMeshProUGUI text:
                toColor = EnsureAlphaConsistency(text.color, toColor);
                break;
            case Marquee marq:
                toColor = EnsureAlphaConsistency(marq.TextColor, toColor);
                break;
            default:
                Debug.LogWarning($"ChangeColor: Unsupported type {typeof(T)} for color adjustment.");
                return;
        }

        if (animate)
        {
            switch (target)
            {
                case Image image:
                    Animations.Panel.PlayImageColor(image, toColor);
                    break;
                case TextMeshProUGUI text:
                    Animations.Panel.PlayTextColor(text, toColor);
                    break;
                case Marquee marq:
                    Animations.Panel.PlayMarqueeColor(marq, toColor);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (target)
            {
                case Image image:
                    image.color = toColor;
                    break;
                case TextMeshProUGUI text:
                    text.color = toColor;
                    break;
                case Marquee marq:
                    marq.TextColor = toColor;
                    break;
            }
        }
    }

    void ChangeSliderColor(Slider slider, Color32 toColor, bool animate)
    {
        var fillImage = slider.fillRect.GetComponent<Image>();
        var handleImage = slider.handleRect.GetComponent<Image>();

        if (!slider.transform.Find("Background").TryGetComponent<Image>(out var backgroundImage))
        {
            print("test");
        }

        toColor = EnsureAlphaConsistency(fillImage.color, toColor);

        if (animate)
        {
            Animations.Panel.PlayImageColor(fillImage, toColor);
            Animations.Panel.PlayImageColor(handleImage, toColor);
            Animations.Panel.PlayImageColor(backgroundImage, Manager.AppColors.TextColor);
        }
        else
        {
            fillImage.color = toColor;
            handleImage.color = toColor;
            backgroundImage.color = Manager.AppColors.TextColor;
        }
    }

    static Color32 EnsureAlphaConsistency(Color32 currentColor, Color32 targetColor)
    {
        targetColor.a = currentColor.a;

        return targetColor;
    }

    Color32 ColorFromEnum(MusicMateColor color)
    {
        return color switch
        {
            MusicMateColor.Accent => Manager.AppColors.AccentColor,
            MusicMateColor.Default => Manager.AppColors.DefaultColor,
            MusicMateColor.Text => Manager.AppColors.TextColor,
            MusicMateColor.DisabledText => Manager.AppColors.DisabledTextColor,
            MusicMateColor.Panel => Manager.AppColors.PanelColor,
            MusicMateColor.AccentText => Manager.AppColors.AccentTextColor,
            MusicMateColor.Icon => Manager.AppColors.IconColor,
            MusicMateColor.DisabledIcon => Manager.AppColors.DisabledIconColor,
            MusicMateColor.Background => Manager.AppColors.BackgroundColor,
            _ => throw new System.NotImplementedException(),
        };
    }
}