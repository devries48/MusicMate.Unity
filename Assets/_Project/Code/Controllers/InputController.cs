using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputController : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string _labelText;

    [Header("Elements")]
    [SerializeField] TextMeshProUGUI _label;
    [SerializeField] TMP_InputField _inputTextField;

    [HideInInspector]
    public UnityEvent ValueTextChanged;

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        _inputTextField.onSelect.AddListener(OnSelect);
        _inputTextField.onDeselect.AddListener(OnDeselect);
    }

    protected override void UnregisterEventHandlers()
    {
        _inputTextField.onSelect.RemoveListener(OnSelect);
        _inputTextField.onDeselect.RemoveListener(OnDeselect);
    }

    protected override void InitializeValues()
    {
        _label.text = _labelText;
        _inputTextField.onValueChanged.AddListener(delegate { OnValueTextChanged(); });
        Animations.Input.PlayTextNormal(_inputTextField);
    }

    protected override void ApplyColors()
    {
        ChangeColor(MusicMateColor.Text, _label);
    }
    #endregion

    public string ValueText { get => _inputTextField.text; set => _inputTextField.text = value; }

    public bool HasValue => !string.IsNullOrWhiteSpace(_inputTextField.text);

    public void SetFocus() => _inputTextField.ActivateInputField();

    public void OnValueTextChanged() => ValueTextChanged?.Invoke();

    // Animations 
    // ==========
    void OnSelect(string _) => Animations.Input.PlayTextSelect(_inputTextField);

    void OnDeselect(string _) => Animations.Input.PlayTextNormal(_inputTextField);

    public void OnPointerEnter(PointerEventData eventData) => Animations.Input.PlayTextHighlight(_inputTextField);

    public void OnPointerExit(PointerEventData eventData) => Animations.Input.PlayTextNormal(_inputTextField);
    // ==========

#if UNITY_EDITOR
    void OnValidate()
    {
        _label.text = _labelText;
    }
#endif
}
