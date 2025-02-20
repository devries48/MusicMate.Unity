#region Usings
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Linq;
#endregion

[RequireComponent(typeof(AudioSource))]
public class AudioPlayerController : MusicMateBehavior
{
    #region Serialized Fields
    [Header("Audio Players")]
    [SerializeField] RectTransform _expandedPlayer;
    [SerializeField] RectTransform _collapsedPlayer;

    [Header("Shared Elements")]
    [SerializeField] Image[] _releaseImages;
    [SerializeField] Slider[] _positionSliders;
    [SerializeField] TextMeshProUGUI[] _trackStartTexts;
    [SerializeField] ButtonAnimator[] _playPauseButtons; // _shuffleButton, _repeatButton;
    [SerializeField] ButtonAnimator[] _nextButtons;
    [SerializeField] ButtonAnimator[] _previousButtons;

    [Header("Elements Expanded")]
    [SerializeField] ButtonAnimator _collapseButton;
    [SerializeField] Marquee _artistMarquee;
    [SerializeField] Marquee _titleMarquee;
    [SerializeField] TextMeshProUGUI _trackTotalText;
    [SerializeField] Slider _volumeSlider1;

    [Header("Elements Collapsed")]
    [SerializeField] ButtonAnimator _expandButton;
    [SerializeField] Marquee _artistAndTitleMarquee;
    [SerializeField] ButtonAnimator _volumeToggle;
    [SerializeField] CloseOnContextLoss _volumeDropdown;
    [SerializeField] Slider _volumeSlider2;
    #endregion

    public bool IsPlayerExpanded { get; set; } = true; // TODO: Save and load state

    AudioSource _audioSource;
    FadeData _fade;
    Image _panelExpanded, _panelCollapsed;

    float _volume = 0.5f;
    bool _isPosDrag = false;
    bool _isVolumeOn = false;

    internal CanvasGroup m_canvasGroupExpanded;

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        PlayerService.SubscribeToStateChanged(OnPlayerStateChanged);
        PlayerService.SubscribeToActionChanged(OnActionChanged);

        _collapseButton.OnButtonClick.AddListener(OnCollapsePlayerClick);
        _expandButton.OnButtonClick.AddListener(OnExpandPlayerdClick);

        foreach (var button in _playPauseButtons)
            button.OnButtonClick.AddListener(OnPlayPauseClicked);

        foreach (var button in _nextButtons)
            button.OnButtonClick.AddListener(OnNextClicked);

        foreach (var button in _previousButtons)
            button.OnButtonClick.AddListener(OnPreviousClicked);

        _volumeToggle.OnButtonClick.AddListener(OnVolumeToggled);

        _volumeSlider1.onValueChanged.AddListener(delegate { OnVolume1Changed(); });
        _volumeSlider2.onValueChanged.AddListener(delegate { OnVolume2Changed(); });
    }

    protected override void UnregisterEventHandlers()
    {
        PlayerService.UnsubscribeFromStateChanged(OnPlayerStateChanged);
        PlayerService.UnsubscribeFromActionChanged(OnActionChanged);

        _collapseButton.OnButtonClick.RemoveListener(OnCollapsePlayerClick);
        _expandButton.OnButtonClick.RemoveListener(OnExpandPlayerdClick);

        foreach (var button in _playPauseButtons)
            button.OnButtonClick.RemoveListener(OnPlayPauseClicked);

        foreach (var button in _nextButtons)
            button.OnButtonClick.RemoveListener(OnNextClicked);

        foreach (var button in _previousButtons)
            button.OnButtonClick.RemoveListener(OnPreviousClicked);

        _volumeToggle.OnButtonClick.RemoveListener(OnVolumeToggled);

        _volumeSlider1.onValueChanged.RemoveListener(delegate { OnVolume1Changed(); });
        _volumeSlider2.onValueChanged.RemoveListener(delegate { OnVolume2Changed(); });
    }

    protected override void InitializeComponents()
    {
        _audioSource = GetComponent<AudioSource>();
        _panelExpanded = _expandedPlayer.transform.GetComponent<Image>();
        _panelCollapsed = _collapsedPlayer.transform.GetComponent<Image>();

        m_canvasGroupExpanded = _expandedPlayer.gameObject.GetComponent<CanvasGroup>();
    }

    protected override void InitializeValues()
    {
        PlayerService.PlayerWidth = _expandedPlayer.rect.width;

        m_canvasGroupExpanded.alpha = 0f;
        _fade = new FadeData(.2f, .4f);

        InitElements();
        StartCoroutine(SetPlayerState());
    }

    protected override void ApplyColors()
    {
        ChangeColor(MusicMateColor.Panel, _panelExpanded, _panelCollapsed);
        ChangeState(PlayerService.IsActive, _artistMarquee, _titleMarquee, _artistAndTitleMarquee);
        ChangeState(PlayerService.IsActive, _trackTotalText);
        ChangeState(PlayerService.IsActive, _trackStartTexts);
        ChangeState(PlayerService.IsActive, _positionSliders);
        ChangeState(true, _volumeSlider1, _volumeSlider2);
    }

    void ChangeReleaseImageState(Image[] images, bool enabled, bool isPlaying)
    {
        foreach (var item in images)
        {
            float target;
            if (!enabled)
                target = .01f;
            else if (!isPlaying)
                target = .2f;
            else
                target = 1f;

            item.DOFade(target, .25f).SetEase(Ease.InSine);
        }
    }

    void InitElements()
    {
        _volumeDropdown.gameObject.SetActive(false);
        _artistMarquee.ClearText();
        _titleMarquee.ClearText();
        _artistAndTitleMarquee.ClearText();
        _volumeSlider1.value = _volume;
        _volumeSlider2.value = _volume;
    }
    #endregion

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

        if (PlayerService.IsPlaying && !_audioSource.isPlaying && _audioSource.time == 0)
            ClipFinished();

        //_audioSource.loop = repeat;
    }

    void OnExpandPlayerdClick() => ExpandPlayer();

    void OnCollapsePlayerClick() => CollapsePlayer();

    /// <summary>
    /// The Audio Player is expanded. Hide mini-player, move player down and notify the parent.
    /// </summary>
    public void ExpandPlayer(bool delay = false)
    {
        IsPlayerExpanded = true;

        _collapseButton.gameObject.SetActive(true);
        _expandButton.gameObject.SetActive(false);
        PlayerService.ChangeExpandedState(true);

        Animations.Panel.PlayExpandAudioPlayer(_expandedPlayer, _collapsedPlayer, delay);
    }

    /// <summary>
    /// The Audio Player is collapsed. Move player up, notify the parent and show the mini-player. When expanded: hide mini-
    /// player, move player down and notify the parent.
    /// </summary>
    public void CollapsePlayer(bool delay = false)
    {
        IsPlayerExpanded = false;

        _collapseButton.gameObject.SetActive(false);
        _expandButton.gameObject.SetActive(true);

        Animations.Panel.PlayCollapseAudioPlayer(_expandedPlayer, _collapsedPlayer, delay, () =>
        {
            PlayerService.ChangeExpandedState(false);
        });
    }

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

    void OnPlayPauseClicked()
    {
        if (PlayerService.IsPlaying)
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
        if (PlayerService.IsPaused)
            ResumeFromPause();
    }

    void Pause()
    {
        if (PlayerService.IsPlaying)
            Fade(_volume, 0, _fade.FadeOutTime);
    }

    IEnumerator SetPlayerState()
    {
        ApplyColors();

        Manager.AppState.ChangePlayButtonsState(_playPauseButtons, PlayerService.IsActive, PlayerService.IsPlaying);
        ChangeState(PlayerService.CanMoveBack, _previousButtons);
        ChangeState(PlayerService.CanMoveForward, _nextButtons);
        ChangeReleaseImageState(_releaseImages, PlayerService.IsActive, PlayerService.IsPlaying);

        yield return null;
    }

    IEnumerator SetPlayerTrack()
    {
        var release = PlayerService.CurrentRelease;
        var track = PlayerService.CurrentTrack;

        ApiService.DownloadImage(release.ThumbnailUrl, ProcessReleaseImage);

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

    void SetPlayerState(PlayerState state) => PlayerService.ChangeState(state);

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
