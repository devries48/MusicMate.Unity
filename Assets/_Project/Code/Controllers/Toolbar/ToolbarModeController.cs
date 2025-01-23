using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ToolbarModeController : ToolbarControllerBase
{
    [SerializeField] ToolbarButtonAnimator _editModeButton;

    #region MusicMate Base Class Methods
    protected override void RegisterEventHandlers()
    {
        base.RegisterEventHandlers();

        _editModeButton.OnButtonClick.AddListener(OnEditModeClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        base.UnregisterEventHandlers();

        CancelInvoke();

        _editModeButton.OnButtonClick.RemoveListener(OnEditModeClicked);
    }
    #endregion

    void OnEditModeClicked()
    {
        var mode = _editModeButton.IsToggleOn ? MusicMateMode.Edit : MusicMateMode.Collection;
        Manager.AppState.NotifyModeChanged(mode);
    }
}