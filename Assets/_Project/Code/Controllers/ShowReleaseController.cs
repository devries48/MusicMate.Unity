#region Usings
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

public class ShowReleaseController : MusicMateBehavior
{
    #region Serialized Fields
    [SerializeField] DetailsAnimator _parent;
    public PanelReleaseStateData m_normal;
    public PanelReleaseStateData m_maximized;

    // Elements
    [SerializeField] Image _image;
    [SerializeField] Marquee _artist;
    [SerializeField] Marquee _title;
    [SerializeField] TextMeshProUGUI _yearCountry;
    [SerializeField] TextMeshProUGUI _mainGenre;
    [SerializeField] TextMeshProUGUI _subGenres;

    public TextMeshProUGUI m_artist_title;
    public TextMeshProUGUI m_total_length;
    public GridTrackController m_tracks;

    // Panels
    public RectTransform m_imagePanel;
    public RectTransform m_mainInfoPanel;

    // Buttons
    [SerializeField] ButtonAnimator _stateButton;
    [SerializeField] ButtonAnimator _upButton;
    [SerializeField] ButtonAnimator _downButton;
    #endregion

    public ReleaseResult CurrentRelease { get; private set; } = null;

    internal CanvasGroup m_canvasGroup;
    readonly Color32 _initialBackgroundColor = new(255, 255, 255, 3);

    #region Base Class Methods
    protected override void InitializeComponents()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Set default state
    /// </summary>
    protected override void InitializeValues()
    {
        if (_stateButton.IsStateOn)
            m_maximized.ApplyTransformDataInstant(this);
        else
            m_normal.ApplyTransformDataInstant(this);
    }

    protected override void ApplyColors()
    {
        ChangeState(true, _artist,_title);
        ChangeState(true,_yearCountry,_mainGenre,_subGenres, m_artist_title, m_total_length);
    }

    protected override void RegisterEventHandlers()
    {
        _stateButton.OnButtonClick.AddListener(OnStateButtonClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        _stateButton.OnButtonClick.RemoveListener(OnStateButtonClicked);
    }
    #endregion

    public void SetRelease(ReleaseResult release)
    {
        if (release != CurrentRelease)
        {
            _parent.StartSpinner();
            StartCoroutine(GetReleaseCore(release));
        }
        else
            m_tracks.ClearSelection();
    }

    IEnumerator GetReleaseCore(ReleaseResult release)
    {
        _image.overrideSprite = null;
        _image.color = _initialBackgroundColor;
        _artist.SetText(release.Artist.Text);
        _title.SetText(release.Title);
        m_artist_title.SetText(release.Artist.Text + " - " + release.Title);
        
        yield return null;

        ApiService.GetRelease(release.Id, (model) =>
        {
            ApiService.DownloadImage(model.ThumbnailUrl, ProcessImage);
            m_tracks.SetRelease(model);
            CurrentRelease = release;

            _parent.StopSpinner();
        });
    }

    void ProcessImage(Sprite sprite)
    {
        _image.overrideSprite = sprite;
        _image.DOFade(1f, .5f).SetEase(Ease.InSine);
    }

    void OnStateButtonClicked()
    {
        if (_stateButton.IsStateOn)
            m_maximized.ApplyTransformData(this);
        else
            m_normal.ApplyTransformData(this);
    }
}
