using DG.Tweening;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AppState : IAppState
{
    readonly AppSetings _config;
    VisiblePart _parentPart, _currentPart;
    MusicMateMode _currentMode = MusicMateMode.Collection;
    IAnimationManager _animations;

    event VisiblePartChangedEventHandler VisiblePartChanged;
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

    IAnimationManager Animations
    {
        get
        {
            if (_animations == null)
                _animations = AnimationManager.Instance;

            return _animations;
        }
    }


    public IColorSettings CurrentColors { get; private set; }

    public void NotifyModeChanged(MusicMateMode newMode) => CurrentMode = newMode;

    #region Change UI element states (disabled/enabled)
    public void ChangeState(Button button, bool enabled, bool? isPlaying)
    {
        button.interactable = enabled;
        if (isPlaying.HasValue)
        {
            var image = button.GetComponent<Image>();
            image.sprite = isPlaying.Value ? _config.PauseSprite : _config.PlaySprite;
        }
    }

    public void ChangeState(ButtonAnimator button, bool enabled, bool? isPlaying)
    {
        button.SetInteractable(enabled);

        if (isPlaying.HasValue)
        {
            var image = button.GetComponent<Image>();
            image.sprite = isPlaying.Value ? _config.PauseSprite : _config.PlaySprite;
        }
    }

    public void ChangeState(Image image, bool enabled, bool? isPlaying = null)
    {
        float target;
        if (!enabled)
            target = .01f;
        else if (isPlaying.HasValue && !isPlaying.Value)
            target = .2f;
        else
            target = 1f;

        image.DOFade(target, .25f).SetEase(Ease.InSine);
    }

    public void ChangeState(TextMeshProUGUI text, bool enabled) => text.color =
        enabled ? _config.Colors.TextColor : _config.Colors.BackgroundColor;

    public void ChangeStates(Button[] buttons, bool enabled, bool? isPlaying = null)
    {
        foreach (var item in buttons)
            ChangeState(item, enabled, isPlaying);
    }

    public void ChangeStates(ButtonAnimator[] buttons, bool enabled, bool? isPlaying = null)
    {
        foreach (var item in buttons)
            ChangeState(item, enabled, isPlaying);
    }

    public void ChangeStates(TextMeshProUGUI[] texts, bool enabled)
    {
        foreach (var item in texts)
            ChangeState(item, enabled);
    }

    public void ChangeStates(Slider[] sliders, bool enabled)
    {
        foreach (var slider in sliders)
        {
            slider.gameObject.SetActive(enabled);
            slider.interactable = enabled;

            var handle = slider.transform.Find("Handle Slide Area/Handle").GetComponent<Image>();
            var fill = slider.transform.Find("Fill Area/Fill").GetComponent<Image>();
            //var background = slider.transform.Find("Background").gameObject;

            handle.color = enabled ? _config.Colors.AccentColor : _config.Colors.DefaultColor;
            fill.color = enabled ? _config.Colors.AccentColor : _config.Colors.DefaultColor;
            //background.SetActive(enabled);
        }
    }

    public void ChangeStates(Image[] images, bool enabled, bool isPlaying)
    {
        foreach (var item in images)
            ChangeState(item, enabled, isPlaying);
    }
    #endregion

    public void ChangeColor(Image image, Color32 toColor, bool animate)
    {
        if (animate)
            Animations.Panel.PlayImageColor(image, toColor);
        else
            image.color = toColor;
    }

    public void ChangeColor(TextMeshProUGUI text, Color32 toColor, bool animate)
    {
        if (animate)
            Animations.Panel.PlayTextColor(text, toColor);
        else
            text.color = toColor;
    }


    /// <summary>
    /// Notify subscribed controllers the visibility of a particular part has changed.
    /// </summary>
    public void ChangeVisiblePart(VisiblePart part) => VisiblePartChanged?.Invoke(this, new VisiblePartChangedEventArgs(part));

    public void SubscribeToVisiblePartChanged(VisiblePartChangedEventHandler handler) => VisiblePartChanged += handler;

    public void UnsubscribeFromVisiblePartChanged(VisiblePartChangedEventHandler handler) => VisiblePartChanged -= handler;

}
