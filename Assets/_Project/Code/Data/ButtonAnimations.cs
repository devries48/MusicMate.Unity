using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Button Animation Settings", fileName = "Button Animations")]
public class ButtonAnimations : ScriptableObject
{
    [Header("General Settings")]
    [SerializeField] float _animationDuration = 0.2f;
    [SerializeField] int _maxAnimationCacheSize = 100;
    [SerializeField] Ease _animationEase = Ease.OutBack;

    [Header("Text Button Animations")]
    [SerializeField] float _textHoverScale = 1.1f;
    [SerializeField] float _textClickScale = 0.9f;

    [Header("Image Button Animations")]
    [SerializeField] float _imageHoverScale = 1.2f;
    [SerializeField] float _imageClickScale = 0.8f;

    [Header("Toolbar Button Animations")]
    [SerializeField] float _toolbarHoverScale = 1.2f;
    [SerializeField, Tooltip("Change the color to the accent color")] bool _toolbarHoverColorChange = true;
    [SerializeField] float _toolbarClickScale = 0.8f;
    [SerializeField] float _toolbarToggleScale = 0.7f;
    [SerializeField, Tooltip("Resize the icon when the spinner is active")] float _toolbarSpinnerScale = 0.7f;
    [SerializeField] float _toolbarTooltipPopupTime = 0.1f;

    readonly Dictionary<string, Sequence> _sequenceCache = new();

    void OnDisable() => ClearCache();

    #region Default Button Events
    public void PlayClicked(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        var scaleClick = buttonType switch
        {
            ButtonAnimationType.ImageButton => _imageClickScale,
            _ => _textClickScale
        };

        var scaleHover = buttonType switch
        {
            ButtonAnimationType.ImageButton => _imageHoverScale,
            _ => _textHoverScale
        };

        var key = GenerateCacheKey(button, buttonType, scaleClick, scaleHover);
        if (TryGetSequence(key, out Sequence cachedSequence))
        {
            Debug.Log($"PlayClicked Sequence started from cache (key: {key})");
            cachedSequence.Restart();
            return;
        }

        var seq = DOTween.Sequence()
            .Append(button.transform.DOScale(scaleClick, _animationDuration).SetEase(_animationEase))
            .Append(button.transform.DOScale(scaleHover, _animationDuration).SetEase(_animationEase))
            .Pause()
            .SetAutoKill(false);

        AddSequence(key, seq);
        seq.Restart();
    }

    public void PlayHover(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        var scale = buttonType switch
        {
            ButtonAnimationType.ImageButton => _imageHoverScale,
            _ => _textHoverScale
        };
        SetScale(button, scale);
    }

    public void PlayNormal(ButtonInteractable button, ButtonAnimationType buttonType) { SetScale(button, 1); }

    public void PlayInteractable(
        ButtonInteractable button,
        Color32 backgroundColor,
        Color32 foreGroundColor,
        ButtonAnimationType buttonType)
    {
        var key = GenerateCacheKey(button, buttonType, backgroundColor, foreGroundColor);
        if (TryGetSequence(key, out Sequence cachedSequence))
        {
            cachedSequence.Restart();
            Debug.Log($"PlayButtonInteractable Sequence started from cache (key: {key})");

            return;
        }

        var seq = DOTween.Sequence()
            .Append(button.ImageComponent.DOColor(backgroundColor, _animationDuration))
            .Join(button.TextComponent.DOColor(foreGroundColor, _animationDuration))
            .Pause()
            .SetAutoKill(false);

        AddSequence(key, seq);
        seq.Restart();
    }
    #endregion

    public void PlayToolbarShowSpinner(ToolbarButtonController button)
    {
        button.m_icon.transform
            .DOScale(_toolbarSpinnerScale, .25f)
            .SetEase(Ease.InBack)
            .OnComplete(
                () =>
                {
                    button.m_spinnerBackground.gameObject.SetActive(true);
                    button.m_spinner.gameObject.SetActive(true);
                    button.m_animator.Button.interactable = false;
                });
    }

    public void PlayToolbarHideSpinner(ToolbarButtonController button)
    {
        button.m_spinnerBackground.gameObject.SetActive(false);
        button.m_spinner.gameObject.SetActive(false);

        button.m_icon.transform
            .DOScale(1f, .25f)
            .SetEase(Ease.OutBack)
            .OnComplete(
                () =>
                {
                    button.m_animator.Button.interactable = true;
                });
    }

    public void PlayToolbarToggleOn(ToolbarButtonController button)
    {
        button.m_icon.transform
            .DOScale(_toolbarToggleScale, .25f)
            .SetEase(Ease.InBack)
            .OnComplete(
                () =>
                {
                    button.m_toggleIcon.gameObject.SetActive(true);
                    button.m_button.interactable = false;

                    //animator.enabled = !IsToggleOn;

                    if (_toolbarHoverColorChange)
                        button.m_icon.color = MusicMateManager.Instance.AccentColor;
                });
    }

    public void PlayToolbarToggleOff(ToolbarButtonController button)
    {
        button.m_icon.transform
            .DOScale(1f, .25f)
            .SetEase(Ease.OutBack)
            .OnComplete(
                () =>
                {
                    button.m_toggleIcon.gameObject.SetActive(false);
                    button.m_button.interactable = true;

                    //animator.enabled = !IsToggleOn;
                });
    }

    public void PlayToolbarShowTooltip(ToolbarButtonController button)
    {
        button.m_tooltipText.color = button.m_button.interactable || button.IsToggleOn
            ? MusicMateManager.Instance.AppConfiguration.AccentColor
            : MusicMateManager.Instance.AppConfiguration.TextColor;

        button.m_tooltipPanel.localScale = Vector3.zero;
        button.m_tooltipPanel.gameObject.SetActive(true);
        button.m_tooltipPanel.DOScale(1, _toolbarTooltipPopupTime);
        button.m_tooltipVisible = true;
    }

    public void PlayToolbarHideTooltip(ToolbarButtonController button)
    {
        button.m_tooltipPanel.DOScale(0, _toolbarTooltipPopupTime).OnComplete(() => button.m_tooltipPanel.gameObject.SetActive(false));
        button.m_tooltipVisible = false;
    }

    string GenerateCacheKey(
        ButtonInteractable button,
        ButtonAnimationType buttonType,
        Color32 backgroundColor,
        Color32 foregroundColor)
    {
        string backgroundHex = ColorUtility.ToHtmlStringRGB(backgroundColor);
        string foregroundHex = ColorUtility.ToHtmlStringRGB(foregroundColor);
        return $"{button.GetInstanceID()}_{buttonType}_{backgroundHex}_{foregroundHex}";
    }

    string GenerateCacheKey(
        ButtonInteractable button,
        ButtonAnimationType buttonType,
        float scaleClick,
        float scaleHover) => $"{button.GetInstanceID()}_{buttonType}_{scaleClick}_{scaleHover}";

    bool TryGetSequence(string key, out Sequence sequence) => _sequenceCache.TryGetValue(key, out sequence);

    void AddSequence(string key, Sequence sequence)
    {
        if (_sequenceCache.Count >= _maxAnimationCacheSize)
        {
            var oldestKey = new List<string>(_sequenceCache.Keys)[0];
            _sequenceCache[oldestKey].Kill();
            _sequenceCache.Remove(oldestKey);
        }

        _sequenceCache[key] = sequence;
    }

    [ContextMenu("Clear Animation Cache")]
    void ClearCache()
    {
        foreach (var sequence in _sequenceCache.Values)
            sequence.Kill();

        _sequenceCache.Clear();
    }

    void SetScale(ButtonInteractable button, float scale) => button.transform
        .DOScale(scale, _animationDuration)
        .SetEase(_animationEase);
}