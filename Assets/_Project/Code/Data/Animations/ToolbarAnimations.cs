using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Animations/Toolbar Animations", fileName = "Toolbar Animations")]
public class ToolbarAnimations : ScriptableObject, IToolbarAnimations
{
    [SerializeField, Tooltip("Default animation duration")] float _animationTime = 0.2f;
    [SerializeField] Ease _animationEase = Ease.OutBack;

    // toolbar button
    [SerializeField] float _toolbarHoverScale = 1.2f;
    [SerializeField] float _toolbarClickScale = 0.8f;
    [SerializeField] float _toolbarToggleScale = 0.7f;
    [SerializeField] float _toolbarTooltipPopupTime = 0.1f;

    // toolbar spinner
    [SerializeField, Tooltip("Resize the icon when the spinner is active")] float _toolbarSpinnerScale = 0.7f;
    [SerializeField, Tooltip("Animation duration when spinner is (de)activated")] float _toolbarSpinTime = 0.4f;

    // toolbar part rotation
    [SerializeField] float _toolbarPartRotateTime = 0.5f;
    [SerializeField, Tooltip("Delay before rotating the new part into view")] float _toolbarPartDelayTime = 0.1f;

    IMusicMateManager _manager;

    public void Initialize(IMusicMateManager manager) => _manager = manager;

    public void PlayClicked(ToolbarButtonAnimator button)
    {
        var duration = _animationTime / 2;
        button.transform
            .DOScale(_toolbarClickScale, duration)
            .SetEase(_animationEase)
            .OnComplete(() => PlayHoverEnter(button, duration));
    }

    public void PlayHoverEnter(ToolbarButtonAnimator button, float duration = 0)
    {
        if (!button.IsInteractable || button.IsSpinning)
            return;

        if (duration == 0)
            duration = _animationTime;

        button.transform.DOScale(_toolbarHoverScale, duration).SetEase(_animationEase);
    }

    public void PlayHoverExit(ToolbarButtonAnimator button)
    {
        button.transform.DOScale(1, _animationTime).SetEase(_animationEase);
    }

    public void PlayShowSpinner(ToolbarButtonAnimator button)
    {
        button.m_icon.transform
            .DOScale(_toolbarSpinnerScale, _toolbarSpinTime)
            .SetEase(Ease.OutBack)
            .OnComplete(
                () =>
                {
                    button.transform.localScale = Vector2.one;
                    button.m_spinnerBackground.gameObject.SetActive(true);
                    button.m_spinner.gameObject.SetActive(true);
                    button.IsInteractable = false;
                });
    }

    public void PlayInteractableChanged(ToolbarButtonAnimator button, bool isInteractable)
    {
        Color32 color = isInteractable ? _manager.AppColors.IconColor : _manager.AppColors.DisabledIconColor;
        button.m_icon.DOColor(color, _animationTime);
    }

    public void PlayHideSpinner(ToolbarButtonAnimator button)
    {
        button.m_spinnerBackground.gameObject.SetActive(false);
        button.m_spinner.gameObject.SetActive(false);

        button.m_icon.transform
            .DOScale(1f, _toolbarSpinTime)
            .SetEase(Ease.InBack)
            .OnComplete(() => button.IsInteractable = true);
    }

    public void PlayToggleOn(ToolbarButtonAnimator button)
    {
        button.m_icon.transform
            .DOScale(_toolbarToggleScale, .25f)
            .SetEase(Ease.InBack)
            .OnComplete(
                () =>
                {
                    button.m_toggleIcon.gameObject.SetActive(true);
                    button.IsInteractable = false;
                    //button.m_icon.color = MusicMateManager.Instance.AccentColor;
                });
    }

    public void PlayToggleOff(ToolbarButtonAnimator button)
    {
        button.m_icon.transform
            .DOScale(1f, .25f)
            .SetEase(Ease.OutBack)
            .OnComplete(
                () =>
                {
                    button.m_toggleIcon.gameObject.SetActive(false);
                    button.IsInteractable = true;
                });
    }

    public void PlayShowTooltip(ToolbarButtonAnimator button)
    {
        //button.m_tooltipText.color = button.m_button.interactable || button.IsToggleOn
        //    ? MusicMateManager.Instance.AccentColor
        //    : MusicMateManager.Instance.TextColor;
        button.m_tooltipPanel.localScale = Vector3.zero;
        button.m_tooltipPanel.gameObject.SetActive(true);
        button.m_tooltipPanel.DOScale(1, _toolbarTooltipPopupTime);
        button.m_tooltipVisible = true;
    }

    public void PlayHideTooltip(ToolbarButtonAnimator button)
    {
        button.m_tooltipPanel
            .DOScale(0, _toolbarTooltipPopupTime / 2)
            .OnComplete(() => button.m_tooltipPanel.gameObject.SetActive(false));
        button.m_tooltipVisible = false;
    }

    public void PlayPartRotate(ToolbarPartController controller, string title, GameObject showPart, GameObject hidePart)
    {
        var duration = _toolbarPartRotateTime / 2;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(
            controller.m_rectTransform.DORotate(new Vector3(90, 0, 0), duration)
                .SetEase(Ease.Linear)
                .OnComplete(
                    () =>
                    {
                        hidePart.SetActive(false);
                        showPart.SetActive(true);
                        controller.SetHeader(title);

                    }));
        sequence.Append(
            controller.m_rectTransform.DORotate(new Vector3(0, 0, 0), duration)
            .SetDelay(_toolbarPartDelayTime)
            .SetEase(Ease.Linear)
                .OnComplete(
                    () =>
                    {
                        controller.m_activePart = showPart;
                    }));
    }
}