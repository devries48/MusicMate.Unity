using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayerController : MonoBehaviour
{
    #region Unity inspector fields
    [Header("Audio Players")]
    [SerializeField] RectTransform _expandedPlayer;
    [SerializeField] RectTransform _collapsedPlayer;

    [Header("Shared Elements")]
    [SerializeField] Image[] _releaseImages;
    [SerializeField] Slider[] _positionSliders;
    [SerializeField] TextMeshProUGUI[] _trackStartTexts;
    [SerializeField] Button[] _playPauseButtons; // _shuffleButton, _repeatButton;
    [SerializeField] Button[] _nextButtons;
    [SerializeField] Button[] _previousButtons;

    [Header("Elements Expanded")]
    [SerializeField] Button _collapseButton;
    [SerializeField] Marquee _artistMarquee;
    [SerializeField] Marquee _titleMarquee;
    [SerializeField] TextMeshProUGUI _trackTotalText;
    [SerializeField] Slider _volumeSlider1;

    [Header("Elements Collapsed")]
    [SerializeField] Button _expandButton;
    [SerializeField] Marquee _artistAndTitleMarquee;
    [SerializeField] Button _volumeToggle;
    [SerializeField] CloseOnContextLoss _volumeDropdown;
    [SerializeField] Slider _volumeSlider2;
    #endregion

    public bool IsPlayerExpanded { get; set; } = true; // TODO: Save and load state

    internal CanvasGroup m_canvasGroupExpanded;

    IAudioPlayerService _playerService;
    IApiService _apiService;
    IMusicMateManager _manager;
    AudioSource _audioSource;
    FadeData _fade;

    readonly float _collapseTime = 1f;

    float _volume = 0.5f;
    bool _isPosDrag = false;
    bool _isVolumeOn = false;

    #region Unity callbacks
    void OnEnable()
    {
        _playerService.SubscribeToStateChanged(OnPlayerStateChanged);
        _playerService.SubscribeToActionChanged(OnActionChanged);
    }

    void OnDisable()
    {
        _playerService.UnsubscribeFromStateChanged(OnPlayerStateChanged);
        _playerService.UnsubscribeFromActionChanged(OnActionChanged);
    }

    void Awake()
    {
        _manager= MusicMateManager.Instance; ;
        _playerService = AudioPlayerService.Instance;
        _apiService = ApiService.Instance.GetClient();
        _audioSource = GetComponent<AudioSource>();

        m_canvasGroupExpanded = _expandedPlayer.gameObject.GetComponent<CanvasGroup>();
        m_canvasGroupExpanded.alpha = 0f;
    }

    void Start()
    {
        _playerService.PlayerWidth = _expandedPlayer.rect.width;
        _fade = new FadeData(.2f, .4f);

        InitElements();
        StartCoroutine(SetPlayerState());
    }

    void Update()
    {
        if (_fade.Time > 0)
            SetFadeVolume();
        else
            _audioSource.volume = _volume;

        if (!_isPosDrag && _audioSource.isPlaying)
        {
            foreach (var slider in _positionSliders)
                slider.value = _audioSource.time;

            foreach (var text in _trackStartTexts)
                text.SetText(ToTimeString(_audioSource.time));
        }

        if (_playerService.IsPlaying && !_audioSource.isPlaying && _audioSource.time == 0)
            ClipFinished();

        //_audioSource.loop = repeat;
    }

    void InitElements()
    {
        _volumeDropdown.gameObject.SetActive(false);
        _artistMarquee.ClearText();
        _titleMarquee.ClearText();
        _artistAndTitleMarquee.ClearText();
        _volumeSlider1.value = _volume;
        _volumeSlider2.value = _volume;

        _collapseButton.onClick.AddListener(() => OnCollapseClicked());
        _expandButton.onClick.AddListener(() => OnExpandClicked());

        foreach (var button in _playPauseButtons)
            button.onClick.AddListener(() => OnPlayPauseClicked());

        foreach (var button in _nextButtons)
            button.onClick.AddListener(() => OnNextClicked());

        foreach (var button in _previousButtons)
            button.onClick.AddListener(() => OnPreviousClicked());

        _volumeToggle.onClick.AddListener(() => OnVolumeToggled());
        _volumeSlider1.onValueChanged.AddListener(delegate { OnVolume1Changed(); });
        _volumeSlider2.onValueChanged.AddListener(delegate { OnVolume2Changed(); });
    }
    #endregion

    void OnPlayerStateChanged(object sender, StateChangedEventArgs e) => StartCoroutine(SetPlayerState());

    void OnActionChanged(object sender, ActionChangedEventArgs e)
    {
        if (e.Action == PlayerAction.PlayRelease)
            StartCoroutine(SetPlayerTrack());
        else if (e.Action == PlayerAction.Play)
            Play();
        else if (e.Action == PlayerAction.Pause)
            Pause();
    }

    /// <summary>
    /// The Audio Player is collapsed. Move player up, notify the parent and show the mini-player. When expanded: hide mini-
    /// player, move player down and notify the parent.
    /// </summary>
    void OnCollapseClicked()
    {
        _collapseButton.gameObject.SetActive(false);
        _expandButton.gameObject.SetActive(true);

        var scaleTo = _expandedPlayer.transform.localScale * .8f;

        _expandedPlayer.DOScale(scaleTo, _collapseTime / 2)
            .SetEase(Ease.InBack)
            .OnComplete(
                () => _expandedPlayer.DOPivotX(-.1f, _collapseTime / 2)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => _playerService.ChangeExpandedState(false)));

        _collapsedPlayer.DOPivotY(1, _collapseTime).SetEase(Ease.OutBack).SetDelay(.5f);
    }

    /// <summary>
    /// The Audio Player is expanded. Hide mini-player, move player down and notify the parent.
    /// </summary>
    void OnExpandClicked()
    {
        _collapseButton.gameObject.SetActive(true);
        _expandButton.gameObject.SetActive(false);
        _playerService.ChangeExpandedState(true);

        _collapsedPlayer.DOPivotY(-.5f, _collapseTime / 2).SetEase(Ease.InBack);

        _expandedPlayer.DOPivotX(1, _collapseTime / 2)
            .SetEase(Ease.OutBack).SetDelay(.5f)
            .OnComplete(
                () => _expandedPlayer.DOScale(1, _collapseTime / 2)
                    .SetEase(Ease.OutBack));
    }

    void OnPlayPauseClicked()
    {
        if (_playerService.IsPlaying)
            Pause();
        else
            Play();
    }

    void OnNextClicked()
    {

    }

    void OnPreviousClicked()
    {

    }

    void OnVolumeToggled()
    {
        _isVolumeOn = !_isVolumeOn;

        if (_isVolumeOn)
            _volumeDropdown.Show();
        else
            _volumeDropdown.Hide();
    }

    void OnVolume1Changed()
    {
        _volume = _volumeSlider1.value;
        _volumeSlider2.value = _volume;
    }

    void OnVolume2Changed()
    {
        _volume = _volumeSlider2.value;
        _volumeSlider1.value = _volume;
    }

    void Play()
    {
        if (_playerService.IsPaused)
            ResumeFromPause();
    }

    void Pause()
    {
        if (_playerService.IsPlaying)
            Fade(_volume, 0, _fade.FadeOutTime);
    }

    IEnumerator SetPlayerState()
    {
        _manager.AppState.ChangeStates(_playPauseButtons, _playerService.IsActive, _playerService.IsPlaying);
        _manager.AppState.ChangeStates(_previousButtons, _playerService.CanMoveBack);
        _manager.AppState.ChangeStates(_nextButtons, _playerService.CanMoveForward);

        _manager.AppState.ChangeStates(_trackStartTexts, _playerService.IsActive);
        _manager.AppState.ChangeState(_trackTotalText, _playerService.IsActive);
        _manager.AppState.ChangeStates(_positionSliders, _playerService.IsActive);
        _manager.AppState.ChangeStates(_releaseImages, _playerService.IsActive, _playerService.IsPlaying);

        yield return null;
    }

    IEnumerator SetPlayerTrack()
    {
        var release = _playerService.CurrentRelease;
        var track = _playerService.CurrentTrack;

        _apiService.DownloadImage(release.ThumbnailUrl, ProcessReleaseImage);

        yield return null;

        _artistMarquee.SetText(release.Artist.Text);
        _titleMarquee.SetText(track.Title);
        _artistAndTitleMarquee.SetText(release.Artist.Text + " - " + track.Title);
        _trackTotalText.SetText(track.DurationString);

        foreach (var text in _trackStartTexts)
            text.SetText("0:00");

        foreach (var slider in _positionSliders)
            slider.maxValue = Mathf.Floor(track.Duration / 1000);

        yield return null;

        StartCoroutine(PlayNewTrack(track.TrackPlayUrl));
    }

    IEnumerator PlayNewTrack(string url)
    {
        using var wr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);

        ((DownloadHandlerAudioClip)wr.downloadHandler).streamAudio = true;
        wr.SendWebRequest();

        while (wr.result == UnityWebRequest.Result.InProgress)
            yield return null;

        if (wr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(wr.error);
        }
        else
        {
            //_audioSource.PlayOneShot(DownloadHandlerAudioClip.GetContent(wr));
            var clip = ((DownloadHandlerAudioClip)wr.downloadHandler).audioClip;
            _audioSource.clip = clip;
            _audioSource.Play();
            SetPlayerState(PlayerState.Playing);
        }
    }

    void SetPlayerState(PlayerState state) => _playerService.ChangeState(state);

    void ProcessReleaseImage(Sprite sprite)
    {
        foreach (var image in _releaseImages)
            image.overrideSprite = sprite;
    }

    void SetFadeVolume()
    {
        _fade.EndVolume = Mathf.Clamp(_fade.EndVolume, 0, _volume); // volume changed during fade?

        float t = (Time.time - _fade.StartTime) / _fade.Time;

        if (t >= 1)
        {
            _fade.Time = 0;
            if (_fade.EndVolume == 0)
            {
                _audioSource.Pause();
                SetPlayerState(PlayerState.Paused);
            }
        }
        else
            _audioSource.volume = (1 - t) * _fade.StartVolume + t * _fade.EndVolume;
    }

    void ClipFinished()
    {
        //if (autoAdvance)
        //{
        //    Next();
        //}
        //else
        //{
        //    Pause();
        //}
    }

    void Fade(float startVolume, float endVolume, float time)
    {
        _fade.StartTime = Time.time;
        _fade.Time = time;
        _fade.StartVolume = startVolume;
        _fade.EndVolume = endVolume;
        _audioSource.volume = startVolume;
    }

    void ResumeFromPause()
    {
        _audioSource.UnPause();
        SetPlayerState(PlayerState.Playing);
        Fade(0, _volume, _fade.FadeInTime);
    }

    static string ToTimeString(float seconds)
    {
        if (seconds < 0)
            return "0:00";

        int currentSeconds = (int)(seconds % 60);
        int minutes = (int)(seconds / 60);

        string secondsStr = "";
        if (currentSeconds < 10)
            secondsStr = "0";

        secondsStr += currentSeconds.ToString();

        return $"{minutes}:{secondsStr}";
    }

    struct FadeData
    {
        public FadeData(float fadeIn, float fadeOut)
        {
            FadeInTime = fadeIn;
            FadeOutTime = fadeOut;
            StartVolume = 0;
            EndVolume = 0;
            Time = 0;
            StartTime = 0;
        }

        public float FadeInTime;
        public float FadeOutTime;

        public float StartVolume;
        public float EndVolume;
        public float Time;
        public float StartTime;

    }
}
