using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ToolbarApplicationController : MonoBehaviour
{
    internal CanvasGroup m_canvasGroup;

    void Awake()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        //m_canvasGroup.alpha = 0f;
    }
}