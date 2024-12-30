using System;
using UnityEngine;

public class LogoAnimator : MusicMateBehavior
{
    [Header("Elements")]
    [SerializeField] GameObject _logo;

    public bool IsLogoActive() => _logo.activeInHierarchy;

    public void HideLogo(Action onComplete = null) => Animations.LogoHide(_logo, () => onComplete?.Invoke());
}
