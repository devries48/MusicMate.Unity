using System;

public interface IMusicMateManager
{
    AppSetings AppConfiguration { get; }
    IAppState AppState { get; }
    IColorSettings AppColors  { get; }

    event Action<MusicMateZone, object> OnEditComplete;

    void Connect();
    void HideSpinner();
    void ShowError(ErrorType error, string message, string description = "");
    void ShowLogin();
    void ShowRelease(ReleaseResult releaseModel);
    void ShowEditor(ZoneAnimator zone);
    void HideEditor();
    void QuitApplication();
}