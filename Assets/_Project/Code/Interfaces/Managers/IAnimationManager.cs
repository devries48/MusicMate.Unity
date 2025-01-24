using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The Animation Manager serves as the central hub for managing and orchestrating animations across various UI elements and components in the application. 
/// </summary>
public interface IAnimationManager
{
    float TooltipDelay { get; }
    float TooltipPadding { get; }
    float TooltipPanelWidth { get; }

    /// <summary>
    /// Handles animations for buttons, including hover effects, click feedback, and interactable state changes.
    /// </summary>
    IButtonAnimations Button { get; }
  
    /// <summary>
    /// Handles animations for grid elements, such as cell and row interactions, hover effects, and action panel transitions.
    /// </summary>
    IGridAnimations Grid { get; }

    /// <summary>
    /// Handles animations for input fields, including text highlighting and focus effects.
    /// </summary>
    IInputAnimations Input { get; }
 
    /// <summary>
    /// Handles animations for panels and windows, including visibility transitions, audio player state changes, login windows, error messages, and release grids.
    /// </summary>
    IPanelAnimations Panel { get; }
 
    /// <summary>
    /// Provides animations for toolbar elements, including button interactions, spinners, toggles, tooltips, and rotating parts.
    /// </summary>
    IToolbarAnimations Toolbar { get; }

    void PlayLogoHide(GameObject logo, Action onComplete = null);
}
public interface IButtonAnimations
{
    float ImageButtonScale { get; }
    float ImageButtonLargeScale { get; }

    void Initialize(IMusicMateManager manager);
    void PlayClicked(ButtonInteractable button, ButtonAnimationType buttonType);
    void PlayHoverEnter(ButtonInteractable button, ButtonAnimationType buttonType, float duration = 0);
    void PlayHoverExit(ButtonInteractable button, ButtonAnimationType buttonType);
    void PlayInteractableChanged(ButtonInteractable button, bool isInteractable, bool isPrimary, ButtonAnimationType buttonType);
}

public interface IGridAnimations
{
    void PlayCellHoverEnter(CellReleaseAnimator cell);
    void PlayCellHoverExit(CellReleaseAnimator cell);
    void PlayCellClick(CellReleaseAnimator cell);
    void PlayCellSelect(bool isSelected, CellReleaseAnimator cell);
    void AbortCellSelect(CellReleaseAnimator cell);
    void PlayRowClick(RowTrackAnimator row);
    void PlayShowActionPanel(RectTransform panelRect, RowTrackAnimator row);
    void PlayHideActionPanel(RectTransform panelRect);
}

public interface IInputAnimations
{
    void Initialize(IMusicMateManager manager);
    void PlayTextNormal(TMP_InputField input);
    void PlayTextSelect(TMP_InputField input);
    void PlayTextHighlight(TMP_InputField input);
}

public interface IPanelAnimations
{
    void PlayPanelVisibility(bool isVisible, float duration, float delay = 0, params CanvasGroup[] panels);
    void PlayImageColor(Image image, Color32 toColor);
    void PlayTextColor(TextMeshProUGUI text, Color32 toColor);
    void PlayMarqueeColor(Marquee marquee, Color32 toColor);
    void PlayCollapseAudioPlayer(RectTransform largePlayer, RectTransform smallPlayer, Action onComplete = null);
    void PlayExpandAudioPlayer(RectTransform expandedPlayer, RectTransform collapsedPlayer, Action onComplete = null);
    void PlayShowLoginWindow(GameObject loginWindow, float delay);
    void PlayHideLoginWindow(GameObject loginWindow);
    void PlayShowErrorWindow(GameObject errorWindow);
    void PlayHideErrorWindow(GameObject errorWindow);
    void PlayReleaseGridVisiblity(bool isVisible, GridReleaseController release);
    void PlayDetailsVisibility(bool isVisible, DetailsAnimator showDetails);

}

public interface IToolbarAnimations
{
    void Initialize(IMusicMateManager manager);
    void PlayClicked(ToolbarButtonAnimator button);
    void PlayHoverEnter(ToolbarButtonAnimator button, float duration = 0);
    void PlayHoverExit(ToolbarButtonAnimator button);
    void PlayInteractableChanged(ToolbarButtonAnimator button, bool isInteractable);
    void PlayShowSpinner(ToolbarButtonAnimator button);
    void PlayHideSpinner(ToolbarButtonAnimator button);
    void PlayToggleOn(ToolbarButtonAnimator button);
    void PlayToggleOff(ToolbarButtonAnimator button);
    void PlayShowTooltip(ToolbarButtonAnimator button);
    void PlayHideTooltip(ToolbarButtonAnimator button);
    void PlayPartRotate(ToolbarPartController controller, string title, GameObject showPart, GameObject hidePart);
}
