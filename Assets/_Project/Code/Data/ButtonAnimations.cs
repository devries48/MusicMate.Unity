using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Button Animation Settings", fileName = "Button Animations")]
public class ButtonAnimations : ScriptableObject
{
    [Header("General Settings")]
    [SerializeField, Tooltip("Default animation duration")] float _animationTime = 0.2f;
    [SerializeField] Ease _animationEase = Ease.OutBack;

    [Header("Text Button")]
    [SerializeField] float _textHoverScale = 1.1f;
    [SerializeField] float _textClickScale = 0.9f;

    [Header("Default Image Button")]
    [SerializeField] float _imageScale = .6f;
    [SerializeField] float _imageHoverScale = .8f;
    [SerializeField] float _imageClickScale = 0.4f;

    [Header("Large Image Button")]
    [SerializeField] float _imageLargeScale = 1f;
    [SerializeField] float _imageLargeHoverScale = 1.2f;
    [SerializeField] float _imageLargeClickScale = 0.8f;

    [Header("Toolbar Button")]
    [SerializeField] float _toolbarHoverScale = 1.2f;
    [SerializeField] float _toolbarClickScale = 0.8f;
    [SerializeField] float _toolbarToggleScale = 0.7f;
    [SerializeField] float _toolbarTooltipPopupTime = 0.1f;

    [Header("Toolbar Spinner")]
    [SerializeField, Tooltip("Resize the icon when the spinner is active")] float _toolbarSpinnerScale = 0.7f;
    [SerializeField, Tooltip("Animation duration when spinner is (de)activated")] float _toolbarSpinTime = 0.4f;

    [Header("Expand/Collapse")]
    [SerializeField] float _iconAnimationTime = .6f;

    public float ImageButtonScale => _imageScale;

    public float ImageButtonLargeScale => _imageLargeScale;

    readonly Dictionary<RectTransform, Tween> _expandTweens = new();

    #region Default Button Events
    public void PlayClicked(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        if (buttonType == ButtonAnimationType.ExpandCollapseButton)
            ExpandOrCollapsClickAnimation(button);
        else
        {
            var duration = _animationTime / 2;
            var scaleClick = buttonType switch
            {
                ButtonAnimationType.ToolbarButton => _toolbarClickScale,
                ButtonAnimationType.DefaultImageButton => _imageClickScale,
                ButtonAnimationType.LargeImageButton => _imageLargeClickScale,
                _ => _textClickScale
            };
            button.transform
                .DOScale(scaleClick, duration)
                .SetEase(_animationEase)
                .OnComplete(() => PlayHover(button, buttonType, duration));
        }
    }

    public void PlayHover(ButtonInteractable button, ButtonAnimationType buttonType, float duration = 0)
    {
        if (duration == 0)
            duration = _animationTime;

        if (buttonType == ButtonAnimationType.ExpandCollapseButton)
            StartExpandOrCollapseAnimation(button);
        else
        {
            var scale = buttonType switch
            {
                ButtonAnimationType.DefaultImageButton => _imageHoverScale,
                ButtonAnimationType.LargeImageButton => _imageLargeHoverScale,
                ButtonAnimationType.ToolbarButton => _toolbarHoverScale,
                _ => _textHoverScale
            };

            button.transform.DOScale(scale, duration).SetEase(_animationEase);
        }
    }

    public void PlayNormal(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        if (buttonType == ButtonAnimationType.ExpandCollapseButton)
            StopExpandOrCollapseAnimation(button);
        else
        {
            var scale = buttonType switch
            {
                ButtonAnimationType.DefaultImageButton => _imageScale,
                ButtonAnimationType.LargeImageButton => _imageLargeScale,
                _ => 1.0f
            };

            button.transform.DOScale(scale, _animationTime).SetEase(_animationEase);
        }
    }

    void StartExpandOrCollapseAnimation(ButtonInteractable button)
    {
        button.TextComponent.DOColor(button.Colors.AccentColor, _animationTime);
        button.ImageComponent.DOColor(button.Colors.AccentColor, _animationTime);

        // Determine the state based on the rotation of the image
        bool isExpanded = Mathf.Approximately(button.ImageComponent.rectTransform.localEulerAngles.z, 180f);

        var target = button.ImageComponent.rectTransform;
        var height = target.rect.height;

        // Normalize animation speed based on the button's height
        const float defaultHeight = 40f; // Default button height
        float speedFactor = height / defaultHeight;
        float adjustedAnimationTime = _iconAnimationTime * speedFactor;

        // Kill any existing tween for this target
        if (_expandTweens.ContainsKey(target) && _expandTweens[target] != null)
        {
            _expandTweens[target].Kill();
            _expandTweens.Remove(target);
        }

        var startPos = target.anchoredPosition;
        var offscreenBottom = new Vector2(startPos.x, -height / 2);
        var offscreenTop = new Vector2(startPos.x, height / 2);

        // Determine initial movement direction based on panel state
        var initialTarget = isExpanded ? offscreenTop : offscreenBottom;
        var loopStart = isExpanded ? offscreenBottom : offscreenTop;
        var loopEnd = isExpanded ? offscreenTop : offscreenBottom;

        // Move the sprite out of view first
        target.anchoredPosition = startPos;
        Tween initialMove = target.DOAnchorPos(initialTarget, adjustedAnimationTime / 2).SetEase(Ease.Linear);

        // Set up the loop
        initialMove.OnComplete(
            () =>
            {
                Tween loopTween = target
                .DOAnchorPos(loopEnd, adjustedAnimationTime)
                    .From(loopStart)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart);

                _expandTweens[target] = loopTween;
            });

        _expandTweens[target] = initialMove;
    }

    void StopExpandOrCollapseAnimation(ButtonInteractable button)
    {
        button.TextComponent.DOColor(button.Colors.TextColor, _animationTime);
        button.ImageComponent.DOColor(button.Colors.TextColor, _animationTime);

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
        text.DOColor(button.Colors.TextColor, 0.1f).OnComplete(() =>
        {
            text.DOColor(originalColor, 0.1f);
        });
    }

    public void PlayInteractable(
        ButtonInteractable button,
        Color32 backgroundColor,
        Color32 foreGroundColor,
        ButtonAnimationType buttonType)
    {
        if (buttonType == ButtonAnimationType.TextButton)
        {
            button.TextComponent.DOColor(foreGroundColor, _animationTime);
            button.ImageComponent.DOColor(backgroundColor, _animationTime);
        }
        else
            button.ImageComponent.DOColor(foreGroundColor, _animationTime);
    }
    #endregion

    public void PlayToolbarShowSpinner(ToolbarButtonAnimator button)
    {
        button.m_icon.transform
            .DOScale(_toolbarSpinnerScale, _toolbarSpinTime)
            .SetEase(Ease.OutBack)
            .OnComplete(
                () =>
                {
                    button.m_spinnerBackground.gameObject.SetActive(true);
                    button.m_spinner.gameObject.SetActive(true);
                    button.SetInteractable(false);
                });
    }

    public void PlayToolbarHideSpinner(ToolbarButtonAnimator button)
    {
        button.m_spinnerBackground.gameObject.SetActive(false);
        button.m_spinner.gameObject.SetActive(false);

        button.m_icon.transform
            .DOScale(1f, _toolbarSpinTime)
            .SetEase(Ease.InBack)
            .OnComplete(() => button.SetInteractable(true));
    }

    public void PlayToolbarToggleOn(ToolbarButtonAnimator button)
    {
        button.m_icon.transform
            .DOScale(_toolbarToggleScale, .25f)
            .SetEase(Ease.InBack)
            .OnComplete(
                () =>
                {
                    button.m_toggleIcon.gameObject.SetActive(true);
                    button.m_button.interactable = false;
                    //button.m_icon.color = MusicMateManager.Instance.AccentColor;
                });
    }

    public void PlayToolbarToggleOff(ToolbarButtonAnimator button)
    {
        button.m_icon.transform
            .DOScale(1f, .25f)
            .SetEase(Ease.OutBack)
            .OnComplete(
                () =>
                {
                    button.m_toggleIcon.gameObject.SetActive(false);
                    button.m_button.interactable = true;
                });
    }

    public void PlayToolbarShowTooltip(ToolbarButtonAnimator button)
    {
        //button.m_tooltipText.color = button.m_button.interactable || button.IsToggleOn
        //    ? MusicMateManager.Instance.AccentColor
        //    : MusicMateManager.Instance.TextColor;
        button.m_tooltipPanel.localScale = Vector3.zero;
        button.m_tooltipPanel.gameObject.SetActive(true);
        button.m_tooltipPanel.DOScale(1, _toolbarTooltipPopupTime);
        button.m_tooltipVisible = true;
    }

    public void PlayToolbarHideTooltip(ToolbarButtonAnimator button)
    {
        button.m_tooltipPanel
            .DOScale(0, _toolbarTooltipPopupTime / 2)
            .OnComplete(() => button.m_tooltipPanel.gameObject.SetActive(false));
        button.m_tooltipVisible = false;
    }
}

