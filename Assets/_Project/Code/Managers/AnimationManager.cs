using System;
using TMPro;
using UnityEngine;

public class AnimationManager : SceneSingleton<AnimationManager>
{
    [Header("Animations")]
    [SerializeField] LogoAnimations _logoAnimations;
    [SerializeField] PanelAndWindowsAnimations _panelAndWindowAnimations;
    [SerializeField] ButtonAnimations _buttonAnimations;
    [SerializeField] InputAnimations _inputAnimations;

    [Header("Tooltip Settings")]
    [SerializeField] float _tooltipPadding = 10f;
    [SerializeField] float _tooltipPanelWidth = 150f;

    IMusicMateManager _manager;

    public float TooltipPadding { get => _tooltipPadding; }
    public float TooltipPanelWidth { get => _tooltipPanelWidth; }

    public float ImageButtonScale => _buttonAnimations.ImageButtonScale;
    public float ImageButtonLargeScale => _buttonAnimations.ImageButtonLargeScale;

    void Awake() => _manager = MusicMateManager.Instance;

    public void LogoHide(GameObject logo, Action onComplete = null) => _logoAnimations.PlayLogoFade(logo, () =>
        {
            logo.SetActive(false);
            onComplete?.Invoke();
        });

    public void InputTextNormal(TMP_InputField input)
    {
        _inputAnimations.SetColor(input, _manager.TextColor);
        _inputAnimations.SetBackgroundColor(input, _manager.BackgroundColor);
    }

    public void InputTextSelect(TMP_InputField input) => _inputAnimations.SetColor(input, _manager.AccentColor);

    public void InputTextHighlight(TMP_InputField input)
    {
        _inputAnimations.SetColor(input, _manager.AccentColor);
        _inputAnimations.SetBackgroundColor(input, _manager.DefaultColor);
    }

    public void ButtonHoverEnter(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        if (button.interactable)
            _buttonAnimations.PlayHover(button, buttonType);
    }

    public void ButtonHoverExit(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        // if (button.interactable)
        _buttonAnimations.PlayNormal(button, buttonType);
    }

    public void ButtonClicked(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        _buttonAnimations.PlayClicked(button, buttonType);
    }

    public void ButtonInteractableChanged(ButtonInteractable button, bool isInteractable, bool isPrimary, ButtonAnimationType buttonType)
    {
        Color32 backgroundColor;
        Color32 foregroundColor;

        if (isInteractable)
        {
            backgroundColor = isPrimary ? _manager.AccentColor : _manager.DefaultColor;
            foregroundColor = isPrimary ? _manager.AccentTextColor : _manager.TextColor;
        }
        else
        {
            backgroundColor = _manager.DefaultColor;
            foregroundColor = _manager.AccentTextColor;
        }

        _buttonAnimations.PlayInteractable(button, backgroundColor, foregroundColor, buttonType);
    }


    public void ToolbarButtonSpinner(ToolbarButtonAnimator button, bool isVisible)
    {
        if (isVisible)
            _buttonAnimations.PlayToolbarShowSpinner(button);
        else
            _buttonAnimations.PlayToolbarHideSpinner(button);
    }

    public void ToolbarButtonToggle(ToolbarButtonAnimator button, bool isOn)
    {
        if (isOn)
            _buttonAnimations.PlayToolbarToggleOn(button);
        else
            _buttonAnimations.PlayToolbarToggleOff(button);
    }

    public void ToolbarButtonTooltip(ToolbarButtonAnimator button, bool isVisible)
    {
        if (isVisible)
            _buttonAnimations.PlayToolbarShowTooltip(button);
        else
            _buttonAnimations.PlayToolbarHideTooltip(button);
    }

    public void PanelVisible(bool isVisible, float duration, float delay = 0, params CanvasGroup[] panels) => _panelAndWindowAnimations.PlayPanelFade(isVisible, duration, delay, panels);

    public void PanelAudioPlayerState(bool isExpanded, RectTransform expandedPlayer, RectTransform collapsedPlayer, Action onComplete = null)
    {
        if (isExpanded)
            _panelAndWindowAnimations.PlayExpandAudioPlayer(expandedPlayer, collapsedPlayer, onComplete);
        else
            _panelAndWindowAnimations.PlayCollapseAudioPlayer(expandedPlayer, collapsedPlayer, onComplete);
    }

    public void PanelReleaseResultVisible(ReleaseResultController releaseResultController, bool show)
    {

    }

    public void WindowLoginVisible(GameObject loginWindow, bool show, float delay = 0f)
    {
        if (show)
            _panelAndWindowAnimations.PlayShowLoginWindow(loginWindow, delay);
        else
            _panelAndWindowAnimations.PlayHideLoginWindow(loginWindow);
    }

    public void WindowErrorVisible(GameObject errorWindow, bool show)
    {
        if (show)
            _panelAndWindowAnimations.PlayShowErrorWindow(errorWindow);
        else
            _panelAndWindowAnimations.PlayHideErrorWindow(errorWindow);
    }


}