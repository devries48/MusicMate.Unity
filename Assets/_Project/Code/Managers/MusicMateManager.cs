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
    [SerializeField] AppSetings _appSettings;
    [SerializeField] ErrorWindow _errorController;
    [SerializeField] LoginWindow _loginController;
    [SerializeField] MainWindowAnimator _mainPage;
    [SerializeField] LogoAnimator _logoAnimator;
    [SerializeField] GameObject _connectionSpinner;
    [SerializeField] GameObject[] _activateOnStart;
    [SerializeField] GameObject[] _inactivateOnStart;
    #endregion

    #region Properties
    IAnimationManager Animations
    {
        get
        {
            _animations ??= AnimationManager.Instance;

            return _animations;
        }
    }

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
            _appState ??= new AppState(this);
            return _appState;
        }
    }

    public IColorSettings AppColors => AppState.CurrentColors;
    #endregion

    #region #region Field Declarations
    IAnimationManager _animations;
    IMusicMateApiService _service;
    IAppState _appState;
    #endregion

    #region Unity Events
    void Awake()
    {
        InactivateGameObjects();
        ActivateGameObjects();

        _service = MusicMateApiService.Instance.GetClient();
    }

    void OnEnable() => _service.SubscribeToConnectionChanged(OnConnectionChanged);

    void OnDisable() => _service.UnsubscribeFromConnectionChanged(OnConnectionChanged);

    void Start() => StartCoroutine(DelayAndConnect(.5f));
    #endregion

    public void Connect()
    {
        if (_loginController.gameObject.activeInHierarchy)
            ShowOrHideLoginPanel(false);

        if (!_connectionSpinner.activeInHierarchy)
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
        AppState.InvokeStateChanged(MusicMateStatePart.ReleaseDetails);
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

    void HideLogo(bool quit = false) => _logoAnimator.HideLogo(() =>
    {
        if (quit)
            QuitApp();

    });

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


    void ShowOrHideErrorPanel(bool show)
    {
        if (show)
        Animations.Panel.PlayShowErrorWindow(_errorController.gameObject);
        else
        Animations.Panel.PlayHideErrorWindow(_errorController.gameObject);
    }

    void ShowOrHideLoginPanel(bool show, float delay = 0f)
    {
        if (show)
            Animations.Panel.PlayShowLoginWindow(_errorController.gameObject,delay);
        else
            Animations.Panel.PlayHideLoginWindow(_errorController.gameObject);
    }

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
