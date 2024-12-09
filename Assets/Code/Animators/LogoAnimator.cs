using System;
using UnityEngine;

public class LogoAnimator : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] GameObject _logo;

    public bool IsLogoActive() => _logo.activeInHierarchy;

    public void HideLogo(Action onComplete = null)
    {
        AnimationManager.Instance.HideLogo(_logo, () => onComplete?.Invoke());
    }
}
