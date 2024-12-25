#region Usings
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion
public class ToolbarButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button")]
    [SerializeField] bool _interactable;
    [SerializeField] Sprite _icon;
    [SerializeField] string _tooltip;

    [Header("Toggle")]
    [SerializeField] bool _isToggleButon;
    [SerializeField] bool _isToggleOn;
    [SerializeField, Tooltip("The button is part of a group where only one button can be toggled.")] bool _isToggleGroup;

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
    AnimationManager _animations;

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

    readonly float _tooltipDelay = .05f;
    readonly float _speed = 1.5f;

    readonly ButtonAnimationType _buttonType = ButtonAnimationType.ToolbarButton;
    #endregion

    #region Unity Events
    void OnEnable()
    {
        m_button.onClick.AddListener(() => OnButtonClicked());
        m_button.OnInteractableChanged += OnInteractableChanged;
    }

    void OnDisable()
    {
        m_button.onClick.RemoveListener(() => OnButtonClicked());
        m_button.OnInteractableChanged -= OnInteractableChanged;
    }

    void Awake()
    {
        _animations = AnimationManager.Instance;

        InitializeComponents();
    }

    void Start() => InitializeValues();

    void Update()
    {
        if (IsSpinning)
            m_spinner.transform.Rotate(new Vector3(0, 0, -1 * _speed));
    }
    #endregion

    void InitializeComponents()
    {
        transform.Find("Button").TryGetComponent(out m_button);
        transform.Find("Button/Icon").TryGetComponent(out m_icon);
        transform.Find("Button/Background").TryGetComponent(out m_spinnerBackground);
        transform.Find("Button/Spinner").TryGetComponent(out m_spinner);
        transform.Find("Toggle Icon").TryGetComponent(out m_toggleIcon);
        transform.Find("Tooltip").TryGetComponent(out m_tooltipPanel);
        transform.Find("Tooltip/Text (TMP)").TryGetComponent(out m_tooltipText);

        m_tooltipText = m_tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    void InitializeValues()
    {
        if (m_icon != null)
        {
            m_icon.sprite = _icon;
            m_icon.color = IsInteractable ? m_button.Colors.IconColor : m_button.Colors.DisabledIconColor;
        }

        m_tooltipText.text = _tooltip;

        var tooltipWidth = m_tooltipText.preferredWidth + (2 * _animations.TooltipPadding);
        var tooltipHeight = m_tooltipText.preferredHeight;

        int numberOfLines = Mathf.CeilToInt(tooltipWidth / _animations.TooltipPanelWidth);
        float newHeight = numberOfLines * tooltipHeight + (2 * _animations.TooltipPadding);

        m_tooltipPanel.sizeDelta = new Vector2(_animations.TooltipPanelWidth, newHeight);

        SetInterActable(_interactable);
        SetToggle();
    }

    public void ShowSpinner() => _animations.ToolbarButtonSpinner(this, true);

    public void HideSpinner() => _animations.ToolbarButtonSpinner(this, false);

    public void SetToggle(bool toggle)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (toggle != IsToggleOn)
        {
            IsToggleOn = toggle;
            SetToggle();
        }
    }

    public void SetInterActable(bool interactable) => m_button.interactable = interactable;

    /// <summary>
    /// Animate the button toggle state.
    /// </summary>
    /// <remarks>
    /// Do not execute when button is not interactable and toggled off.
    /// </remarks>
    void SetToggle()
    {
        if (IsToggleOn || m_button.interactable)
            _animations.ToolbarButtonToggle(this, IsToggleOn);
    }

    void OnInteractableChanged(bool isInteractable)
    {
        if (!IsToggleOn && !IsSpinning)
            _animations.ButtonInteractableChanged(m_button, isInteractable, false, _buttonType);
    }

    void OnButtonClicked()
    {
        if (_isToggleButon)
        {
            IsToggleOn = !IsToggleOn;
            SetToggle();
        }

        OnButtonClick?.Invoke();
        _animations.ButtonClicked(m_button, _buttonType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _animations.ButtonHoverEnter(m_button, _buttonType);

        if (_tooltipHideCoroutine != null)
        {
            StopCoroutine(_tooltipHideCoroutine);
            _tooltipHideCoroutine = null;
        }

        if (m_button.interactable && !string.IsNullOrEmpty(_tooltip) && !m_tooltipVisible)
            _tooltipShowCoroutine = StartCoroutine(ShowTooltipWithDelay(_tooltipDelay));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _animations.ButtonHoverExit(m_button, _buttonType);

        if (_tooltipShowCoroutine != null)
        {
            StopCoroutine(_tooltipShowCoroutine);
            _tooltipShowCoroutine = null;
        }

        if (m_tooltipVisible)
            _tooltipHideCoroutine = StartCoroutine(HideTooltipWithDelay(_tooltipDelay));
    }

    IEnumerator ShowTooltipWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _animations.ToolbarButtonTooltip(this, true);
    }

    IEnumerator HideTooltipWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _animations.ToolbarButtonTooltip(this, false);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        transform.Find("Button").TryGetComponent(out m_button);
        transform.Find("Button/Icon").TryGetComponent(out m_icon);
        
        // Buttons cannot be toggled when disabled
        if (m_button != null)
        {
            if (!_interactable && _isToggleOn)
                _isToggleOn = false;

            if (!_isToggleButon)
            {
                _isToggleOn = false;
                _isToggleGroup = false;
            }
        }

        if (m_icon != null)
        {
            m_icon.sprite = _icon;
            m_icon.color = _interactable ? m_button.Colors.IconColor : m_button.Colors.DisabledIconColor;
        }
    }
#endif
}
