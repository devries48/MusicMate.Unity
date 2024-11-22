using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindowController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] Image _loginImage;
    [SerializeField] Button _acceptButton;
    [SerializeField] Button _cancelButton;

    IMusicMateManager _manager;

    void Awake()
    {
        _manager = MusicMateManager.Instance;
    }

    void Start()
    {
        _cancelButton.onClick.AddListener(() => OnCancelClicked());
    }

    void OnCancelClicked()
    {
        _manager.QuitApplication();
    }

}
