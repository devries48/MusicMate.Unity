using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorWindowController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _descriptionText;
    [SerializeField] TextMeshProUGUI _errorText;
    [SerializeField] Image _warningImage;
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
        _acceptButton.onClick.AddListener(() => OnAcceptClicked());
    }

    public void SetError(ErrorType error, string message, string description = null)
    {
        if (error == ErrorType.Connection)
        {
            _descriptionText.text = description ?? string.Empty;
        }
        _errorText.text = message;
    }

    void OnCancelClicked() => _manager.QuitApplication();
    void OnAcceptClicked() => _manager.ShowLogin();
}
