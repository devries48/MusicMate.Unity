using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowReleaseController : MusicMateBehavior
{
    //[Header("Parent")]
    [SerializeField] DetailsAnimator _showDetails;

    //[Header("States")]
    public PanelReleaseStateData m_normal;
    public PanelReleaseStateData m_maximized;

    // Elements
    [SerializeField] Image _image;
    [SerializeField] Marquee _artist;
    [SerializeField] Marquee _title;
    public TextMeshProUGUI m_artist_title;
    public PlaylistController m_tracks;

    // Panels
    public RectTransform m_imagePanel;
    public RectTransform m_mainInfoPanel;

    // Buttons
    [SerializeField] ButtonAnimator _stateButton;
    [SerializeField] ButtonAnimator _upButton;
    [SerializeField] ButtonAnimator _downButton;

    public ReleaseResult CurrentRelease { get; private set; } = null;

    internal CanvasGroup m_canvasGroup;
    readonly Color32 _initialBackgroundColor = new(255, 255, 255, 3);

    #region Base Class Methods

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
            _showDetails.StartSpinner();
            CurrentRelease = release;
            StartCoroutine(GetReleaseCore());
        }
    }

    IEnumerator GetReleaseCore()
    {
        _image.overrideSprite = null;
        _image.color = _initialBackgroundColor;
        _artist.SetText(CurrentRelease.Artist.Text);
        _title.SetText(CurrentRelease.Title);
        m_artist_title.SetText(CurrentRelease.Artist.Text + " - " + CurrentRelease.Title);

        yield return null;

        ApiService.GetRelease(CurrentRelease.Id, (model) =>
        {
            ApiService.DownloadImage(model.ThumbnailUrl, ProcessImage);
            _showDetails.StopSpinner();

        });
        // get complete release
        //_manager.ChangeState(_releaseImage, false);
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
