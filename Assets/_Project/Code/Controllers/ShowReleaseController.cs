#region Usings
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion
public class ShowReleaseController : MusicMateBehavior, IShowDetails<ReleaseResult, ReleaseModel>
{
    #region Serialized Fields
    [SerializeField] DetailsAnimator _parent;
    [SerializeField] CountryFlags countryFlags;
    public PanelReleaseStateData m_normal;
    public PanelReleaseStateData m_maximized;

    // Elements
    [SerializeField] Image _image;
    [SerializeField] Marquee _artist;
    [SerializeField] Marquee _title;
    [SerializeField] TextMeshProUGUI _type;
    [SerializeField] TextMeshProUGUI _yearCountry;
    [SerializeField] Image countryFlag;
    [SerializeField] TextMeshProUGUI _mainGenre;
    [SerializeField] TextMeshProUGUI _subGenres;
    [SerializeField] TextMeshProUGUI _label;

    public TextMeshProUGUI m_artist_title;
    public TextMeshProUGUI m_total_length;
    public GridTrackController m_tracks;

    [SerializeField] private TextMeshProUGUI _info;

    // Panels & Zone's
    public RectTransform m_mainInfoPanel;
    public RectTransform m_imageZone;
    public ZoneAnimator[] _allZones;

    // Buttons
    [SerializeField] ButtonAnimator _stateButton;
    [SerializeField] ButtonAnimator _upButton;
    [SerializeField] ButtonAnimator _downButton;
    #endregion

    internal CanvasGroup m_canvasGroup;
    ReleaseResult _currentRelease;

    #region Base Class Methods
    protected override void InitializeComponents() { m_canvasGroup = GetComponent<CanvasGroup>(); }

    protected override void ApplyColors()
    {
        var editMode = Manager.AppState.CurrentMode == MusicMateMode.Edit;
        
        if (editMode &&
            _stateButton.IsStateOn &&
            _stateButton.gameObject.activeInHierarchy)
            m_normal.ApplyTransformDataInstant(this);
        else if (!editMode &&
            _stateButton.IsStateOn &&
            !_stateButton.gameObject.activeInHierarchy)
            m_maximized.ApplyTransformDataInstant(this);

        foreach (var zone in _allZones)
            zone.gameObject.SetActive(editMode);

        _stateButton.gameObject.SetActive(Manager.AppState.CurrentMode != MusicMateMode.Edit);
        _upButton.gameObject.SetActive(Manager.AppState.CurrentMode != MusicMateMode.Edit);
        _downButton.gameObject.SetActive(Manager.AppState.CurrentMode != MusicMateMode.Edit);

        ChangeState(true, _artist, _title);
        ChangeState(true, _type, _yearCountry, _mainGenre, _subGenres, _label, m_artist_title, m_total_length);
        ChangeState(true, _info);
    }

    protected override void RegisterEventHandlers() { _stateButton.OnButtonClick.AddListener(OnStateButtonClicked); }

    protected override void UnregisterEventHandlers()
    { _stateButton.OnButtonClick.RemoveListener(OnStateButtonClicked); }
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
            _type.text=string.Empty;
            SetReleaseYearCountry(result.ReleaseYear, result.Country);
            _mainGenre.text = result.MainGenre;
            _subGenres.text = string.Empty;
            _label.text = string.Empty;
            _info.text = string.Empty;  
            
            m_artist_title.SetText($"{result.Artist.Text} - {result.Title}");
        }
        else
            m_tracks.ClearSelection();
    }

    public void OnUpdated(ReleaseModel model)
    {
        _type.text = model.ReleaseType?.ToString();
        _subGenres.text = ReleaseSubGenres(model);
        _label.text=ReleaseLabels(model);
        _info.text = model.Info;
        
        _parent.GetImage(model.ThumbnailUrl, _image);
        m_tracks.SetRelease(model);
    }

    void SetReleaseYearCountry(int releaseYear, string country)
    {
        var result = string.Empty;

        if (releaseYear >0)
            result = releaseYear.ToString();

        if (country == null)
        {
            _yearCountry.text = result;
            countryFlag.enabled = false;
            return ;
        }
        
        if (result.Length > 0 && !string.IsNullOrWhiteSpace(country))
            result += ", ";

        result += CountryFlags.GetCountryName(country);
        _yearCountry.text = result;
        countryFlag.sprite = countryFlags.GetFlag(country);
        countryFlag.enabled = true;
    }

    static string ReleaseSubGenres(ReleaseModel model)
    {
        var result = string.Empty;

        foreach (var genre in model.Genres)
        {
            if (genre.Text == model.MainGenre)
                continue;

            if (result.Length > 0)
                result += ", ";

            result += genre.Text;
        }

        return result;
    }

    static string ReleaseLabels(ReleaseModel model)
    {
        return model.Labels?.FirstOrDefault()?.Label.Text ?? string.Empty;
    }

    internal void UpdateModel(MusicMateZone zone, ReleaseModel releaseModel)
    {
        throw new NotImplementedException();
    }
}
