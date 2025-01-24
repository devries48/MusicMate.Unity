using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class ToolbarControllerBase : MusicMateBehavior
{
    Image _panel;

    internal CanvasGroup m_CanvasGroup;

    protected override void InitializeComponents()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        _panel = GetComponent<Image>();
    }

    protected override void InitializeValues()
    {
        m_CanvasGroup.alpha = 0f;

        InitElements();
        SetElementStates();
    }

    protected virtual void InitElements() { }

    /// <summary>
    /// Set the state of elements on the toolbar
    /// </summary>
    /// <returns></returns>
    protected virtual void SetElementStates() { }

    protected override void ApplyColors()
    {
        ChangeColor(MusicMateColor.Panel, _panel);
    }
}