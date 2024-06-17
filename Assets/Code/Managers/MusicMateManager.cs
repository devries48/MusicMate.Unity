using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicMateManager : SceneSingleton<MusicMateManager>, IMusicMateManager
{
    [Header("Views")]
    [SerializeField] MainPageView _mainPage;

    [Header("Colors")]
    [SerializeField] Color32 _accentColor;
    [SerializeField] Color32 _foregroundColor;
    [SerializeField] Color32 _disabledColor;
    [SerializeField] Color32 _disabledTextColor;

    [Header("Sprites")]
    [SerializeField] Sprite _playSprite;
    [SerializeField] Sprite _pauseSprite;
    [SerializeField] Sprite _muteSprite;
    [SerializeField] Sprite _unmuteSprite;

    event VisiblePartChangedEventHandler VisiblePartChanged;

    public Color32 ForegroundColor => _foregroundColor;
    public Color32 AccentColor => _accentColor;

    IApiService _service;
    VisiblePart _parentPart,_currentPart;

    void Awake() => _service = ApiService.Instance.GetClient();

    void OnEnable() => _service.SubscribeToConnectionChanged(OnConnectionChanged);

    void OnDisable() => _service.UnsubscribeFromConnectionChanged(OnConnectionChanged);

    void Start() { _service.SignIn("admin", "123"); }

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

    public void ChangeVisiblePart(VisiblePart part)
    {
        VisiblePartChanged?.Invoke(this, new VisiblePartChangedEventArgs( part));
    }

    public void ShowRelease(ReleaseResult releaseModel)
    {
        _mainPage.ShowRelease(releaseModel);
        ChangeVisiblePart(VisiblePart.ReleaseDetails);
    }

    public void SubscribeToVisiblePartChanged(VisiblePartChangedEventHandler handler) => VisiblePartChanged += handler;

    public void UnsubscribeFromVisiblePartChanged(VisiblePartChangedEventHandler handler) => VisiblePartChanged -= handler;

    void OnConnectionChanged(object sender, ConnectionChangedEventArgs e)
    {
        _mainPage.ConnectionChanged(e.Connected);
        //TODO: Display error, login
    }


}
