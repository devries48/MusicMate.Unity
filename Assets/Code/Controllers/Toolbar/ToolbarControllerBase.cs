using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class ToolbarControllerBase : MonoBehaviour
{
    public CanvasGroup m_CanvasGroup;

    protected IMusicMateManager m_Manager;
    protected IApiService m_ApiService;

    protected virtual void Awake()
    {
        m_Manager = MusicMateManager.Instance;
        m_ApiService = ApiService.Instance.GetClient();
    }

    protected virtual void Start()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_CanvasGroup.alpha = 0f;

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