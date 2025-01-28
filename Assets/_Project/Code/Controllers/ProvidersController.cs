using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ProvidersController : MusicMateBehavior
{
    internal CanvasGroup m_canvasGroup;

    #region Base Class Methods
    protected override void InitializeComponents()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
    }
    #endregion

}
