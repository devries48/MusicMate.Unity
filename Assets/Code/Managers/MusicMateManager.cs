using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicMateManager : SceneSingleton<MusicMateManager>, IMusicMateManager
{
    [Header("Configuration")]
    [SerializeField] AppConfiguration _appConfig;

    [Header("Colors")]
    [SerializeField] Color32 _accentColor;
    [SerializeField] Color32 _accentTextColor;
    [SerializeField] Color32 _foregroundColor;
    [SerializeField] Color32 _disabledColor;
    [SerializeField] Color32 _disabledTextColor;

    [Header("Controllers")]
    [SerializeField] ErrorWindowController _errorController;
    [SerializeField] LoginWindowController _loginController;

    [Header("Elements")]
    [SerializeField] GameObject _logo;
    [SerializeField] GameObject _connectionSpinner;
    [SerializeField] MainPageView _mainPage;
    [SerializeField] GameObject[] _inactivateOnStart;

    [Header("Sprites")]
    [SerializeField] Sprite _playSprite;
    [SerializeField] Sprite _pauseSprite;
    [SerializeField] Sprite _muteSprite;
    [SerializeField] Sprite _unmuteSprite;

    readonly float _popupTime = .5f;

    event VisiblePartChangedEventHandler VisiblePartChanged;

    IApiService _service;
    VisiblePart _parentPart, _currentPart;

    public Color32 ForegroundColor => _foregroundColor;
    public Color32 AccentColor => _accentColor;

    void Awake()
    {
        InactivateGameObjects();
        _service = ApiService.Instance.GetClient();
    }

    void OnEnable() => _service.SubscribeToConnectionChanged(OnConnectionChanged);

    void OnDisable() => _service.UnsubscribeFromConnectionChanged(OnConnectionChanged);

    void Start() { _service.SignIn(_appConfig.ApiServiceUrl, "admin", "123"); }

    public IMusicMateManager GetClient() => this;

    #region Change UI element states (disabled/enabled)
    public void ChangeState(Button button, bool enabled, bool? isPlaying)
    {
        button.interactable = enabled;
        if (isPlaying.HasValue)
        {
            var image = button.GetComponent<Image>();
            image.sprite = isPlaying.Value ? _pauseSprite : _playSprite;
        }
    }

    public void ChangeState(Image image, bool enabled, bool? isPlaying = null)
    {
        float target;
        if (!enabled)
            target = .01f;
        else if (isPlaying.HasValue && !isPlaying.Value)
            target = .2f;
        else
            target = 1f;

        image.DOFade(target, .25f).SetEase(Ease.InSine);
    }

    public void ChangeState(TextMeshProUGUI text, bool enabled) => text.color =
        enabled ? _foregroundColor : _disabledTextColor;

    public void ChangeStates(Button[] buttons, bool enabled, bool? isPlaying = null)
    {
        foreach (var item in buttons)
            ChangeState(item, enabled, isPlaying);
    }

    public void ChangeStates(TextMeshProUGUI[] texts, bool enabled)
    {
        foreach (var item in texts)
            ChangeState(item, enabled);
    }

    public void ChangeStates(Slider[] sliders, bool enabled)
    {
        foreach (var slider in sliders)
        {
            slider.interactable = enabled;

            var background = slider.transform.Find("Background").GetComponent<Image>();
            var handle = slider.transform.Find("Handle Slide Area/Handle").GetComponent<Image>();
            var fill = slider.transform.Find("Fill Area").gameObject;

            background.color = enabled ? _foregroundColor : _disabledColor;
            handle.color = enabled ? _accentColor : _disabledColor;
            fill.SetActive(enabled);
        }
    }

    public void ChangeStates(Image[] images, bool enabled, bool isPlaying)
    {
        foreach (var item in images)
            ChangeState(item, enabled, isPlaying);
    }
    #endregion


    /// <summary>
    /// Notify subscribed controllers the visibility of a particular part has changed.
    /// </summary>
    public void ChangeVisiblePart(VisiblePart part) => VisiblePartChanged?.Invoke(this, new VisiblePartChangedEventArgs(part));

    public void ShowError(ErrorType error, string message, string description = "")
    {
        _errorController.SetError(error, message, description);
        MoveErrorPanel(true);
    }

    public void ShowLogin()
    {
        float delay = 0f;
        if (_errorController.gameObject.activeSelf)
        {
            MoveErrorPanel(false);
            delay = .5f;
        }

        MoveLoginPanel(true, delay);
    }

    public void ShowRelease(ReleaseResult releaseModel)
    {
        _mainPage.ShowRelease(releaseModel);
        ChangeVisiblePart(VisiblePart.ReleaseDetails);
    }

    void HideLogo(bool quit=false)
    {
        var par = _logo.GetComponentInChildren<ParticleSystem>(true);
        var anim = _logo.GetComponent<Animator>();

        anim.Play("Fade");

        if (quit)
            StartCoroutine(DelayAndQuit(par.main.duration));
    }

    public void HideSpinner()
    {
        var images = _connectionSpinner.GetComponentsInChildren<Image>(true);
        var seq = DOTween.Sequence();

        for (int i = 0; i < images.Length; ++i)
        {
            var img = images[i];
            seq.Join(img.DOFade(0f, .5f));
        }
    }

    // Hide GameObjects initially not shown.
    void InactivateGameObjects()
    {
        for (int i = 0; i < _inactivateOnStart?.Length; i++)
        {
            _inactivateOnStart[i].SetActive(false);
        }
    }

    void MoveErrorPanel(bool show) => MovePanel(show, _errorController.gameObject, 3f, .5f);

    void MoveLoginPanel(bool show, float delay = 0f) => MovePanel(show, _loginController.gameObject, -2f, .5f, delay);

    void MovePanel(bool show, GameObject panelObj, float hidePivot, float showPivot, float delay = 0f)
    {
        if (show)
            panelObj.SetActive(true);

        var pivotTo = show ? showPivot : hidePivot;
        var easing = show ? Ease.OutBack : Ease.InBack;
        var rect = panelObj.GetComponent<RectTransform>();

        rect.DOPivotY(pivotTo, _popupTime).SetEase(easing)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                if (!show)
                    panelObj.SetActive(false);
            });
    }

    public void SubscribeToVisiblePartChanged(VisiblePartChangedEventHandler handler) => VisiblePartChanged += handler;

    public void UnsubscribeFromVisiblePartChanged(VisiblePartChangedEventHandler handler) => VisiblePartChanged -= handler;

    public void QuitApplication()
    {
        if (_errorController.gameObject.activeInHierarchy)
            MoveErrorPanel(false);
        else if (_loginController.gameObject.activeInHierarchy)
            MoveLoginPanel(false);

        if (_logo.activeInHierarchy)
            HideLogo(true);
        else
            QuitApp();
    }

    IEnumerator DelayAndQuit(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        QuitApp();
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
            ShowError(ErrorType.Connection, e.Error, _appConfig.ApiServiceUrl);
            HideSpinner();
        }
        else
            HideLogo();
    }

}
