using System;
using TMPro;
using UnityEngine;

public interface IAnimationManager
{
    float TooltipDelay { get; }
    float TooltipPadding { get; }
    float TooltipPanelWidth { get; }

    IButtonAnimations Button { get; }
    IGridAnimations Grid { get; }
    IInputAnimations Input { get; }
    IPanelAnimations Panel { get; }
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

    void PlayCollapseAudioPlayer(RectTransform largePlayer, RectTransform smallPlayer, Action onComplete = null);
    void PlayExpandAudioPlayer(RectTransform expandedPlayer, RectTransform collapsedPlayer, Action onComplete = null);

    void PlayShowLoginWindow(GameObject loginWindow, float delay);
    void PlayHideLoginWindow(GameObject loginWindow);
    void PlayShowErrorWindow(GameObject errorWindow);
    void PlayHideErrorWindow(GameObject errorWindow);

    void PlayGridReleaseVisiblity(bool isVisible, GridReleaseController release);
    void PlayDetailsVisibility(bool isVisible, DetailsAnimator showDetails);

}

public interface IToolbarAnimations
{
    void PlayButtonClicked(ToolbarButtonAnimator button);
    void PlayButtonHoverEnter(ToolbarButtonAnimator button, float duration = 0);
    void PlayButtonHoverExit(ToolbarButtonAnimator button);
    void PlayButtonInteractableChanged(ToolbarButtonAnimator button, bool isInteractable);

    void PlayToolbarShowSpinner(ToolbarButtonAnimator button);
    void PlayToolbarHideSpinner(ToolbarButtonAnimator button);
    void PlayToolbarToggleOn(ToolbarButtonAnimator button);
    void PlayToolbarToggleOff(ToolbarButtonAnimator button);
    void PlayToolbarShowTooltip(ToolbarButtonAnimator button);
    void PlayToolbarHideTooltip(ToolbarButtonAnimator button);
    void PlayToolbarPartRotate(ToolbarPartController controller, string title, GameObject showPart, GameObject hidePart);
}
