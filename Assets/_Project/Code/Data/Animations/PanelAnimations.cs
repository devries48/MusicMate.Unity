using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(menuName = "MusicMate/Animations/Panel Animations", fileName = "Panel Animations")]
public class PanelAnimations : ScriptableObject, IPanelAnimations
{
    [Header("Default Window Visibility")]
    [SerializeField] float _showAndHideDuration = 0.5f;
    [SerializeField, Tooltip("A delay before hiding the window")] float _hideDelay = .1f;
    [SerializeField] Ease _showEase = Ease.OutBack;
    [SerializeField] Ease _hideEase = Ease.InBack;

    [Header("Default Panel Fading")]
    [SerializeField] float _fadeDuration = .5f;
    [SerializeField] Ease _fadeEase = Ease.InCirc;

    [Header("Default Color/Theme Transition")]
    [SerializeField] float _colorDuration = .5f;
    [SerializeField] Ease _colorEase = Ease.InOutSine;

    [Header("Login Window")]
    [SerializeField, Tooltip("Pivot Y position to hide Login Window")] float _loginHidePivot = -2f;
    [SerializeField, Tooltip("Pivot Y position to show Login Window")] float _loginShowPivot = .5f;

    [Header("Error Window")]
    [SerializeField, Tooltip("Pivot Y position to hide Error Window")] float _errorHidePivot = 3f;
    [SerializeField, Tooltip("Pivot Y position to show Error Window")] float _errorShowPivot = .5f;

    [Header("Result Grid")]
    [SerializeField] float _resultShowTime = .5f;
    [SerializeField] Ease _resultShowEase = Ease.InQuint;
    [SerializeField] float _resultHideTime = .5f;
    [SerializeField] float _resultHideScaleTo = .5f;
    [SerializeField] float _resultHideFadeTo = .01f;
    [SerializeField] Ease _resultHideEase = Ease.OutQuint;

    [Header("Details Panel")]
    [SerializeField] float _detailsShowTime = .1f;
    [SerializeField] Ease _detailsShowEase = Ease.Linear;
    [SerializeField] float _detailsHideTime = .2f;
    [SerializeField] Ease _detailsHideEase = Ease.OutQuint;
    [SerializeField] float _detailsSwitchTime = .3f;
    [SerializeField] Ease _detailsSwitchShowEase = Ease.InQuart;
    [SerializeField] Ease _detailsSwitchHideEase = Ease.OutQuart;

    [Header("Providers Panel")]
    [SerializeField] float _providersShowTime = .5f;
    [SerializeField] Ease _providersShowEase = Ease.OutBack;
    [SerializeField, Tooltip("Pivot X position to show Providers Panel")] float _providersShowPivot = 1f;
    [SerializeField] float _providersHideTime = .2f;
    [SerializeField] Ease _providersHideEase = Ease.InBack;
    [SerializeField, Tooltip("Pivot X position to hide Providers Panel")] float _providersHidePivot = -.5f;

    [Header("AudioPlayer Collapsed")]
    [SerializeField] float _playerCollapseTime = 1f;
    [SerializeField] float _playerSmallShowDelay = .5f;
    [SerializeField] Ease _playerLargeShrinkEase = Ease.InBack;
    [SerializeField] Ease _playerLargeHideEase = Ease.InBack;
    [SerializeField] Ease _playerSmallShowEase = Ease.OutBack;

    [Header("AudioPlayer Expanded")]
    [SerializeField] float _playerExpandTime = 1f;
    [SerializeField] float _playerLargeGrowDelay = .5f;
    [SerializeField] Ease _playerSmallHideEase = Ease.InBack;
    [SerializeField] Ease _playerLargeShowEase = Ease.OutBack;
    [SerializeField] Ease _playerLargeGrowEase = Ease.OutBack;


    public void PlayShowLoginWindow(GameObject loginWindow, float delay) => MoveVertical(
        true,
        loginWindow,
        _loginHidePivot,
        _loginShowPivot,
        delay);

    public void PlayHideLoginWindow(GameObject loginWindow) => MoveVertical(
        false,
        loginWindow,
        _loginHidePivot,
        _loginShowPivot,
        _hideDelay);

    public void PlayShowErrorWindow(GameObject errorWindow) => MoveVertical(
        true,
        errorWindow,
        _errorHidePivot,
        _errorShowPivot);

    public void PlayHideErrorWindow(GameObject errorWindow) => MoveVertical(
        false,
        errorWindow,
        _errorHidePivot,
        _errorShowPivot,
        _hideDelay);

    public void PlayCollapseAudioPlayer(RectTransform largePlayer, RectTransform smallPlayer, bool delay = false, Action onComplete = null)
    {
        var scaleTo = largePlayer.transform.localScale * .8f;

        largePlayer.DOScale(scaleTo, _playerCollapseTime / 2)
            .SetEase(_playerLargeShrinkEase)
            .OnComplete(
                () => largePlayer.DOPivotX(-.1f, _playerCollapseTime / 2)
                    .SetEase(_playerLargeHideEase)
                    .OnComplete(() => onComplete?.Invoke()));

        smallPlayer.DOPivotY(1, _playerCollapseTime).SetEase(_playerSmallShowEase).SetDelay(_playerSmallShowDelay);
    }

    public void PlayExpandAudioPlayer(
        RectTransform expandedPlayer,
        RectTransform collapsedPlayer,
        bool delay = false,
        Action onComplete = null)
    {
        collapsedPlayer.DOPivotY(-.5f, _playerExpandTime / 2).SetEase(_playerSmallHideEase);

        var delayTime = delay ? _providersHideTime : 0;

        expandedPlayer.DOPivotX(1, _playerExpandTime / 2)
            .SetEase(_playerLargeShowEase)
            .SetDelay(_playerLargeGrowDelay + delayTime)
            .OnComplete(
                () =>
                {
                    expandedPlayer.DOScale(1, _playerExpandTime / 2).SetEase(_playerLargeGrowEase);
                    onComplete?.Invoke();
                });
    }

    public void PlayPanelVisibility(bool fadeIn, float duration, float delay = 0, params CanvasGroup[] canvases)
    {
        if (canvases == null || canvases.Length == 0)
            return;

        if (duration == 0f)
            duration = _fadeDuration;

        foreach (var canvas in canvases)
        {
            if (canvas == null)
                continue;

            canvas.alpha = fadeIn ? 0f : 1f;
            canvas.DOFade(fadeIn ? 1f : 0f, duration).SetEase(_fadeEase).SetDelay(delay);
        }
    }

    public void PlayImageColor(Image image, Color32 toColor)
    { image.DOColor(toColor, _colorDuration).SetEase(_colorEase); }

    public void PlayTextColor(TextMeshProUGUI text, Color32 toColor)
    { text.DOColor(toColor, _colorDuration).SetEase(_colorEase); }

    public void PlayMarqueeColor(Marquee marquee, Color32 toColor)
    { DOTween.To(() => marquee.TextColor, x => marquee.TextColor = x, toColor, _colorDuration).SetEase(_colorEase); }

    public void PlayReleaseGridVisiblity(bool isVisible, GridReleaseController release)
    {
        var scaleTo = isVisible ? 1f : _resultHideScaleTo;
        var fadeTo = isVisible ? 1f : _resultHideFadeTo;
        var easing = isVisible ? _resultShowEase : _resultHideEase;
        var time = isVisible ? _resultShowTime : _resultHideTime;

        release.transform.DOScale(scaleTo, time).SetEase(easing);
        release.m_canvasGroup.DOFade(fadeTo, time).SetEase(easing);
    }

    public void PlayDetailsVisibility(bool isVisible, DetailsAnimator showDetails)
    {
        if (isVisible)
            showDetails.gameObject.SetActive(true);

        var scaleTo = isVisible ? 1f : 0f;
        var easing = isVisible ? _detailsShowEase : _detailsHideEase;
        var time = isVisible ? _detailsShowTime : _detailsHideTime;

        showDetails.transform
            .DOScale(scaleTo, time)
            .SetEase(easing)
            .OnComplete(
                () =>
                {
                    if (!isVisible)
                        showDetails.gameObject.SetActive(false);
                });
    }

    public void PlaySwitchDetails(GameObject showGroupObject, CanvasGroup hideGroup)
    {
        showGroupObject.TryGetComponent<CanvasGroup>(out var showGroup);
        showGroup.gameObject.SetActive(true);

        if (hideGroup != null)
        {
            hideGroup.DOFade(0, _detailsSwitchTime).SetEase(_detailsSwitchHideEase).OnComplete(() =>
            {
                hideGroup.gameObject.SetActive(false);
            });
        }

        showGroup.DOFade(1, _detailsSwitchTime).SetEase(_detailsSwitchShowEase);
    }

    public void PlayProvidersVisibility(bool isVisible, ProvidersController providers, bool delay = false)
    {
        if (isVisible)
            providers.gameObject.SetActive(true);

        // Wait for Audip Player to collapse
        float delaytime = delay ? _playerCollapseTime : 0f;

        var pivotTo = isVisible ? _providersShowPivot : _providersHidePivot;
        var easing = isVisible ? _providersShowEase : _providersHideEase;
        var rect = providers.gameObject.GetComponent<RectTransform>();

        rect.DOPivotX(pivotTo, _providersShowTime)
            .SetEase(easing)
            .SetDelay(delaytime)
            .OnComplete(
                () =>
                {
                    if (!isVisible)
                        providers.gameObject.SetActive(false);
                });
    }

    void MoveVertical(bool show, GameObject obj, float hidePivot, float showPivot, float delay = 0f)
    {
        if (show)
            obj.SetActive(true);

        var pivotTo = show ? showPivot : hidePivot;
        var easing = show ? _showEase : _hideEase;
        var rect = obj.GetComponent<RectTransform>();

        rect.DOPivotY(pivotTo, _showAndHideDuration)
            .SetEase(easing)
            .SetDelay(delay)
            .OnComplete(
                () =>
                {
                    if (!show)
                        obj.SetActive(false);
                });
    }

}