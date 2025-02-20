using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ToolbarModeController : ToolbarControllerBase
{
    [SerializeField] ToolbarButtonAnimator _editModeButton;
    [SerializeField] ToolbarButtonAnimator _providersButton;

    internal LayoutElement m_layoutElement;
    internal RectTransform m_groupElement;

    #region MusicMate Base Class Methods
    protected override void RegisterEventHandlers()
    {
        base.RegisterEventHandlers();

        _editModeButton.OnButtonClick.AddListener(OnEditModeClicked);
        _providersButton.OnButtonClick.AddListener(OnProvidersToggleClicked);

        Manager.AppState.SubscribeToMusicMateStateChanged(OnMusicMateStateChanged);
    }

    protected override void UnregisterEventHandlers()
    {
        base.UnregisterEventHandlers();

        CancelInvoke();

        _editModeButton.OnButtonClick.RemoveListener(OnEditModeClicked);
        _providersButton.OnButtonClick.RemoveListener(OnProvidersToggleClicked);

        Manager.AppState.UnsubscribeFromMusicMateStateChangedd(OnMusicMateStateChanged);
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();

        var group = GetComponentInChildren<HorizontalLayoutGroup>();

        m_layoutElement = GetComponent<LayoutElement>();
        m_groupElement = group.gameObject.GetComponent<RectTransform>();
    }

    protected override void MusicMateModeChanged(MusicMateMode mode)
    {
        if (mode == MusicMateMode.Collection && _providersButton.IsToggleOn)
            _providersButton.SetToggleState(false);

        Animations.Toolbar.PlayModePanelResize(mode, this);

        _providersButton.gameObject.SetActive(mode == MusicMateMode.Edit);
    }

    #endregion

    void OnEditModeClicked()
    {
        var mode = _editModeButton.IsToggleOn ? MusicMateMode.Edit : MusicMateMode.Collection;
        Manager.AppState.NotifyModeChanged(mode);
    }

    void OnProvidersToggleClicked()
    {
        Manager.AppState.InvokeStateChanged(MusicMateStateChange.Providers, _providersButton.IsToggleOn);
    }

    void OnMusicMateStateChanged(MusicMateState state)
    {
        if (state.Change == MusicMateStateChange.Providers)
        {
            if (_providersButton.IsToggleOn != state.ShowProviders)
                _providersButton.SetToggleState(state.ShowProviders);
        }
    }

}