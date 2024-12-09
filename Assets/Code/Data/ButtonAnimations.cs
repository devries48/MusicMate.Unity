using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Button Animation Settings", fileName = "Button Animations")]
public class ButtonAnimations : ScriptableObject
{
    [Header("General Settings")]
    public float animationDuration = 0.2f;
    public Ease animationEase = Ease.OutBack;

    [Header("Text Button Animations")]
    public float textHoverScale = 1.1f;
    public float textClickScale = 0.9f;

    [Header("Image Button Animations")]
    public float imageHoverScale = 1.2f;
    public float imageClickScale = 0.8f;

    readonly Dictionary<string, Sequence> _sequenceCache = new();
    const int MaxCacheSize = 100;

    void OnDisable() => ClearCache();

    public void PlayClicked(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        var scaleClick = buttonType switch
        {
            ButtonAnimationType.ImageButton => imageClickScale,
            _ => textClickScale
        };

        var scaleHover = buttonType switch
        {
            ButtonAnimationType.ImageButton => imageHoverScale,
            _ => textHoverScale
        };

        var key = GenerateCacheKey(buttonType, scaleClick, scaleHover);
        if (TryGetSequence(key, out Sequence cachedSequence))
        {
            Debug.Log($"PlayClicked Sequence started from cache (key: {key})");
            cachedSequence.Restart();
            return;
        }

        var seq = DOTween.Sequence()
            .Append(button.transform.DOScale(scaleClick, animationDuration).SetEase(animationEase))
            .Append(button.transform.DOScale(scaleHover, animationDuration).SetEase(animationEase))
            .Pause()
            .SetAutoKill(false);

        AddSequence(key, seq);
        seq.Restart();
    }

    public void PlayHover(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        var scale = buttonType switch
        {
            ButtonAnimationType.ImageButton => imageHoverScale,
            _ => textHoverScale
        };
        SetScale(button, scale);
    }

    public void PlayNormal(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        SetScale(button, 1);
    }

    public void PlayButtonInteractable(ButtonInteractable button, Color32 backgroundColor, Color32 foreGroundColor, ButtonAnimationType buttonType)
    {
        var key = GenerateCacheKey(buttonType, backgroundColor, foreGroundColor);
        if (TryGetSequence(key, out Sequence cachedSequence))
        {
            cachedSequence.Restart();
            Debug.Log($"PlayButtonInteractable Sequence started from cache (key: {key})");

            return;
        }

        var seq = DOTween.Sequence()
            .Append(button.ImageComponent.DOColor(backgroundColor, animationDuration))
            .Join(button.TextComponent.DOColor(foreGroundColor, animationDuration))
            .Pause()
            .SetAutoKill(false);

        AddSequence(key, seq);
        seq.Restart();
    }

    string GenerateCacheKey(ButtonAnimationType buttonType, Color32 backgroundColor, Color32 foregroundColor)
    {
        string backgroundHex = ColorUtility.ToHtmlStringRGB(backgroundColor);
        string foregroundHex = ColorUtility.ToHtmlStringRGB(foregroundColor);
        return $"{buttonType}_{backgroundHex}_{foregroundHex}";
    }

    string GenerateCacheKey(ButtonAnimationType buttonType, float scaleClick, float scaleHover) => $"{buttonType}_{scaleClick}_{scaleHover}";

    bool TryGetSequence(string key, out Sequence sequence) => _sequenceCache.TryGetValue(key, out sequence);

    void AddSequence(string key, Sequence sequence)
    {
        if (_sequenceCache.Count >= MaxCacheSize)
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

    void SetScale(ButtonInteractable button, float scale)
    {
        button.transform.DOScale(scale, animationDuration).SetEase(animationEase);
    }
}