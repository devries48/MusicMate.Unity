using UnityEngine;

public interface IMusicMateManager
{
    AppSetings AppConfiguration { get; }
    IAppState AppState { get; }

    Color32 AccentColor { get; }
    Color32 DefaultColor { get; }
    Color32 AccentTextColor { get; }
    Color32 TextColor { get; }
    Color32 BackgroundColor { get; }
     Color32 IconColor { get; }
     Color32 DisabledIconColor { get; }

    void Connect();
    void HideSpinner();
    void ShowError(ErrorType error, string message, string description = "");
    void ShowLogin();
    void ShowRelease(ReleaseResult releaseModel);
    void QuitApplication();
}