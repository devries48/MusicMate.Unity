using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The central manager for the MusicMate application. Handles configuration, initialization,
/// and high-level application events such as connecting to the API or showing panels.
/// </summary>
public class MusicMateManager : SceneSingleton<MusicMateManager>, IMusicMateManager
{
    #region Serialized Fields
    //[Header("Configuration")]
    [SerializeField] AppSetings _appSettings;

    //[Header("Windows")]
    [SerializeField] ErrorWindow _errorController;
    [SerializeField] LoginWindow _loginController;
    
    //[Header("Animators")]
    [SerializeField] MainWindowAnimator _mainPage;
    [SerializeField] LogoAnimator _logoAnimator;

    //[Header("Elements")]
    [SerializeField] GameObject _connectionSpinner;
    [SerializeField] GameObject[] _activateOnStart;
    [SerializeField] GameObject[] _inactivateOnStart;
    #endregion

    #region Properties

    /// <summary>
    /// Gets the application's configuration settings.
    /// </summary>
    public AppSetings AppConfiguration => _appSettings;

    /// <summary>
    /// The state of the application, including the current visible part and other runtime information.
    /// </summary>
    public IAppState AppState
    {
        get
        {
            _appState ??= new AppState(_appSettings);
            return _appState;
        }
    }

    public Color32 AccentColor => _appSettings.Colors.AccentColor;
    public Color32 DefaultColor => _appSettings.Colors.DefaultColor;
    public Color32 AccentTextColor => _appSettings.Colors.BackgroundColor;
    public Color32 TextColor => _appSettings.Colors.TextColor;
    public Color32 BackgroundColor => _appSettings.Colors.BackgroundColor;
    #endregion

    #region #region Field Declarations
    IMusicMateApiService _service;
    IAppState _appState;
    AnimationManager _animations;
    #endregion

    #region Unity Events
    void Awake()
    {
        InactivateGameObjects();
        ActivateGameObjects();

        _service = MusicMateApiService.Instance.GetClient();
        _animations = AnimationManager.Instance;
    }

    void OnEnable() => _service.SubscribeToConnectionChanged(OnConnectionChanged);

    void OnDisable() => _service.UnsubscribeFromConnectionChanged(OnConnectionChanged);

    void Start() => StartCoroutine(DelayAndConnect(.5f));
    #endregion

    public void Connect()
    {
        if (_loginController.gameObject.activeInHierarchy)
            ShowOrHideLoginPanel(false);

        if (!_connectionSpinner.gameObject.activeInHierarchy)
            ShowSpinner();

        _service.SignIn(_appSettings.ApiServiceUrl, "admin", "123");
    }

    public void ShowError(ErrorType error, string message, string description = "")
    {
        _errorController.SetError(error, message, description);
        ShowOrHideErrorPanel(true);
    }

    public void ShowLogin()
    {
        float delay = 0f;
        if (_errorController.gameObject.activeSelf)
        {
            ShowOrHideErrorPanel(false);
            delay = .5f;
        }

        ShowOrHideLoginPanel(true, delay);
    }

    public void ShowRelease(ReleaseResult releaseModel)
    {
        _mainPage.ShowRelease(releaseModel);
        AppState.ChangeVisiblePart(VisiblePart.ReleaseDetails);
    }

    void ShowSpinner()
    {
        _connectionSpinner.SetActive(true);

        var images = _connectionSpinner.GetComponentsInChildren<Image>(true);
        var seq = DOTween.Sequence();

        for (int i = 0; i < images.Length; ++i)
        {
            var img = images[i];
            seq.Join(img.DOFade(1f, .2f));
        }
    }

    public void HideSpinner()
    {
        var images = _connectionSpinner.GetComponentsInChildren<Image>(true);
        var seq = DOTween.Sequence();

        for (int i = 0; i < images.Length; ++i)
        {
            var img = images[i];
            seq.Join(img.DOFade(0f, 1f));
        }
        seq.OnComplete(()=>_connectionSpinner.SetActive(false));
    }

    void HideLogo(bool quit = false) => _logoAnimator.HideLogo(() =>
    {
        if (quit)
            QuitApp();

    });


    public void QuitApplication()
    {
        if (_errorController.gameObject.activeInHierarchy)
            ShowOrHideErrorPanel(false);
        else if (_loginController.gameObject.activeInHierarchy)
            ShowOrHideLoginPanel(false);

        if (_logoAnimator.IsLogoActive())
            HideLogo(true);
        else
            QuitApp();
    }


    /// <summary>
    /// Hide GameObjects initially not shown.
    /// </summary>
    void InactivateGameObjects()
    {
        for (int i = 0; i < _inactivateOnStart?.Length; i++)
            _inactivateOnStart[i].SetActive(false);
    }

    /// <summary>
    /// Show GameObjects initially hidden.
    /// Logo was often set to disabled. 
    /// </summary>
    void ActivateGameObjects()
    {
        for (int i = 0; i < _activateOnStart?.Length; i++)
            _activateOnStart[i].SetActive(true);
    }


    void ShowOrHideErrorPanel(bool show) => _animations.WindowErrorVisible(_errorController.gameObject, show);

    void ShowOrHideLoginPanel(bool show, float delay = 0f) => _animations.WindowLoginVisible(_loginController.gameObject, show, delay);

    IEnumerator DelayAndConnect(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Connect();
    }

    /// <summary>
    /// Application.Quit() does not work in the editor so
    /// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
    /// </summary>
    void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    // 
    void OnConnectionChanged(object sender, ConnectionChangedEventArgs e)
    {
        _mainPage.ConnectionChanged(e.Connected);

        if (!e.Connected)
        {
            ShowError(ErrorType.Connection, e.Error, _appSettings.ApiServiceUrl);
            HideSpinner();
        }
        else
            HideLogo();
    }
}
