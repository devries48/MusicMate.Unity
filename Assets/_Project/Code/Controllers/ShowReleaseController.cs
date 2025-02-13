#region Usings
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

public class ShowReleaseController : MusicMateBehavior, IShowDetails<ReleaseResult, ReleaseModel>
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

    internal CanvasGroup m_canvasGroup;
    private ReleaseResult _currentRelease;

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
        ChangeState(true, _artist, _title);
        ChangeState(true, _yearCountry, _mainGenre, _subGenres, m_artist_title, m_total_length);
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

    void OnStateButtonClicked()
    {
        if (_stateButton.IsStateOn)
            m_maximized.ApplyTransformData(this);
        else
            m_normal.ApplyTransformData(this);
    }

    public void OnInit(ReleaseResult result)
    {
        if (_currentRelease != result)
        {
            _currentRelease = result;

            _parent.InitImage(_image);
            _artist.SetText(result.Artist.Text);
            _title.SetText(result.Title);
            m_artist_title.SetText(result.Artist.Text + " - " + result.Title);
        }
        else
            m_tracks.ClearSelection();
    }

    public void OnUpdated(ReleaseModel model)
    {
        _parent.GetImage(model.ThumbnailUrl, _image);
        m_tracks.SetRelease(model);
    }
}
