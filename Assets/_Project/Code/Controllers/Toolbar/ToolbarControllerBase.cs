using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class ToolbarControllerBase : MusicMateBehavior
{
    internal CanvasGroup m_CanvasGroup;

    protected override void InitializeComponents()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void InitializeValues()
    {
        m_CanvasGroup.alpha = 0f;
        InitElements();
        ChangeElementStates();
    }

    protected void ChangeElementStates() => StartCoroutine(SetElementStates());

    protected virtual void InitElements() { }

    /// <summary>
    /// Set the state of elements on the toolbar
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator SetElementStates()
    {
        yield return null;
    }
}