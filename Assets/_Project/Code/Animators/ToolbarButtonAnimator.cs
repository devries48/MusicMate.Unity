#region Usings
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor.Experimental.GraphView;

#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

public class ToolbarButtonAnimator : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Serialized Fields
    [SerializeField] bool _interactable;
    [SerializeField] ToolbarButtonType _buttonType;
    [SerializeField] Sprite _icon;
    [SerializeField] string _text;
    [SerializeField] string _tooltip;
    [SerializeField] bool _isToggleOn;
    [SerializeField, Tooltip("The button is part of a group where only one button can be toggled.")] bool _isToggleGroup;
    #endregion

    #region Properties
    public UnityEvent OnButtonClick { get; private set; } = new UnityEvent();

    public bool IsInteractable
    {
        get => _interactable;
        set
        {
            if (value == _interactable)
            {
                _interactable = value;
                OnInteractableChanged(value);
            }
        }
    }

    public bool IsToggleOn { get => _isToggleOn; private set { _isToggleOn = value; } }
    public bool IsToggleGroup { get  => _isToggleGroup; }
    public bool IsTextToggle { get => _buttonType == ToolbarButtonType.ToggleText; }

    public bool IsSpinning { get; set; }
    public bool CanShowSpinner => IsSpinning && !m_spinner.isActiveAndEnabled;
    public bool CanHideSpinner => !IsSpinning && m_spinner.isActiveAndEnabled;
    #endregion

    #region Field Declarations
    internal Image m_icon;
    internal Image m_toggleIcon;
    internal Image m_background;
    internal Image m_spinner;
    internal TextMeshProUGUI m_text;

    internal RectTransform m_tooltipPanel;
    internal Image m_tooltipBackground;
    internal TextMeshProUGUI m_tooltipText;
    internal bool m_tooltipVisible;

    Coroutine _tooltipShowCoroutine;
    Coroutine _tooltipHideCoroutine;

    bool _isInitialized = false;

    readonly float _speed = 1.5f;
    #endregion

    #region Base Class Methods

    protected override void InitializeComponents()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        if (_buttonType == ToolbarButtonType.ToggleText)
            transform.Find("Button/Text").TryGetComponent(out m_text);
        else
        {
            transform.Find("Button/Spinner").TryGetComponent(out m_spinner);
            transform.Find("Button/Icon").TryGetComponent(out m_icon);
        }

        transform.Find("Button/Background").TryGetComponent(out m_background);
        transform.Find("Toggle Icon").TryGetComponent(out m_toggleIcon);
        transform.Find("Tooltip").TryGetComponent(out m_tooltipPanel);
        transform.Find("Tooltip").TryGetComponent(out m_tooltipBackground);
        transform.Find("Tooltip/Text (TMP)").TryGetComponent(out m_tooltipText);

        m_tooltipText = m_tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    protected override void InitializeValues()
    {
        if (m_icon != null)
            m_icon.sprite = _icon;

        if (m_text != null)
            m_text.text = _text;

        m_tooltipText.text = _tooltip;

        var tooltipWidth = m_tooltipText.preferredWidth + (2 * Animations.TooltipPadding);
        var tooltipHeight = m_tooltipText.preferredHeight;

        int numberOfLines = Mathf.CeilToInt(tooltipWidth / Animations.TooltipPanelWidth);
        float newHeight = numberOfLines * tooltipHeight + (2 * Animations.TooltipPadding);

        m_tooltipPanel.sizeDelta = new Vector2(Animations.TooltipPanelWidth, newHeight);

        IsInteractable = _interactable;

        if (_isToggleOn && (_buttonType == ToolbarButtonType.Toggle || _buttonType == ToolbarButtonType.ToggleText))
            SetToggle();
    }

    protected override void ApplyColors()
    {
        if (_buttonType == ToolbarButtonType.ToggleText)
        {
            ChangeState(_interactable, m_background);
            ChangeColor(MusicMateColor.Panel, m_text);
        }
        else
        {
            if (m_icon != null)
                ChangeState(_interactable, m_icon);
            if (m_text != null)
                ChangeState(_interactable, m_text);
        }

        ChangeColor(MusicMateColor.Accent, m_toggleIcon, m_spinner);
        ChangeColor(MusicMateColor.Background, m_tooltipBackground);
        ChangeColor(MusicMateColor.Text, m_tooltipText);
    }
    #endregion

    void Update()
    {
        if (IsSpinning)
            m_spinner.transform.Rotate(new Vector3(0, 0, -1 * _speed));
    }

    public void ShowSpinner() => Animations.Toolbar.PlayShowSpinner(this);

    public void HideSpinner() => Animations.Toolbar.PlayHideSpinner(this);

    public void SetToggle(bool toggle)
    {
        if (toggle != IsToggleOn)
        {
            IsToggleOn = toggle;
            SetToggle();
        }
    }

    void OnButtonClicked()
    {
        if (_buttonType != ToolbarButtonType.Toggle && _buttonType != ToolbarButtonType.Spinner)
            Animations.Toolbar.PlayClicked(this);

        if (_buttonType == ToolbarButtonType.Toggle || _buttonType == ToolbarButtonType.ToggleText)
        {
            IsToggleOn = !IsToggleOn;
            SetToggle();
        }

        OnButtonClick?.Invoke();
    }

    void OnInteractableChanged(bool isInteractable)
    {
        if (!IsToggleOn && !IsSpinning)
            Animations.Toolbar.PlayInteractableChanged(this, isInteractable);
    }

    /// <summary>
    /// Toggles the state of the button. 
    /// Ensures all required components are initialized before performing the toggle action.
    /// </summary>
    void SetToggle()
    {
        if (!_isInitialized)
            InitializeComponents();

        if (IsToggleOn)
            Animations.Toolbar.PlayToggleOn(this);
        else
            Animations.Toolbar.PlayToggleOff(this);
    }

    #region Pointer Event Handlers (Handles pointer hover events)

    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClicked();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isToggleGroup || !IsToggleOn)
            Animations.Toolbar.PlayHoverEnter(this);

        if (_tooltipHideCoroutine != null)
        {
            StopCoroutine(_tooltipHideCoroutine);
            _tooltipHideCoroutine = null;
        }

        if (IsInteractable && !string.IsNullOrEmpty(_tooltip) && !m_tooltipVisible)
            _tooltipShowCoroutine = StartCoroutine(ShowTooltipWithDelay(Animations.TooltipDelay));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Animations.Toolbar.PlayHoverExit(this);

        if (_tooltipShowCoroutine != null)
        {
            StopCoroutine(_tooltipShowCoroutine);
            _tooltipShowCoroutine = null;
        }

        if (m_tooltipVisible)
            _tooltipHideCoroutine = StartCoroutine(HideTooltipWithDelay(Animations.TooltipDelay));
    }

    IEnumerator ShowTooltipWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Animations.Toolbar.PlayShowTooltip(this);
    }

    IEnumerator HideTooltipWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Animations.Toolbar.PlayHideTooltip(this);
    }
    #endregion

    #region Editor-Specific Code
#if UNITY_EDITOR
    void OnValidate() { EditorApplication.delayCall += OnValidateDelayed; }

    // Suppress message: "SendMessage cannot be called during Awake, CheckConsistency, or OnValidate”  
    void OnValidateDelayed()
    {
        if (Application.isPlaying) return;
        if (EditorApplication.isCompiling) return;

        // Buttons cannot be toggled when disabled
        if (!_interactable && _isToggleOn)
            _isToggleOn = false;

        if (_buttonType != ToolbarButtonType.Toggle && _buttonType != ToolbarButtonType.ToggleText)
            _isToggleOn = false;

        // Prevent Object Reference error when loading the project and the colors scriptable is not loaded yet.
        if (m_icon != null && Manager.AppConfiguration != null)
        {
            m_icon.sprite = _icon;
            ApplyColors();
        }
    }

#endif
    #endregion
}
