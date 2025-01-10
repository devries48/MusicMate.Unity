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

    void Connect();
    void HideSpinner();
    void ShowError(ErrorType error, string message, string description = "");
    void ShowLogin();
    void ShowRelease(ReleaseResult releaseModel);
    void QuitApplication();
}