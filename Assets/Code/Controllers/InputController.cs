using TMPro;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] TextMeshProUGUI _label;
    [SerializeField] TMP_InputField _inputTextField;

    [Header("Values")]
    [SerializeField] string _labelText;


     void Start()
    {
        _label.text = _labelText;
    }

    public string ValueText { get => _inputTextField.text; set => _inputTextField.text = value; }

    public bool HasValue() => !string.IsNullOrWhiteSpace(_inputTextField.text);

    public void SetFocus() => _inputTextField.ActivateInputField();


#if UNITY_EDITOR
    void OnValidate()
    {
        _label.text = _labelText;
    }
#endif
}
