using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ProvidersController : MusicMateBehavior
{
    Image _panel;
    internal CanvasGroup m_canvasGroup;

    #region Base Class Methods
    protected override void InitializeComponents()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        _panel = GetComponent<Image>();
    }

    protected override void ApplyColors()
    {
        ChangeColor(MusicMateColor.Panel, _panel);
    }
    #endregion

}
