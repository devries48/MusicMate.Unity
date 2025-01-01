using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : MusicMateBehavior
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

    static readonly string encryptionKey = "YourEncryptionKeyHere";

    protected override void RegisterEventHandlers()
    {
        _cancelButton.onClick.AddListener(OnCancelClicked);
        _acceptButton.onClick.AddListener(OnAcceptClicked);
   
        _inputUrl.ValueTextChanged.AddListener(OnInputChanged);
        _inputUser.ValueTextChanged.AddListener( OnInputChanged);
        _inputPassword.ValueTextChanged.AddListener(OnInputChanged);
    }

    protected override void UnregisterEventHandlers()
    {
        _cancelButton.onClick.RemoveListener(OnCancelClicked);
        _acceptButton.onClick.RemoveListener(OnAcceptClicked);

        _inputUrl.ValueTextChanged.RemoveListener(OnInputChanged);
        _inputUser.ValueTextChanged.RemoveListener(OnInputChanged);
        _inputPassword.ValueTextChanged.RemoveListener(OnInputChanged);
    }

    protected override void InitializeValues()
    {
        var cfg = Manager.AppConfiguration;

        _inputUrl.ValueText = cfg.ApiServiceUrl;
        _inputUser.ValueText = cfg.User;
        _inputPassword.ValueText = cfg.GetPassword(encryptionKey);

        if (!_inputUser.HasValue)
            _inputUser.SetFocus();
        else if (!_inputPassword.HasValue)
            _inputPassword.SetFocus();
        else
            _inputUrl.SetFocus();

        OnInputChanged();
    }

    void OnInputChanged()
    {
        var hasValue = _inputUrl.HasValue && _inputUser.HasValue && _inputPassword.HasValue;
        if (_acceptButton.interactable == hasValue)
            return;

        _acceptButton.interactable = hasValue;
    }

    void OnCancelClicked() => Manager.QuitApplication();

    void OnAcceptClicked() => Manager.Connect();
}
