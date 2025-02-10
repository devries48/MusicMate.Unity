using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DetailsAnimator : MusicMateBehavior
{
    [Header("Show Details Controllers")]
    [SerializeField] ShowReleaseController _releaseDetails;
    [SerializeField] ShowArtistController _artistDetails;

    [Header("Elements")]
    [SerializeField] ButtonInteractable _closeButton;
    [SerializeField] Image _spinnerBackground;
    [SerializeField] Image _spinner;

    public bool IsLoading { get; set; }

    readonly float _speed = 1.5f;
    Image _panel;
    MusicMateStateDetails _current;

    protected override void RegisterEventHandlers()
    {
        _closeButton.onClick.AddListener(OnCloseClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        _closeButton.onClick.RemoveListener(OnCloseClicked);
    }

    protected override void InitializeComponents()
    {
        _panel = GetComponent<Image>();
    }

    protected override void InitializeValues()
    {
        _current = MusicMateStateDetails.Release;
    }

    protected override void ApplyColors()
    {
        ChangeColor(MusicMateColor.Panel, _panel);
        ChangeColor(MusicMateColor.Accent, _spinner);
    }

    void Update()
    {
        if (IsLoading)
            _spinner.transform.Rotate(new Vector3(0, 0, -1 * _speed));
    }

    public void SetRelease(ReleaseResult release)
    {
        _releaseDetails.SetRelease(release);
    }

    public void StartSpinner()
    {
        _spinner.DOFade(1, .1f);
        _spinnerBackground.DOFade(1, .1f);

        IsLoading = true;
    }

    public void StopSpinner()
    {
        _spinner.DOFade(0, .25f).SetDelay(.25f);
        _spinnerBackground.DOFade(0, .25f).SetDelay(.25f).OnComplete(() =>
        {
            IsLoading = false;
        });
    }

    public void CloseDetails()
    {
        Animations.Panel.PlayDetailsVisibility(false, this);
        Manager.AppState.InvokeStateChanged(MusicMateStateChange.Details, false);
    }

    void OnCloseClicked() => CloseDetails();

    public void Show(MusicMateStateDetails details)
    {
        if (details == _current)
            return;

        var showGroup = details switch
        {
            MusicMateStateDetails.Release => _releaseDetails.gameObject,
            MusicMateStateDetails.Artist => _artistDetails.gameObject,
            _ => null
        };

        var hideGroup = _current switch
        {
            MusicMateStateDetails.Release => _releaseDetails.m_canvasGroup,
            MusicMateStateDetails.Artist => _artistDetails.m_canvasGroup,
            _ => null
        };

        _current = details;

        Animations.Panel.PlaySwitchDetails(showGroup, hideGroup);
    }
}
