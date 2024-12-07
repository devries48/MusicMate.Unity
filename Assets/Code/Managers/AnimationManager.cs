using System;
using TMPro;
using UnityEngine;

public class AnimationManager : SceneSingleton<AnimationManager>
{
    [Header("Animations")]
    [SerializeField] LogoAnimations _logoAnimations;
    [SerializeField] InputAnimations _inputAnimations;

    IMusicMateManager _musicMateManager;

    void Awake() => _musicMateManager = MusicMateManager.Instance;

    public void PlayHideLogo(GameObject logo, Action onComplete = null) => _logoAnimations.PlayLogoFade(logo, () =>
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
        _inputAnimations.SetBackgroundColor(input, _musicMateManager.DisabledColor);
    }
}