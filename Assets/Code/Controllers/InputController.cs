using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string _labelText;

    [Header("Elements")]
    [SerializeField] TextMeshProUGUI _label;
    [SerializeField] TMP_InputField _inputTextField;

    [HideInInspector]
    public UnityEvent ValueTextChanged;

    AnimationManager _animations;

    void Awake() => _animations = AnimationManager.Instance;

    void Start()
    {
        _label.text = _labelText;
        _inputTextField.onValueChanged.AddListener(delegate { OnValueTextChanged(); });
        _animations.InputTextNormal(_inputTextField);
    }

    void OnEnable()
    {
        _inputTextField.onSelect.AddListener(OnSelect);
        _inputTextField.onDeselect.AddListener(OnDeselect);
    }

    void OnDisable()
    {
        _inputTextField.onSelect.RemoveListener(OnSelect);
        _inputTextField.onDeselect.RemoveListener(OnDeselect);
    }

    public string ValueText { get => _inputTextField.text; set => _inputTextField.text = value; }

    public bool HasValue => !string.IsNullOrWhiteSpace(_inputTextField.text);

    public void SetFocus() => _inputTextField.ActivateInputField();

    public void OnValueTextChanged() => ValueTextChanged?.Invoke();

    // Animations 
    // ==========
    void OnSelect(string _) => _animations.InputTextSelect(_inputTextField);

    void OnDeselect(string _) => _animations.InputTextNormal(_inputTextField);

    public void OnPointerEnter(PointerEventData eventData) => _animations.InputTextHighlight(_inputTextField);

    public void OnPointerExit(PointerEventData eventData) => _animations.InputTextNormal(_inputTextField);
    // ==========

#if UNITY_EDITOR
    void OnValidate()
    {
        _label.text = _labelText;
    }
#endif
}
