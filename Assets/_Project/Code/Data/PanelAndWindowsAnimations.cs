using UnityEngine;
using DG.Tweening;
using System;

[CreateAssetMenu(menuName = "MusicMate/Panel & Window Animation Settings", fileName = "Panel & Window Animations")]
public class PanelAndWindowsAnimations : ScriptableObject
{
    [Header("Visibility")]
    [SerializeField] float _showAndHideDuration = 0.5f;
    [SerializeField, Tooltip("A delay before hiding the window")] float _hideDelay = .1f;
    [SerializeField] Ease _showEase = Ease.OutBack;
    [SerializeField] Ease _hideEase = Ease.InBack;

    [Header("Fading")]
    [SerializeField] float _fadeDuration = .5f;
    [SerializeField] Ease _fadeEase = Ease.InCirc;

    [Header("Login Window Visibility")]
    [SerializeField, Tooltip("Pivot Y position to hide Login Window")] float _loginHidePivot = -2f;
    [SerializeField, Tooltip("Pivot Y position to show Login Window")] float _loginShowPivot = .5f;

    [Header("Error Window Visibility")]
    [SerializeField, Tooltip("Pivot Y position to hide Error Window")] float _errorHidePivot = 3f;
    [SerializeField, Tooltip("Pivot Y position to show Error Window")] float _errorShowPivot = .5f;

    [Header("Result")]
    [SerializeField] float _resultHideScaleTo = .5f;
    [SerializeField] float _resultHideFadeTo = .01f;
    [SerializeField] Ease _resultShowEase = Ease.InQuint;
    [SerializeField] Ease _resultHideEase = Ease.OutQuint;

    [Header("AudioPlayer Collapse")]
    [SerializeField] float _playerCollapseTime = 1f;
    [SerializeField] float _playerSmallShowDelay = .5f;
    [SerializeField] Ease _playerLargeShrinkEase = Ease.InBack;
    [SerializeField] Ease _playerLargeHideEase = Ease.InBack;
    [SerializeField] Ease _playerSmallShowEase = Ease.OutBack;

    [Header("AudioPlayer Expand")]
    [SerializeField] float _playerExpandTime = 1f;
    [SerializeField] float _playerLargeGrowDelay = .5f;
    [SerializeField] Ease _playerSmallHideEase = Ease.InBack;
    [SerializeField] Ease _playerLargeShowEase = Ease.OutBack;
    [SerializeField] Ease _playerLargeGrowEase = Ease.OutBack;

    public void PlayShowLoginWindow(GameObject loginWindow, float delay) => MoveVertical(true, loginWindow, _loginHidePivot, _loginShowPivot, delay);

    public void PlayHideLoginWindow(GameObject loginWindow) => MoveVertical(false, loginWindow, _loginHidePivot, _loginShowPivot, _hideDelay);

    public void PlayShowErrorWindow(GameObject errorWindow) => MoveVertical(true, errorWindow, _errorHidePivot, _errorShowPivot);

    public void PlayHideErrorWindow(GameObject errorWindow) => MoveVertical(false, errorWindow, _errorHidePivot, _errorShowPivot, _hideDelay);

    public void PlayCollapseAudioPlayer(RectTransform largePlayer, RectTransform smallPlayer, Action onComplete = null)
    {
        var scaleTo = largePlayer.transform.localScale * .8f;

        largePlayer.DOScale(scaleTo, _playerCollapseTime / 2)
            .SetEase(_playerLargeShrinkEase)
            .OnComplete(
                () => largePlayer.DOPivotX(-.1f, _playerCollapseTime / 2)
                    .SetEase(_playerLargeHideEase)
                    .OnComplete(() => onComplete.Invoke()));

        smallPlayer.DOPivotY(1, _playerCollapseTime)
            .SetEase(_playerSmallShowEase)
            .SetDelay(_playerSmallShowDelay);
    }

    public void PlayExpandAudioPlayer(RectTransform expandedPlayer, RectTransform collapsedPlayer, Action onComplete = null)
    {
        collapsedPlayer.DOPivotY(-.5f, _playerExpandTime / 2).SetEase(Ease.InBack);

        expandedPlayer.DOPivotX(1, _playerExpandTime / 2)
            .SetEase(Ease.OutBack).SetDelay(_playerLargeGrowDelay)
            .OnComplete(
                () => expandedPlayer.DOScale(1, _playerExpandTime / 2)
                    .SetEase(Ease.OutBack));
    }


    public void PlayPanelFade(bool fadeIn, float duration, float delay = 0, params CanvasGroup[] canvases)
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
            canvas.DOFade(fadeIn ? 1f : 0f, duration)
                .SetEase(_fadeEase)
                .SetDelay(delay);
        }
    }

    void MoveVertical(bool show, GameObject obj, float hidePivot, float showPivot, float delay = 0f)
    {
        if (show)
            obj.SetActive(true);

        var pivotTo = show ? showPivot : hidePivot;
        var easing = show ? _showEase : _hideEase;
        var rect = obj.GetComponent<RectTransform>();

        rect.DOPivotY(pivotTo, _showAndHideDuration).SetEase(easing)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                if (!show)
                    obj.SetActive(false);
            });
    }

}