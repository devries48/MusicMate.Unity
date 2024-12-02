using System;
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
    [SerializeField] Button _acceptButton;
    [SerializeField] Button _cancelButton;

    IMusicMateManager _manager;

    static readonly string encryptionKey = "YourEncryptionKeyHere";

    void Awake()
    {
        _manager = MusicMateManager.Instance;
    }

    void Start()
    {
        _cancelButton.onClick.AddListener(() => OnCancelClicked());
        
        var cfg = _manager.AppConfiguration;
        _inputUrl.ValueText = cfg.ApiServiceUrl;
        _inputUser.ValueText = cfg.User;
        _inputPassword.ValueText = cfg.GetPassword(encryptionKey);
        
    }

    void OnCancelClicked()
    {
        _manager.QuitApplication();
    }

}
