public interface IMusicMateManager
{
    AppSetings AppConfiguration { get; }
    IAppState AppState { get; }
    IColorSettings AppColors  { get; }

    void Connect();
    void HideSpinner();
    void ShowError(ErrorType error, string message, string description = "");
    void ShowLogin();
    void ShowRelease(ReleaseResult releaseModel);
    void QuitApplication();
}