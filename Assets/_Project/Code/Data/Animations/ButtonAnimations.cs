using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Animations/Button Animations", fileName = "Button Animations")]
public class ButtonAnimations : ScriptableObject, IButtonAnimations
{
    // general settings 
    [SerializeField, Tooltip("Default animation duration")] float _animationTime = 0.2f;
    [SerializeField] Ease _animationEase = Ease.OutBack;

    // text button
    [SerializeField] float _textHoverScale = 1.1f;
    [SerializeField] float _textClickScale = 0.9f;

    // default image button
    [SerializeField] float _imageScale = .6f;
    [SerializeField] float _imageHoverScale = .8f;
    [SerializeField] float _imageClickScale = 0.4f;

    // large image button
    [SerializeField] float _imageLargeScale = 1f;
    [SerializeField] float _imageLargeHoverScale = 1.2f;
    [SerializeField] float _imageLargeClickScale = 0.8f;

 
    // expand/collapse button
    [SerializeField] float _iconAnimationTime = .6f;
    [SerializeField] int _iconLoops = 3;

    IMusicMateManager _manager;

    public void Initialize(IMusicMateManager manager) => _manager = manager;

    public float ImageButtonScale => _imageScale;

    public float ImageButtonLargeScale => _imageLargeScale;

    readonly Dictionary<RectTransform, Tween> _expandTweens = new();

    #region Default Button Events
    public void PlayClicked(ButtonInteractable button, ButtonType buttonType)
    {
        if (buttonType == ButtonType.ExpandCollapse)
            ExpandOrCollapsClickAnimation(button);
        else
        {
            var duration = _animationTime / 2;
            var scaleClick = buttonType switch
            {
                ButtonType.DefaultImage => _imageClickScale,
                ButtonType.StateImage => _imageClickScale,
                ButtonType.LargeImage => _imageLargeClickScale,
                _ => _textClickScale
            };
            button.transform
                .DOScale(scaleClick, duration)
                .SetEase(_animationEase)
                .OnComplete(() => PlayHoverEnter(button, buttonType, duration));
        }
    }

    public void PlayHoverEnter(ButtonInteractable button, ButtonType buttonType, float duration = 0)
    {
        if (!button.interactable)
            return;

        if (duration == 0)
            duration = _animationTime;

        if (buttonType == ButtonType.ExpandCollapse)
            StartExpandOrCollapseAnimation(button);
        else
        {
            var scale = buttonType switch
            {
                ButtonType.DefaultImage => _imageHoverScale,
                ButtonType.StateImage => _imageHoverScale,
                ButtonType.LargeImage => _imageLargeHoverScale,
                _ => _textHoverScale
            };

            button.transform.DOScale(scale, duration).SetEase(_animationEase);
        }
    }

    public void PlayHoverExit(ButtonInteractable button, ButtonType buttonType)
    {
        if (buttonType == ButtonType.ExpandCollapse)
            StopExpandOrCollapseAnimation(button);
        else
        {
            var scale = buttonType switch
            {
                ButtonType.DefaultImage => _imageScale,
                ButtonType.StateImage => _imageScale,
                ButtonType.LargeImage => _imageLargeScale,
                _ => 1.0f
            };

            button.transform.DOScale(scale, _animationTime).SetEase(_animationEase);
        }
    }

    public void PlayInteractableChanged(ButtonInteractable button, bool isInteractable, bool isPrimary, ButtonType buttonType)
    {
        Color32 backgroundColor = _manager.AppColors.DefaultColor;
        Color32 foregroundColor;

        if (buttonType == ButtonType.DefaultImage || buttonType == ButtonType.LargeImage)
        {
            foregroundColor = isPrimary ? _manager.AppColors.AccentColor : _manager.AppColors.TextColor;
        }
        else
        {
            if (isInteractable)
            {
                if (isPrimary)
                    backgroundColor = _manager.AppColors.AccentColor;

                foregroundColor = isPrimary ? _manager.AppColors.AccentTextColor : _manager.AppColors.TextColor;
            }
            else
                foregroundColor = _manager.AppColors.AccentTextColor;
        }
        PlayInteractable(button, backgroundColor, foregroundColor, buttonType);
    }


    void StartExpandOrCollapseAnimation(ButtonInteractable button)
    {
        button.TextComponent.DOColor(_manager.AppColors.AccentColor, _animationTime);
        button.ImageComponent.DOColor(_manager.AppColors.AccentColor, _animationTime);

        bool isExpanded = Mathf.Approximately(button.ImageComponent.rectTransform.localEulerAngles.z, 180f);

        var target = button.ImageComponent.rectTransform;
        var height = target.rect.height;

        // Kill any existing tween for this target
        if (_expandTweens.ContainsKey(target) && _expandTweens[target] != null)
        {
            _expandTweens[target].Kill();
            _expandTweens.Remove(target);
        }

        var startPos = target.anchoredPosition; // Store the original position
        var offscreenBottom = new Vector2(startPos.x, -height / 2);
        var offscreenTop = new Vector2(startPos.x, height / 2);

        var initialTarget = isExpanded ? offscreenTop : offscreenBottom;
        var loopStart = isExpanded ? offscreenBottom : offscreenTop;
        var loopEnd = isExpanded ? offscreenTop : offscreenBottom;

        // Move out of view first
        target.anchoredPosition = startPos;
        Tween initialMove = target.DOAnchorPos(initialTarget, _iconAnimationTime / 2).SetEase(Ease.Linear);

        // Set up looping animation
        initialMove.OnComplete(
            () =>
            {
                Tween loopTween = target
                .DOAnchorPos(loopEnd, _iconAnimationTime)
                    .From(loopStart)
                    .SetEase(Ease.Linear)
                    .SetLoops(_iconLoops, LoopType.Restart)
                    .OnComplete(() => target.anchoredPosition = startPos); // Reset position after loops

                _expandTweens[target] = loopTween;
            });

        _expandTweens[target] = initialMove;
    }

    void StopExpandOrCollapseAnimation(ButtonInteractable button)
    {
        button.TextComponent.DOColor(_manager.AppColors.TextColor, _animationTime);
        button.ImageComponent.DOColor(_manager.AppColors.TextColor, _animationTime);

        var target = button.ImageComponent.rectTransform;
        var resetPosition = new Vector2(0f, 0f);

        // Kill any active tween for this target
        if (_expandTweens.ContainsKey(target) && _expandTweens[target] != null)
        {
            _expandTweens[target].Kill();
            _expandTweens.Remove(target);
        }

        // Reset to the original center position
        target.anchoredPosition = resetPosition;
    }

    void ExpandOrCollapsClickAnimation(ButtonInteractable button)
    {
        var buttonTransform = button.transform;
        var text = button.TextComponent; // Reference to button text
        buttonTransform.DOKill();
        text.DOKill();

        // Pop button
        buttonTransform.DOScale(1.1f, 0.1f).OnComplete(() =>
        {
            buttonTransform.DOScale(1f, 0.1f);
        });

        // Flash text color
        Color originalColor = text.color;
        text.DOColor(_manager.AppColors.TextColor, 0.1f).OnComplete(() =>
        {
            text.DOColor(originalColor, 0.1f);
        });
    }

    void PlayInteractable(
        ButtonInteractable button,
        Color32 backgroundColor,
        Color32 foregroundColor,
        ButtonType buttonType)
    {
        if (buttonType == ButtonType.Text)
        {
            button.TextComponent.DOColor(foregroundColor, _animationTime);
            button.ImageComponent.DOColor(backgroundColor, _animationTime);
        }
        else
            button.ImageComponent.DOColor(foregroundColor, _animationTime);
    }
    #endregion

}

