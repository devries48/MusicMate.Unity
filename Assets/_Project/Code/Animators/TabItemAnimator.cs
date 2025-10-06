#region Usings

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

#endregion

public class TabItemAnimator : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] string text;
    [SerializeField] bool isActive;

    public UnityEvent OnTabItemClick { get; private set; } = new UnityEvent();

    internal TextMeshProUGUI m_Text;
    internal bool m_IsActive;
    
    Image _background;

    bool _isInitialized = false;

    #region Base Class Methods

    protected override void InitializeComponents()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        transform.TryGetComponent(out _background);
        transform.Find("Text").TryGetComponent(out m_Text);
    }

    protected override void InitializeValues()
    {
        m_IsActive=isActive;
        SetActive(isActive);
    }

    protected override void ApplyColors()
    {
        var color = m_IsActive ? MusicMateColor.Accent : MusicMateColor.Text;
        
        ChangeColor(color, m_Text);
        ChangeColor(MusicMateColor.Panel, _background);
    }
    #endregion

    public void SetActive(bool activate)
    {
        m_IsActive = activate;    
        ApplyColors();
    }
    void OnButtonClicked()
    {
        Animations.Button.PlayClicked(this);
        OnTabItemClick?.Invoke();
    }

    #region Pointer Event Handlers (Handles pointer hover events)

    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClicked();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Animations.Button.PlayHoverEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Animations.Button.PlayHoverExit(this);
    }
    #endregion

    #region Editor-Specific Code

#if UNITY_EDITOR
    void OnValidate()
    {
        EditorApplication.delayCall += OnValidateDelayed;
    }

    // Suppress message: "SendMessage cannot be called during Awake, CheckConsistency, or OnValidate�  
    void OnValidateDelayed()
    {
        if (Application.isPlaying || EditorApplication.isCompiling)
            return;

        InitializeComponents();
        
        // Prevent Object Reference error when loading the project and the colors scriptable is not loaded yet.
        if (!Manager.AppConfiguration) return;
        
        if (m_Text)
            m_Text.text = text;

        SetActive(isActive);
        ApplyColors();
    }
#endif

    #endregion
}