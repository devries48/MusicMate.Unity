using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorWindow : MusicMateBehavior
{
    #region Serialized Fields
    [Header("Elements")]
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _messageText;
    [SerializeField] TextMeshProUGUI _descriptionText;
    [SerializeField] Image _warningImage;
    [SerializeField] ButtonAnimator _acceptButton;
    [SerializeField] ButtonAnimator _cancelButton;
    [SerializeField] GameObject _modalBackground;
    #endregion

    ErrorType _errorType;

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        _cancelButton.OnButtonClick.AddListener(OnCancelClicked);
        _acceptButton.OnButtonClick.AddListener(OnAcceptClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        _cancelButton.OnButtonClick.RemoveListener(OnCancelClicked);
        _acceptButton.OnButtonClick.RemoveListener(OnAcceptClicked);
    }

    protected override void ApplyColors()
    {
        ChangeColor(MusicMateColor.Accent, _warningImage);
        ChangeColor(MusicMateColor.Accent, _titleText);
        ChangeColor(MusicMateColor.Text, _messageText);
        ChangeColor(MusicMateColor.Text, _descriptionText);
    }
    #endregion

    public void SetError(ErrorType error, string message, string description)
    {
        _errorType = error;

        switch (error)
        {
            case ErrorType.Connection:
                _titleText.text = "Connecting to API Service failed";
                _acceptButton.SetText("Connect");
                break;

            case ErrorType.Api:
                _titleText.text = "Error in API Request";
                _acceptButton.SetText("Close");
                break;

            default:
                break;
        }
        if (error != ErrorType.Connection)
            Animations.Panel.PlayModalBackgroundVisibility(true, _modalBackground);

        _messageText.text = message;
        _descriptionText.text = description;
    }

    void OnCancelClicked()
    {
        if (_errorType != ErrorType.Connection)
            Animations.Panel.PlayModalBackgroundVisibility(false, _modalBackground);

        Manager.QuitApplication();
    }

    void OnAcceptClicked()
    {
        if (_errorType == ErrorType.Connection)
            Manager.ShowLogin();
        else
        {
            Animations.Panel.PlayModalBackgroundVisibility(false, _modalBackground);
            Animations.Panel.PlayHideErrorWindow(gameObject);
        }
    }
}
