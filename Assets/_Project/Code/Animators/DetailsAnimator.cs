using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DetailsAnimator : MusicMateBehavior
{
    [Header("Show Details Controllers")]
    [SerializeField] ShowReleaseController _releaseDetails;
    [SerializeField] ShowArtistController _artistDetails;
    [SerializeField] ShowCatalogController _catalogDetails;

    [Header("Elements")]
    [SerializeField] ButtonInteractable _closeButton;
    [SerializeField] Image _spinnerBackground;
    [SerializeField] Image _spinner;

    public ReleaseModel ReleaseModel { get; private set; } = null;
    public ArtistModel ArtistModel { get; private set; } = null;

    public bool IsLoading { get; set; }

    readonly float _speed = 1.5f;
    readonly Color32 _initialBackgroundColor = new(255, 255, 255, 3);

    Image _panel;
    MusicMateStateDetails _current;
    ReleaseResult _currentRelease = null;
    DataResult _currentArtist = null;

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        Manager.OnEditComplete += OnEditComplete;

        _closeButton.onClick.AddListener(OnCloseClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        Manager.OnEditComplete -= OnEditComplete;

        _closeButton.onClick.RemoveListener(OnCloseClicked);
    }

    protected override void InitializeComponents() { _panel = GetComponent<Image>(); }

    protected override void InitializeValues() { _current = MusicMateStateDetails.Release; }

    protected override void ApplyColors()
    {
        ChangeColor(MusicMateColor.Panel, _panel);
        ChangeColor(MusicMateColor.Accent, _spinner);
    }
    #endregion

    void Update()
    {
        if (IsLoading)
            _spinner.transform.Rotate(new Vector3(0, 0, -1 * _speed));
    }

    public void SetRelease(ReleaseResult release)
    {
        if (release != _currentRelease)
        {
            StartSpinner();
            NotifyPanelsOnInit<IShowDetails<ReleaseResult, ReleaseModel>, ReleaseResult, ReleaseModel>(release);

            _currentRelease = release;

            ApiService.GetRelease(release.Id, (model) =>
            {
                ReleaseModel = model;

                if (model == null)
                {
                    StopSpinner();
                    return;
                }
                NotifyPanelsOnUpdate<IShowDetails<ReleaseResult, ReleaseModel>, ReleaseResult, ReleaseModel>(model);

                var artist = _currentRelease.Artist;

                if (_currentArtist == artist)
                {
                    StopSpinner();
                    return;
                }

                NotifyPanelsOnInit<IShowDetails<DataResult, ArtistModel>, DataResult, ArtistModel>(artist);

                _currentArtist = artist;

                ApiService.GetArtist(artist.Id, (model) =>
                {
                    ArtistModel = model;

                    if (model != null)
                        NotifyPanelsOnUpdate<IShowDetails<DataResult, ArtistModel>, DataResult, ArtistModel>(model);
                  
                    StopSpinner();
                });
            });
        }
    }

    public void GetImage(string url, Image image)
    {
        if (string.IsNullOrEmpty(url))
            return;

        ApiService.DownloadImage(url, (sprite) =>
        {
            image.overrideSprite = sprite;
            image.DOFade(1f, .5f).SetEase(Ease.InSine);
        });
    }

    public void InitImage(Image image)
    {
        image.overrideSprite = null;
        image.color = _initialBackgroundColor;
    }

    public void CloseDetails()
    {
        Animations.Panel.PlayDetailsVisibility(false, this);
        Manager.AppState.InvokeStateChanged(MusicMateStateChange.Details, false);
    }

    public void Show(MusicMateStateDetails details)
    {
        if (details == _current)
            return;

        var showGroup = details switch
        {
            MusicMateStateDetails.Release => _releaseDetails.gameObject,
            MusicMateStateDetails.Artist => _artistDetails.gameObject,
            MusicMateStateDetails.Catalog => _catalogDetails.gameObject,
            _ => null
        };

        var hideGroup = _current switch
        {
            MusicMateStateDetails.Release => _releaseDetails.m_canvasGroup,
            MusicMateStateDetails.Artist => _artistDetails.m_canvasGroup,
            MusicMateStateDetails.Catalog => _catalogDetails.m_canvasGroup,
            _ => null
        };

        _current = details;

        Animations.Panel.PlaySwitchDetails(showGroup, hideGroup);
    }

    void OnEditComplete(MusicMateZone zone, object mocdel)
    {
        if (mocdel is ReleaseModel releaseModel)
            _releaseDetails.UpdateModel(zone, releaseModel);
        else if (mocdel is ArtistModel artistModel)
            _artistDetails.UpdateModel(zone, artistModel);
    }

    void OnCloseClicked() => CloseDetails();

    void NotifyPanelsOnInit<TPanel, TInit, TModel>(TInit initData) where TPanel : IShowDetails<TInit, TModel>
    {
        foreach (var panel in GetComponentsInChildren<TPanel>(true))
            panel.OnInit(initData);
    }

    void NotifyPanelsOnUpdate<TPanel, TInit, TModel>(TModel modelData) where TPanel : IShowDetails<TInit, TModel>
    {
        foreach (var panel in GetComponentsInChildren<TPanel>(true))
            panel.OnUpdated(modelData);
    }

    void StartSpinner()
    {
        _spinner.DOFade(1, .1f);
        _spinnerBackground.DOFade(1, .1f);

        IsLoading = true;
    }

    void StopSpinner()
    {
        _spinner.DOFade(0, .25f).SetDelay(.25f);
        _spinnerBackground.DOFade(0, .25f)
            .SetDelay(.25f)
            .OnComplete(
                () =>
                {
                    IsLoading = false;
                });
    }
}
