using DG.Tweening;
using UnityEngine;

public class MainWindowAnimator : MonoBehaviour
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

    #region Field Declarations
    IApiService _service;
    IAudioPlayerService _playerService;
    IMusicMateManager _manager;
    AnimationManager _animations;
    State _state;

    float _fadeTime = 0;
    readonly float _popupTime = .5f;
    #endregion

    #region Unity Events
    void Awake()
    {
        _state = new State();
        _service = ApiService.Instance.GetClient();
        _playerService = AudioPlayerService.Instance;
        _manager = MusicMateManager.Instance;
        _animations = AnimationManager.Instance;

        InitPanels();
    }

    void OnEnable()
    {
        _playerService.SubscribeToExpandedChanged(OnAudioPlayerExpandedChanged);
        _manager.AppState.SubscribeToVisiblePartChanged(OnVisiblePartChanged);
    }

    void OnDisable()
    {
        _playerService.UnsubscribeFromExpandedChanged(OnAudioPlayerExpandedChanged);
        _manager.AppState.UnsubscribeFromVisiblePartChanged(OnVisiblePartChanged);
    }
    #endregion

    public void ConnectionChanged(bool connected)
    {
        if (connected && _fadeTime == 0)
        {
            _fadeTime = 1f;
            ActivatePanels(true);
        }

        _animations.PanelVisible(connected, _fadeTime, 0f,
            _audioPlayer.m_canvasGroupExpanded,
            _applicationToolbar.m_canvasGroup,
            _searchToolbar.m_CanvasGroup,
            _importToolbar.m_CanvasGroup);

        _animations.PanelVisible(connected, _fadeTime, 2f, _releaseResult.m_canvasGroup);

        if (connected)
            _service.GetInitialReleases(GetInitialReleasesCallback);
    }

    public void ShowRelease(ReleaseResult release) => _releaseDetails.GetRelease(release);

    /// <summary>
    /// Hide all elements
    /// </summary>
    void InitPanels()
    {
        ActivatePanels(false);
        //ConnectionChanged(false);
    }

    void ActivatePanels(bool activate)
    {
        _audioPlayer.gameObject.SetActive(activate);
        _releaseResult.gameObject.SetActive(activate);
        _applicationToolbar.gameObject.SetActive(activate);
        _searchToolbar.gameObject.SetActive(activate);
        _importToolbar.gameObject.SetActive(activate);
    }

    void VisibleReleaseResult(bool show)
    {
        var scaleTo = show ? 1f : .5f;
        var fadeTo = show ? 1f : .01f;
        var easing = show ? Ease.InQuint : Ease.OutQuint;

        _releaseResult.transform.DOScale(scaleTo, _popupTime).SetEase(easing);
        _releaseResult.m_canvasGroup.DOFade(fadeTo, _popupTime).SetEase(easing);

        _animations.PanelReleaseResultVisible(_releaseResult, show);
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

    void MoveReleaseDetails(bool show, float delay = 0)
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

    void GetInitialReleasesCallback(PagedResult<ReleaseResult> result)
    {
        _releaseResult.SetResult(result);
        _manager.HideSpinner();
    }

    void OnAudioPlayerExpandedChanged(object sender, ExpandedChangedEventArgs e)
    {
        _releaseResult.SetRightMargin(e.IsExpanded);
    }

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
