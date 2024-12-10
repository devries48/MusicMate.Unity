using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindowController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] InputController _inputUrl;
    [SerializeField] InputController _inputUser;
    [SerializeField] InputController _inputPassword;

    [Header("Elements")]
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] Image _loginImage;
    [SerializeField] ButtonInteractable _acceptButton;
    [SerializeField] Button _cancelButton;

    IMusicMateManager _manager;

    static readonly string encryptionKey = "YourEncryptionKeyHere";

    void Awake() => _manager = MusicMateManager.Instance;

    void Start() => Initialize();

    void Initialize()
    {
        var cfg = _manager.AppConfiguration;
        _inputUrl.ValueText = cfg.ApiServiceUrl;
        _inputUser.ValueText = cfg.User;
        _inputPassword.ValueText = cfg.GetPassword(encryptionKey);

        if (!_inputUser.HasValue)
            _inputUser.SetFocus();
        else if (!_inputPassword.HasValue)
            _inputPassword.SetFocus();
        else
            _inputUrl.SetFocus();

        _inputUrl.ValueTextChanged.AddListener(() => OnInputChanged());
        _inputUser.ValueTextChanged.AddListener(() => OnInputChanged());
        _inputPassword.ValueTextChanged.AddListener(() => OnInputChanged());

        _cancelButton.onClick.AddListener(() => OnCancelClicked());
        _acceptButton.onClick.AddListener(() => OnAcceptClicked());

        OnInputChanged();
    }

    void OnInputChanged()
    {
        var hasValue = _inputUrl.HasValue && _inputUser.HasValue && _inputPassword.HasValue;
        if (_acceptButton.interactable == hasValue)
            return;

        _acceptButton.interactable = hasValue;
    }

    void OnCancelClicked() => _manager.QuitApplication();

    void OnAcceptClicked() => _manager.Connect();

}
