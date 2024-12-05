using System;
using UnityEngine;

public class AnimationManager : SceneSingleton<AnimationManager>
{
    [Header("Animations")]
    [SerializeField] LogoAnimations _logoAnimations;

    public void PlayHideLogo(GameObject logo, Action onComplete = null) => _logoAnimations.PlayLogoFade(logo, () =>
        {
            logo.SetActive(false);
            onComplete?.Invoke();
        });
}