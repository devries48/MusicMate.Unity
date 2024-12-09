using System;
using TMPro;
using UnityEngine;

public class AnimationManager : SceneSingleton<AnimationManager>
{
    [Header("Animations")]
    [SerializeField] LogoAnimations _logoAnimations;
    [SerializeField] ButtonAnimations _buttonAnimations;
    [SerializeField] InputAnimations _inputAnimations;

    IMusicMateManager _musicMateManager;

    void Awake() => _musicMateManager = MusicMateManager.Instance;

    public void HideLogo(GameObject logo, Action onComplete = null) => _logoAnimations.PlayLogoFade(logo, () =>
        {
            logo.SetActive(false);
            onComplete?.Invoke();
        });

    public void InputTextNormal(TMP_InputField input)
    {
        _inputAnimations.SetColor(input, _musicMateManager.TextColor);
        _inputAnimations.SetBackgroundColor(input, _musicMateManager.BackgroundColor);
    }

    public void InputTextSelect(TMP_InputField input) => _inputAnimations.SetColor(input, _musicMateManager.AccentColor);

    public void InputTextHighlight(TMP_InputField input)
    {
        _inputAnimations.SetColor(input, _musicMateManager.AccentColor);
        _inputAnimations.SetBackgroundColor(input, _musicMateManager.DefaultColor);
    }

    public void ButtonHoverEnter(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        Debug.Log("Hover enter");
        _buttonAnimations.PlayHover(button, buttonType);
    }
    public void ButtonHoverExit(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        Debug.Log("Hover exit");

        _buttonAnimations.PlayNormal(button, buttonType);
    }
    public void ButtonClicked(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        Debug.Log("Clicked");

        _buttonAnimations.PlayClicked(button, buttonType);
    }
    public void ButtonInteractableChanged(ButtonInteractable button, bool isInteractable, bool isPrimary, ButtonAnimationType buttonType)
    {
        Debug.Log("Interactable: " + isInteractable);
        Color32 backgroundColor;
        Color32 foregroundColor;

        if (isInteractable)
        {
            backgroundColor = isPrimary ? _musicMateManager.AccentColor : _musicMateManager.DefaultColor;
            foregroundColor = isPrimary ? _musicMateManager.AccentTextColor : _musicMateManager.TextColor;
        }
        else
        {
            backgroundColor = _musicMateManager.DefaultColor;
            foregroundColor = _musicMateManager.AccentTextColor;
        }

        _buttonAnimations.PlayButtonInteractable(button, backgroundColor, foregroundColor, buttonType);
    }
}