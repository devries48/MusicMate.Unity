using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class ToolbarControllerBase : MonoBehaviour
{
    internal CanvasGroup m_CanvasGroup;

    protected IMusicMateManager m_Manager;
    protected IApiService m_ApiService;

    protected virtual void Awake()
    {
        m_Manager = MusicMateManager.Instance;
        m_ApiService = ApiService.Instance.GetClient();
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_CanvasGroup.alpha = 0f;
    }

    protected virtual void Start()
    {
        InitElements();
        ChangeElementStates();
    }

    protected void ChangeElementStates() => StartCoroutine(SetElementStates());

    protected virtual void InitElements() { }

    protected virtual IEnumerator SetElementStates()
    {
        yield return null;
    }
}