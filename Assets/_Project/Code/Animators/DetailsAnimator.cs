using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DetailsAnimator : MusicMateBehavior
{
    [Header("Tabs")] 
    [SerializeField] TabItemAnimator releaseTab;
    [SerializeField] TabItemAnimator artistTab;
    [SerializeField] TabItemAnimator catalogTab;

    [Header("Controllers")] 
    [SerializeField] ShowReleaseController releaseDetails;
    [SerializeField] ShowArtistController artistDetails;
    [SerializeField] ShowCatalogController catalogDetails;

    [Header("Elements")] [SerializeField] ButtonInteractable closeButton;
    [SerializeField] Image spinnerBackground;
    [SerializeField] Image spinner;

    public ReleaseModel ReleaseModel { get; private set; }
    public ArtistModel ArtistModel { get; private set; }

    bool IsLoading { get; set; }

    readonly Color32 _initialBackgroundColor = new(255, 255, 255, 3);

    Image _background;
    MusicMateStateDetails _currentTab;
    ReleaseResult _currentRelease;
    DataResult _currentArtist;

    const float Speed = 1.5f;

    #region Base Class Methods

    protected override void RegisterEventHandlers()
    {
        Manager.OnEditComplete += OnEditComplete;

        closeButton.onClick.AddListener(OnCloseClicked);

        releaseTab.OnTabItemClick.AddListener(OnReleaseTabClicked);
        artistTab.OnTabItemClick.AddListener(OnArtistTabClicked);
        catalogTab.OnTabItemClick.AddListener(OnCatalogTabClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        Manager.OnEditComplete -= OnEditComplete;

        closeButton.onClick.RemoveListener(OnCloseClicked);

        releaseTab.OnTabItemClick.RemoveListener(OnReleaseTabClicked);
        artistTab.OnTabItemClick.RemoveListener(OnArtistTabClicked);
        catalogTab.OnTabItemClick.RemoveListener(OnCatalogTabClicked);
    }

    protected override void InitializeComponents()
    {
        //_background = GetComponent<Image>();
        transform.Find("Content").TryGetComponent(out _background);
    }

    protected override void InitializeValues()
    {
        _currentTab = MusicMateStateDetails.Release;
        SetElementStates(_currentTab);
    }

    protected override void ApplyColors()
    {
        ChangeColor(MusicMateColor.Panel, _background);
        ChangeColor(MusicMateColor.Accent, spinner);
    }

    #endregion

    void SetElementStates(MusicMateStateDetails tab)
    {
        releaseTab.SetActive(tab == MusicMateStateDetails.Release);
        artistTab.SetActive(tab == MusicMateStateDetails.Artist);
        catalogTab.SetActive(tab == MusicMateStateDetails.Catalog);
        
        Manager.AppState.InvokeStateChanged(tab);
    }

    void Update()
    {
        if (IsLoading)
            spinner.transform.Rotate(new Vector3(0, 0, -1 * Speed));
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

                ApiService.GetArtist(artist.Id, artistModel =>
                {
                    ArtistModel = artistModel;

                    if (artistModel != null)
                        NotifyPanelsOnUpdate<IShowDetails<DataResult, ArtistModel>, DataResult, ArtistModel>(
                            artistModel);

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

    public void Show(MusicMateStateDetails details)
    {
        if (details == _currentTab)
            return;

        var showGroup = details switch
        {
            MusicMateStateDetails.Release => releaseDetails.gameObject,
            MusicMateStateDetails.Artist => artistDetails.gameObject,
            MusicMateStateDetails.Catalog => catalogDetails.gameObject,
            _ => null
        };

        var hideGroup = _currentTab switch
        {
            MusicMateStateDetails.Release => releaseDetails.m_canvasGroup,
            MusicMateStateDetails.Artist => artistDetails.m_canvasGroup,
            MusicMateStateDetails.Catalog => catalogDetails.m_canvasGroup,
            _ => null
        };

        _currentTab = details;

        Animations.Panel.PlaySwitchDetails(showGroup, hideGroup);
    }

    void OnEditComplete(MusicMateZone zone, object model)
    {
        if (model is ReleaseModel releaseModel)
            releaseDetails.UpdateModel(zone, releaseModel);
        else if (model is ArtistModel artistModel)
            artistDetails.UpdateModel(zone, artistModel);
    }

    void OnCloseClicked() => CloseDetails();

    void CloseDetails()
    {
        Animations.Panel.PlayDetailsVisibility(false, this);
        Manager.AppState.InvokeStateChanged(MusicMateStateChange.Details, false);
    }

    void OnReleaseTabClicked()
    {
        SetElementStates(MusicMateStateDetails.Release);
    }

    void OnArtistTabClicked()
    {
        SetElementStates(MusicMateStateDetails.Artist);
    }

    void OnCatalogTabClicked()
    {
        SetElementStates(MusicMateStateDetails.Catalog);
    }

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
        spinner.DOFade(1, .1f);
        spinnerBackground.DOFade(1, .1f);

        IsLoading = true;
    }

    void StopSpinner()
    {
        spinner.DOFade(0, .25f).SetDelay(.25f);
        spinnerBackground.DOFade(0, .25f)
            .SetDelay(.25f)
            .OnComplete(() => { IsLoading = false; });
    }
}