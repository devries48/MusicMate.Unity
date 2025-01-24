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

public class ToolbarButtonAnimator : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Serialized Fields
    [SerializeField] bool _interactable;
    [SerializeField] Sprite _icon;
    [SerializeField] string _tooltip;

    [Header("Toggle")]
    [SerializeField] bool _isToggleButton;
    [SerializeField] bool _isToggleOn;

    [Header("Spinner")]
    [SerializeField] bool _isSpinnerButton;
    #endregion

    #region Properties
    public UnityEvent OnButtonClick { get; private set; } = new UnityEvent();

    public bool IsSpinning { get; set; }

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

    public bool CanShowSpinner => IsSpinning && !m_spinner.isActiveAndEnabled;

    public bool CanHideSpinner => !IsSpinning && m_spinner.isActiveAndEnabled;
    #endregion

    #region Field Declarations
    internal Image m_icon;
    internal Image m_toggleIcon;
    internal Image m_spinnerBackground;
    internal Image m_spinner;

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

        transform.Find("Button/Icon").TryGetComponent(out m_icon);
        transform.Find("Button/Background").TryGetComponent(out m_spinnerBackground);
        transform.Find("Button/Spinner").TryGetComponent(out m_spinner);
        transform.Find("Toggle Icon").TryGetComponent(out m_toggleIcon);
        transform.Find("Tooltip").TryGetComponent(out m_tooltipPanel);
        transform.Find("Tooltip").TryGetComponent(out m_tooltipBackground);
        transform.Find("Tooltip/Text (TMP)").TryGetComponent(out m_tooltipText);

        m_tooltipText = m_tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    protected override void InitializeValues()
    {
        if (m_icon != null)
        {
            m_icon.sprite = _icon;
            m_icon.color = _interactable ? Manager.AppColors.IconColor : Manager.AppColors.DisabledIconColor;
        }

        m_tooltipText.text = _tooltip;

        var tooltipWidth = m_tooltipText.preferredWidth + (2 * Animations.TooltipPadding);
        var tooltipHeight = m_tooltipText.preferredHeight;

        int numberOfLines = Mathf.CeilToInt(tooltipWidth / Animations.TooltipPanelWidth);
        float newHeight = numberOfLines * tooltipHeight + (2 * Animations.TooltipPadding);

        m_tooltipPanel.sizeDelta = new Vector2(Animations.TooltipPanelWidth, newHeight);

        IsInteractable = _interactable;

        if (_isToggleButton && _isToggleOn)
            SetToggle();
    }

    protected override void ApplyColors()
    {
        ChangeColor(_interactable ? MusicMateColor.Icon : MusicMateColor.DisabledIcon, m_icon);
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
        if (_isToggleButton)
        {
            IsToggleOn = !IsToggleOn;
            SetToggle();
        }

        if (!_isToggleButton && !_isSpinnerButton)
            Animations.Toolbar.PlayClicked(this);

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

        transform.Find("Button/Icon").TryGetComponent(out m_icon);

        // Buttons cannot be toggled when disabled
        if (!_interactable && _isToggleOn)
            _isToggleOn = false;

        if (!_isToggleButton)
            _isToggleOn = false;
        else
            _isSpinnerButton = false;

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
