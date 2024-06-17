using DG.Tweening;
using System;
using UnityEngine;

public class MainPageView : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] AudioPlayerController _audioPlayer;
    [SerializeField] ReleaseResultController _releaseResult;
    [SerializeField] ReleaseDetailsController _releaseDetails;
    [SerializeField] ArtistDetailsController _artistDetails;

    [Header("Toolbar Controllers")]
    [SerializeField] ToolbarApplicationController _applicationToolbar;
    [SerializeField] ToolbarPartController _searchToolbar;
    [SerializeField] ToolbarImportController _importToolbar;

    IApiService _service;
    IAudioPlayerService _playerService;
    IMusicMateManager _manager;
    State _state;

    readonly float _fadeTime = 1;
    readonly float _popupTime = .5f;

    void Awake()
    {
        _service = ApiService.Instance.GetClient();
        _playerService = AudioPlayerService.Instance;
        _manager = MusicMateManager.Instance;
    }

    void Start()
    {
        _state = new State();
        _releaseDetails.gameObject.SetActive(false);
        _artistDetails.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        _playerService.SubscribeToExpandedChanged(OnAudioPlayerExpandedChanged);
        _manager.SubscribeToVisiblePartChanged(OnVisiblePartChanged);
    }

    void OnDisable()
    {
        _playerService.UnsubscribeFromExpandedChanged(OnAudioPlayerExpandedChanged);
        _manager.UnsubscribeFromVisiblePartChanged(OnVisiblePartChanged);
    }

    public void ShowRelease(ReleaseResult release) => _releaseDetails.GetRelease(release);

    public void ConnectionChanged(bool connected)
    {
        PanelFade(_audioPlayer.m_canvasGroupExpanded, connected);
        PanelFade(_releaseResult.m_canvasGroup, connected);
        PanelFade(_applicationToolbar.m_canvasGroup, connected);
        PanelFade(_searchToolbar.m_CanvasGroup, connected);
        PanelFade(_importToolbar.m_CanvasGroup, connected);

        if (connected)
            _service.GetInitialReleases(GetInitialReleasesCallback);
    }

    void OnAudioPlayerExpandedChanged(object sender, ExpandedChangedEventArgs e)
    {
        _releaseResult.SetRightMargin(e.IsExpanded);
    }

    // ReleaseResult -> ReleaseDetails
    void OnVisiblePartChanged(object sender, VisiblePartChangedEventArgs e)
    {
        var p = e.Part;
        if (p == VisiblePart.Previous)
        {
            if (_state.ReleaseDetailsArtist == State.States.visible)
            {
                p = VisiblePart.ReleaseDetails;
                VisibleArtistDetails(false);
            }
        }

        switch (p)
        {
            case VisiblePart.ReleaseResult:
                VisibleReleaseResult(true);

                if (_state.ReleaseDetails == State.States.visible)
                    VisibleReleaseDetails(false);

                break;

            case VisiblePart.ReleaseDetails:
                if (_state.ReleaseDetails == State.States.moved)
                    MoveReleaseDetails(true, _popupTime);
                else
                    VisibleReleaseDetails(true);

                if (_state.ReleaseDetailsArtist == State.States.visible)
                    VisibleArtistDetails(false);

                if (_state.ReleaseResult == State.States.visible)
                    VisibleReleaseResult(false);

                break;

            case VisiblePart.ArtistDetails:
                VisibleArtistDetails(true, _popupTime);
                MoveReleaseDetails(false);
                break;

            default:
                break;
        }
    }

    void VisibleReleaseResult(bool show)
    {
        var scaleTo = show ? 1f : .5f;
        var fadeTo = show ? 1f : .01f;
        var easing = show ? Ease.InQuint : Ease.OutQuint;

        _releaseResult.transform.DOScale(scaleTo, _popupTime).SetEase(easing);
        _releaseResult.m_canvasGroup.DOFade(fadeTo, _popupTime).SetEase(easing);

        _state.ReleaseResult = show ? State.States.visible : State.States.hidden;
    }

    void VisibleReleaseDetails(bool show)
    {
        if (show)
            ScaleIn(_releaseDetails.transform);
        else
            ScaleOut(_releaseDetails.transform);

        _state.ReleaseDetails = show ? State.States.visible : State.States.hidden;
    }

    void MoveReleaseDetails(bool show, float delay=0)
    {
        var pivotTo = show ? .5f : 2f;
        var easing = show ? Ease.OutBack : Ease.InBack;
        var rect = _releaseDetails.gameObject.GetComponent<RectTransform>();

        rect.DOPivotY(pivotTo, _popupTime).SetEase(easing).SetDelay(delay);

        _state.ReleaseDetails = show ? State.States.visible : State.States.moved;
    }

    void VisibleArtistDetails(bool show, float delay = 0)
    {
        if (show)
            ScaleIn(_artistDetails.transform, delay);
        else
            ScaleOut(_artistDetails.transform);

        _state.ReleaseDetailsArtist = show ? State.States.visible : State.States.hidden;
    }

    void ScaleIn(Transform trans, float delay = 0)
    {
        trans.localScale = Vector3.zero;
        trans.gameObject.SetActive(true);
        trans.DOScale(1, _popupTime).SetEase(Ease.OutBack).SetDelay(delay);
    }

    void ScaleOut(Transform trans)
    {
        trans.DOScale(0, _popupTime).SetEase(Ease.InBack)
            .OnComplete(() => trans.gameObject.SetActive(false));
    }

    void PanelFade(CanvasGroup canvas, bool fadeIn)
    {
        canvas.alpha = fadeIn ? 0f : 1f;
        canvas.DOFade(fadeIn ? 1f : 0f, _fadeTime).SetEase(Ease.InSine).SetDelay(fadeIn ? .5f : 0f);
    }

    void GetInitialReleasesCallback(PagedResult<ReleaseResult> result) => _releaseResult.SetResult(result);

    class State
    {
        public State()
        {
            ReleaseResult = States.visible;
            ReleaseDetails = States.hidden;
            ReleaseDetailsArtist = States.hidden;
        }

        public enum States { visible, hidden, moved }

        public States ReleaseResult;
        public States ReleaseDetails;
        public States ReleaseDetailsArtist;
    }
}
