#region Usings
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

public class ToolbarButtonAnimator : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler
{
    #region Serialized Fields
    [SerializeField] bool _interactable;
    [SerializeField] Sprite _icon;
    [SerializeField] string _tooltip;

    [Header("Toggle")]
    [SerializeField] bool _isToggleButton;
    [SerializeField] bool _isToggleOn;
    [SerializeField, Tooltip("The button is part of a group where only one button can be toggled.")] bool _isToggleGroup;

    [Header("Spinner")]
    [SerializeField] bool _isSpinnerButton;
    #endregion

    #region Properties
    public UnityEvent OnButtonClick { get; private set; } = new UnityEvent();

    public bool IsSpinning { get; set; }

    public bool IsInteractable
    {
        get => m_button.interactable;
        private set => m_button.interactable = value;
    }

    public bool IsToggleOn { get => _isToggleOn; private set { _isToggleOn = value; } }

    public bool CanShowSpinner => IsSpinning && !m_spinner.isActiveAndEnabled;

    public bool CanHideSpinner => !IsSpinning && m_spinner.isActiveAndEnabled;
    #endregion

    #region Field Declarations
    internal ButtonInteractable m_button;
    internal Image m_icon;
    internal Image m_toggleIcon;
    internal Image m_spinnerBackground;
    internal Image m_spinner;

    internal RectTransform m_tooltipPanel;
    internal TextMeshProUGUI m_tooltipText;
    internal bool m_tooltipVisible;

    Coroutine _tooltipShowCoroutine;
    Coroutine _tooltipHideCoroutine;

    bool _isInitialized = false;

    readonly float _speed = 1.5f;

    readonly ButtonAnimationType _buttonType = ButtonAnimationType.ToolbarButton;
    #endregion

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        base.RegisterEventHandlers();

        m_button.onClick.AddListener(OnButtonClicked);
        m_button.OnInteractableChanged += OnInteractableChanged;
    }

    protected override void UnregisterEventHandlers()
    {
        base.UnregisterEventHandlers();

        m_button.onClick.RemoveListener(OnButtonClicked);
        m_button.OnInteractableChanged -= OnInteractableChanged;
    }

    protected override void InitializeComponents()
    {
        if (_isInitialized) return;
        _isInitialized = true;

        transform.Find("Button").TryGetComponent(out m_button);
        transform.Find("Button/Icon").TryGetComponent(out m_icon);
        transform.Find("Button/Background").TryGetComponent(out m_spinnerBackground);
        transform.Find("Button/Spinner").TryGetComponent(out m_spinner);
        transform.Find("Toggle Icon").TryGetComponent(out m_toggleIcon);
        transform.Find("Tooltip").TryGetComponent(out m_tooltipPanel);
        transform.Find("Tooltip/Text (TMP)").TryGetComponent(out m_tooltipText);

        m_tooltipText = m_tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    protected override void InitializeValues()
    {
        if (m_icon != null)
        {
            m_icon.sprite = _icon;
            m_icon.color = _interactable ? m_button.Colors.IconColor : m_button.Colors.DisabledIconColor;
        }

        m_tooltipText.text = _tooltip;

        var tooltipWidth = m_tooltipText.preferredWidth + (2 * Animations.TooltipPadding);
        var tooltipHeight = m_tooltipText.preferredHeight;

        int numberOfLines = Mathf.CeilToInt(tooltipWidth / Animations.TooltipPanelWidth);
        float newHeight = numberOfLines * tooltipHeight + (2 * Animations.TooltipPadding);

        m_tooltipPanel.sizeDelta = new Vector2(Animations.TooltipPanelWidth, newHeight);

        SetInteractable(_interactable);

        if (_isToggleButton && _isToggleOn)
            SetToggle();
    }
    #endregion

    void Update()
    {
        if (IsSpinning)
            m_spinner.transform.Rotate(new Vector3(0, 0, -1 * _speed));
    }

    public void ShowSpinner() => Animations.ToolbarButtonSpinner(this, true);

    public void HideSpinner() => Animations.ToolbarButtonSpinner(this, false);

    public void SetToggle(bool toggle)
    {
        if (toggle != IsToggleOn)
        {
            IsToggleOn = toggle;
            SetToggle();
        }
    }

    public void SetInteractable(bool interactable) => m_button.interactable = interactable;

    void OnButtonClicked()
    {
        if (_isToggleButton)
        {
            IsToggleOn = !IsToggleOn;
            SetToggle();
        }

        if (!_isToggleButton && !_isSpinnerButton)
            Animations.ButtonClicked(m_button, _buttonType);

        OnButtonClick?.Invoke();
    }

    void OnInteractableChanged(bool isInteractable)
    {
        if (!IsToggleOn && !IsSpinning)
            Animations.ButtonInteractableChanged(m_button, isInteractable, false, _buttonType);
    }

    /// <summary>
    /// Toggles the state of the button. 
    /// Ensures all required components are initialized before performing the toggle action.
    /// </summary>
    void SetToggle()
    {
        if (!_isInitialized)
            InitializeComponents();

        Animations.ToolbarButtonToggle(this, IsToggleOn);
    }

    #region Pointer Event Handlers (Handles pointer hover events)
    public void OnPointerEnter(PointerEventData eventData)
    {
        Animations.ButtonHoverEnter(m_button, _buttonType);

        if (_tooltipHideCoroutine != null)
        {
            StopCoroutine(_tooltipHideCoroutine);
            _tooltipHideCoroutine = null;
        }

        if (m_button.interactable && !string.IsNullOrEmpty(_tooltip) && !m_tooltipVisible)
            _tooltipShowCoroutine = StartCoroutine(ShowTooltipWithDelay(Animations.TooltipDelay));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Animations.ButtonHoverExit(m_button, _buttonType);

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

        Animations.ToolbarButtonTooltip(this, true);
    }

    IEnumerator HideTooltipWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Animations.ToolbarButtonTooltip(this, false);
    }
    #endregion

    #region Editor-Specific Code
#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying) return;
        if (EditorApplication.isCompiling) return;

        transform.Find("Button").TryGetComponent(out m_button);
        transform.Find("Button/Icon").TryGetComponent(out m_icon);

        // Buttons cannot be toggled when disabled
        if (m_button != null)
        {
            if (!_interactable && _isToggleOn)
                _isToggleOn = false;

            if (!_isToggleButton)
            {
                _isToggleOn = false;
                _isToggleGroup = false;
            }
            else
                _isSpinnerButton = false;
        }

        if (m_icon != null)
        {
            m_icon.sprite = _icon;
            m_icon.color = _interactable ? m_button.Colors.IconColor : m_button.Colors.DisabledIconColor;
        }
    }
#endif
    #endregion
}
