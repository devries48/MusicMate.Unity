#region Usings
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion
public class ToolbarButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Toolbar Button")]
    [SerializeField] Sprite _icon;
    [SerializeField] string _tooltip;

    [Header("Toggle")]
    [SerializeField] bool _isToggleButon;
    [SerializeField] bool _isToggleOn;

    public UnityEvent OnButtonClick { get; private set; } = new UnityEvent();

    public bool IsSpinning { get; set; }

    public bool IsToggleOn { get { return _isToggleOn; } private set { _isToggleOn = value; } }

    public bool CanShowSpinner => IsSpinning && !m_spinner.isActiveAndEnabled;

    public bool CanHideSpinner => !IsSpinning && m_spinner.isActiveAndEnabled;

    #region Field Declarations
    IMusicMateManager _manager;
    AnimationManager _animations;

    internal ButtonInteractable m_button;
    internal ButtonAnimator m_animator;
    internal Image m_icon;
    internal Image m_toggleIcon;
    internal Image m_spinnerBackground;
    internal Image m_spinner;

    internal RectTransform m_tooltipPanel;
    internal TextMeshProUGUI m_tooltipText;
    internal bool m_tooltipVisible;
    float _tooltipPadding = 40f;

    Coroutine _tooltipShowCoroutine;
    Coroutine _tooltipHideCoroutine;

    readonly float _speed = 1.5f;
    readonly float _tooltipDelay = .05f;
    readonly float _popupTime = .1f;
    #endregion

    #region Unity Events
    void OnEnable() => m_animator.OnButtonClick.AddListener(() => OnButtonClick?.Invoke());

    void OnDisable() => m_animator.OnButtonClick.RemoveAllListeners();

    void Awake() => InitializeComponents();

    void Start() => InitializeValues();

    void Update()
    {
        if(IsSpinning)
            m_spinner.transform.Rotate(new Vector3(0, 0, -1 * _speed));
    }
    #endregion

    void InitializeComponents()
    {
        _manager = MusicMateManager.Instance;
        _animations = AnimationManager.Instance;

        transform.Find("Button").TryGetComponent(out m_button);
        transform.Find("Button/Icon").TryGetComponent(out m_icon);
        transform.Find("Button/Background").TryGetComponent(out m_spinnerBackground);
        transform.Find("Button/Spinner").TryGetComponent(out m_spinner);
        transform.Find("Toggle Icon").TryGetComponent(out m_toggleIcon);
        transform.Find("Tooltip").TryGetComponent(out m_tooltipPanel);
        transform.Find("Tooltip/Text (TMP)").TryGetComponent(out m_tooltipText);

        m_animator = GetComponentInChildren<ButtonAnimator>();
        m_tooltipText = m_tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
    }

    void InitializeValues()
    {
        if (m_icon == null)
            Debug.Log("ICON=0");
        else
            m_icon.sprite = _icon;
        m_tooltipText.text = _tooltip;
        m_tooltipPanel.sizeDelta = new Vector2(
            m_tooltipText.preferredWidth + (2 * _tooltipPadding),
            m_tooltipPanel.sizeDelta.y);

        SetToggle();
    }

    public void ShowSpinner() => _animations.ToolbarButtonSpinner(this, true);

    public void HideSpinner() => _animations.ToolbarButtonSpinner(this, false);

    public void SetToggle(bool toggle)
    {
        IsToggleOn = toggle;
        if(this.gameObject.activeInHierarchy)
            SetToggle();
    }

    void SetToggle() => _animations.ToolbarButtonToggle(this, IsToggleOn);

    public void SetInterActable(bool interactable) => m_animator.Button.interactable = interactable;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_tooltipHideCoroutine != null)
        {
            StopCoroutine(_tooltipHideCoroutine);
            _tooltipHideCoroutine = null;
        }

        if(!string.IsNullOrEmpty(_tooltip) && !m_tooltipVisible)
            _tooltipShowCoroutine = StartCoroutine(ShowTooltipWithDelay(_tooltipDelay));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(_tooltipShowCoroutine != null)
        {
            StopCoroutine(_tooltipShowCoroutine);
            _tooltipShowCoroutine = null;
        }

        if(m_tooltipVisible)
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

        _animations.ToolbarButtonTooltip(this, true);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        InitializeComponents();
        InitializeValues();
    }
#endif
}
