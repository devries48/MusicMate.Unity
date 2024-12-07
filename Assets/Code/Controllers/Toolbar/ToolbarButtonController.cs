using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolbarButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button")]
    [SerializeField] Button _button;
    [SerializeField] Image _icon;

    [Header("Toggle")]
    [SerializeField] Image _toggleIcon;
    [SerializeField] bool _isToggle;
    [SerializeField] bool _isToggleOn;

    [Header("Tooltip")]
    [SerializeField] RectTransform _tooltipPanel;
    [SerializeField] TextMeshProUGUI _tooltipText;
    [SerializeField] string _tooltip;

    [Header("Spinner")]
    [SerializeField] Image _spinnerBackground;
    [SerializeField] Image _spinner;

    IMusicMateManager _manager;
    bool _isSpinning;
    readonly float _speed = 1.5f;

    public UnityEvent OnButtonClick { get; private set; } = new UnityEvent();
    public bool CanShowSpinner => _isSpinning && !_spinner.isActiveAndEnabled;
    public bool CanHideSpinner => !_isSpinning && _spinner.isActiveAndEnabled;

    public bool IsSpinning
    {
        get => _isSpinning;
        set => _isSpinning = value;
    }

    public bool IsToggleOn
    {
        get => _isToggleOn;
        private set => _isToggleOn = value;
    }

    Coroutine _tooltipShowCoroutine;
    Coroutine _tooltipHideCoroutine;
    readonly float _tooltipDelay = .05f;
    readonly float _popupTime = .1f;
    bool _tooltipVisible;

    void OnEnable() => _button.onClick.AddListener(() => OnButtonClick?.Invoke());

    void OnDisable() => _button.onClick.RemoveAllListeners();

    void Start()
    {
        float padding = 40f;

        _manager = MusicMateManager.Instance;
        _tooltipText.text = _tooltip;
        _tooltipPanel.sizeDelta = new Vector2(_tooltipText.preferredWidth + (2 * padding), _tooltipPanel.sizeDelta.y);

        SetToggle();
    }

    void Update()
    {
        if (_isSpinning)
            _spinner.transform.Rotate(new Vector3(0, 0, -1 * _speed));
    }

    public void ShowSpinner()
    {
        _icon.transform.DOScale(.7f, .25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _spinnerBackground.gameObject.SetActive(true);
            _spinner.gameObject.SetActive(true);
            _button.interactable = false;
        });
    }

    public void HideSpinner()
    {
        _spinnerBackground.gameObject.SetActive(false);
        _spinner.gameObject.SetActive(false);

        _icon.transform.DOScale(1f, .25f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            _button.interactable = true;
        });
    }

    public void SetToggle(bool toggle)
    {
        _isToggleOn = toggle;
        if (this.gameObject.activeInHierarchy)
            SetToggle();
    }

    void SetToggle()
    {
        var animator = _button.GetComponent<Animator>();
        var scaleTo = _isToggleOn ? .7f : 1;
        var easing = _isToggleOn ? Ease.InBack : Ease.OutBack;

        _icon.transform.DOScale(scaleTo, .25f).SetEase(easing).OnComplete(() =>
            {
                _toggleIcon.gameObject.SetActive(_isToggleOn);

                animator.enabled = !IsToggleOn;
                SetInterActable(!IsToggleOn);

                if (IsToggleOn)
                    _icon.color = _manager.AppConfiguration.AccentColor;
            });
    }

    public void SetInterActable(bool interactable) => _button.interactable = interactable;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipHideCoroutine != null)
        {
            StopCoroutine(_tooltipHideCoroutine);
            _tooltipHideCoroutine = null;
        }

        if (!string.IsNullOrEmpty(_tooltip) && !_tooltipVisible)
            _tooltipShowCoroutine = StartCoroutine(ShowTooltipWithDelay(_tooltipDelay));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipShowCoroutine != null)
        {
            StopCoroutine(_tooltipShowCoroutine);
            _tooltipShowCoroutine = null;
        }

        if (_tooltipVisible)
            _tooltipHideCoroutine = StartCoroutine(HideTooltipWithDelay(_tooltipDelay));
    }

    IEnumerator ShowTooltipWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _tooltipText.color = _button.interactable || IsToggleOn ? _manager.AppConfiguration.AccentColor : _manager.AppConfiguration.AccentColor;

        _tooltipPanel.localScale = Vector3.zero;
        _tooltipPanel.gameObject.SetActive(true);
        _tooltipPanel.DOScale(1, _popupTime);
        _tooltipVisible = true;
    }

    IEnumerator HideTooltipWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _tooltipPanel.DOScale(0, _popupTime).OnComplete(() => _tooltipPanel.gameObject.SetActive(false));
        _tooltipVisible = false;
    }
}
