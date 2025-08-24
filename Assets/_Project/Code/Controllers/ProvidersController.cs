using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ProvidersController : MusicMateBehavior
{
    [SerializeField] ButtonAnimator _header;

    Image _panel;
    internal CanvasGroup m_canvasGroup;

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        _header.OnButtonClick.AddListener(OnCollapseProvidersClick);
    }

    protected override void UnregisterEventHandlers()
    {
        _header.OnButtonClick.RemoveListener(OnCollapseProvidersClick);
    }

    protected override void InitializeComponents()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        _panel = GetComponent<Image>();
    }

    private void OnCollapseProvidersClick()
    {
        Manager.AppState.InvokeStateChanged(MusicMateStateChange.Providers, false);
    }

    protected override void ApplyColors()
    {
        ChangeColor(MusicMateColor.Panel, _panel);
    }
    #endregion

}
