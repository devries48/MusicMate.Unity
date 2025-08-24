using DG.Tweening;
using System;
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
    [SerializeField] ErrorWindow _errorWindow;
    [SerializeField] LoginWindow _loginWindow;
    [SerializeField] EditorWindow _editorWindow;
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

    public event Action<MusicMateZone, object> OnEditComplete;

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

    void OnEnable()
    {
        _service.SubscribeToConnectionChanged(OnConnectionChanged);
        _service.SubscribeToApiError(OnErrorOccurred);
    }

    void OnDisable()
    {
        _service.UnsubscribeFromConnectionChanged(OnConnectionChanged);
        _service.UnsubscribeFromApiError(OnErrorOccurred);
    }

    void Start() => StartCoroutine(DelayAndConnect(.5f));
    #endregion

    public void Connect()
    {
        if (_loginWindow.gameObject.activeInHierarchy)
            ShowOrHideLoginPanel(false);

        if (!_connectionSpinner.activeInHierarchy)
            ShowSpinner();

        _service.SignIn(_appSettings.ApiServiceUrl, "admin", "123");
    }

    public void ShowError(ErrorType error, string message, string description = "")
    {
        _errorWindow.SetError(error, message, description);
        ShowOrHideErrorPanel(true);
    }

    public void ShowLogin()
    {
        float delay = 0f;
        if (_errorWindow.gameObject.activeSelf)
        {
            ShowOrHideErrorPanel(false);
            delay = .5f;
        }

        ShowOrHideLoginPanel(true, delay);
    }


    public void ShowEditor(ZoneAnimator zone)
    {
        _editorWindow.OnEditorAccepted += OnEditorAccepted;
        _editorWindow.PanelRect.localPosition = new Vector2(_mainPage.SidePanelExpanded ? -Constants.SidePanelWidth / 2 : 0, 0);
        _editorWindow.SetEditor(zone);

        ShowOrHideEditorWindow(true);
    }

    public void HideEditor()
    {
        _editorWindow.OnEditorAccepted -= OnEditorAccepted;
        ShowOrHideEditorWindow(false);
    }

    void OnEditorAccepted(MusicMateZone zone, object modifiedModel)
    {
        OnEditComplete?.Invoke(zone, modifiedModel);
        Debug.Log($"Editor changes accepted for model: {modifiedModel.GetType().Name}");
    }

    public void ShowRelease(ReleaseResult releaseModel)
    {
        _mainPage.ShowRelease(releaseModel);
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
        seq.OnComplete(() => _connectionSpinner.SetActive(false));
    }

    public void QuitApplication()
    {
        if (_errorWindow.gameObject.activeInHierarchy)
            ShowOrHideErrorPanel(false);
        else if (_loginWindow.gameObject.activeInHierarchy)
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
            Animations.Panel.PlayShowErrorWindow(_errorWindow.gameObject);
        else
            Animations.Panel.PlayHideErrorWindow(_errorWindow.gameObject);
    }

    void ShowOrHideLoginPanel(bool show, float delay = 0f)
    {
        if (show)
            Animations.Panel.PlayShowLoginWindow(_loginWindow.gameObject, delay);
        else
            Animations.Panel.PlayHideLoginWindow(_loginWindow.gameObject);
    }

    void ShowOrHideEditorWindow(bool show)
    {
        Animations.Panel.PlayEditorVisibility(show, _editorWindow);
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
    void OnConnectionChanged(ConnectionChangedEventArgs e)
    {
        _mainPage.ConnectionChanged(e.Connected);

        if (!e.Connected)
        {
            ShowError(ErrorType.Connection, _appSettings.ApiServiceUrl, e.Error);
            HideSpinner();
        }
        else
            HideLogo();
    }

    void OnErrorOccurred(ErrorEventArgs e) => ShowError(e.Error, e.Message, e.Description);

}
