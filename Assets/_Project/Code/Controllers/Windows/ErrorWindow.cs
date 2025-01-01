using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorWindow : MusicMateBehavior
{
    #region Serialized Fields
    [Header("Elements")]
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _descriptionText;
    [SerializeField] TextMeshProUGUI _errorText;
    [SerializeField] Image _warningImage;
    [SerializeField] ButtonInteractable _acceptButton;
    [SerializeField] Button _cancelButton;
    #endregion

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        _cancelButton.onClick.AddListener(OnCancelClicked);
        _acceptButton.onClick.AddListener(OnAcceptClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        _cancelButton.onClick.RemoveListener(OnCancelClicked);
        _acceptButton.onClick.RemoveListener(OnAcceptClicked);
    }
    #endregion

    public void SetError(ErrorType error, string message, string description = null)
    {
        if (error == ErrorType.Connection)
        {
            _descriptionText.text = description ?? string.Empty;
        }
        _errorText.text = message;
    }

    void OnCancelClicked() => Manager.QuitApplication();
    void OnAcceptClicked() => Manager.ShowLogin();
}
