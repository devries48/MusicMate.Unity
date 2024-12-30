using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Button Animation Settings", fileName = "Button Animations")]
public class ButtonAnimations : ScriptableObject
{
    [Header("General Settings")]
    [SerializeField, Tooltip("Default animation duration")] float _animationTime = 0.2f;
    [SerializeField] Ease _animationEase = Ease.OutBack;

    [Header("Text Button Animations")]
    [SerializeField] float _textHoverScale = 1.1f;
    [SerializeField] float _textClickScale = 0.9f;

    [Header("Default Image Button Animations")]
    [SerializeField] float _imageScale = .6f;
    [SerializeField] float _imageHoverScale = .8f;
    [SerializeField] float _imageClickScale = 0.4f;

    [Header("Large Image Button Animations")]
    [SerializeField] float _imageLargeScale = 1f;
    [SerializeField] float _imageLargeHoverScale = 1.2f;
    [SerializeField] float _imageLargeClickScale = 0.8f;

    [Header("Toolbar Button Animations")]
    [SerializeField] float _toolbarHoverScale = 1.2f;
    //[SerializeField, Tooltip("Change the color to the accent color")] bool _toolbarHoverColorChange = true;
    [SerializeField] float _toolbarClickScale = 0.8f;
    [SerializeField] float _toolbarToggleScale = 0.7f;
    [SerializeField] float _toolbarTooltipPopupTime = 0.1f;

    [Header("Toolbar Spinner Animations")]
    [SerializeField, Tooltip("Resize the icon when the spinner is active")] float _toolbarSpinnerScale = 0.7f;
    [SerializeField, Tooltip("Animation duration when spinner is (de)activated")] float _toolbarSpinTime = 0.4f;

    public float ImageButtonScale => _imageScale;
    public float ImageButtonLargeScale => _imageLargeScale;

    //readonly Dictionary<string, Sequence> _sequenceCache = new();

    #region Default Button Events
    public void PlayClicked(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        var duration = _animationTime / 2;
        var scaleClick = buttonType switch
        {
            ButtonAnimationType.ToolbarButton => _toolbarClickScale,
            ButtonAnimationType.DefaultImageButton => _imageClickScale,
            ButtonAnimationType.LargeImageButton => _imageLargeClickScale,
            _ => _textClickScale
        };
        button.transform.DOScale(scaleClick, duration).SetEase(_animationEase)
            .OnComplete(() => PlayHover(button, buttonType, duration));
    }

    public void PlayHover(ButtonInteractable button, ButtonAnimationType buttonType, float duration = 0)
    {
        if (duration == 0) duration = _animationTime;

        var scale = buttonType switch
        {
            ButtonAnimationType.DefaultImageButton => _imageHoverScale,
            ButtonAnimationType.LargeImageButton => _imageLargeHoverScale,
            ButtonAnimationType.ToolbarButton => _toolbarHoverScale,
            _ => _textHoverScale
        };

        button.transform.DOScale(scale, duration).SetEase(_animationEase);
    }

    public void PlayNormal(ButtonInteractable button, ButtonAnimationType buttonType)
    {
        var scale = buttonType switch
        {
            ButtonAnimationType.DefaultImageButton => _imageScale,
            ButtonAnimationType.LargeImageButton => _imageLargeScale,
            _ => 1.0f
        };

        button.transform.DOScale(scale, _animationTime).SetEase(_animationEase);
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
        {
            button.ImageComponent.DOColor(foreGroundColor, _animationTime);
        }
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
            .OnComplete(
                () => button.SetInteractable(true));
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
        button.m_tooltipPanel.DOScale(0, _toolbarTooltipPopupTime / 2).OnComplete(() => button.m_tooltipPanel.gameObject.SetActive(false));
        button.m_tooltipVisible = false;
    }

}

