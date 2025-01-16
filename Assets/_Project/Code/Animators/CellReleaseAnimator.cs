#region Usings
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion
public class CellReleaseAnimator : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler
{
    #region Properties
    public bool IsSelected { get; set; } = false;
    #endregion

    ReleaseResult _releaseModel;
    GridReleaseController _parentController;

    internal ButtonInteractable m_cellButton;
    ButtonAnimator _playPauseButton;
    ButtonAnimator _showReleaseButton;
    ButtonAnimator _playlistButton;

    RectTransform _rectTransform;
    internal RectTransform m_actionPanel;

    Image _borderImage;
    Image _releaseImage;

    TextMeshProUGUI _artistText;
    TextMeshProUGUI _titleText;

    bool _showText = false;
    readonly float _maxPanelScale = 2;

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        PlayerService.SubscribeToStateChanged(OnPlayerStateChanged);

        _playPauseButton.OnButtonClick.AddListener(OnPlayOrPauseClicked);
        _showReleaseButton.OnButtonClick.AddListener(OnShowReleaseClicked);
        m_cellButton.onClick.AddListener(OnClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        PlayerService.UnsubscribeFromStateChanged(OnPlayerStateChanged);

        _playPauseButton.OnButtonClick.AddListener(OnPlayOrPauseClicked);
        _showReleaseButton.OnButtonClick.RemoveListener(OnShowReleaseClicked);
        m_cellButton.onClick.RemoveListener(OnClicked);
    }

    protected override void InitializeComponents()
    {
        _rectTransform = GetComponent<RectTransform>();
        m_cellButton = GetComponent<ButtonInteractable>();

        transform.Find("Border").TryGetComponent(out _borderImage);
        transform.Find("Image").TryGetComponent(out _releaseImage);
        transform.Find("Artist").TryGetComponent(out _artistText);
        transform.Find("Title").TryGetComponent(out _titleText);

        transform.Find("Panel Controls").TryGetComponent(out m_actionPanel);
        transform.Find("Panel Controls/Show Release").TryGetComponent(out _showReleaseButton);
        transform.Find("Panel Controls/Play or Pause").TryGetComponent(out _playPauseButton);
        transform.Find("Panel Controls/Playlist").TryGetComponent(out _playlistButton);
    }

    protected override void InitializeValues()
    {
        m_actionPanel.pivot = new Vector2(m_actionPanel.pivot.x, 1);
        m_actionPanel.gameObject.SetActive(false);
        _borderImage.gameObject.SetActive(false);
        //_selectionColor = m_cellButton.Colors.AccentColor;
        //_selectionColor.a = 0;
        //_borderImage.color = _selectionColor;
    }
    #endregion

    public void Initialize(ReleaseResult model, GridReleaseController controller)
    {
        _releaseModel = model;
        _parentController = controller;

        ApiService.DownloadImage(model.ThumbnailUrl, ProcessImage);

        _artistText.text = model.Artist.Text;
        _titleText.text = model.Title;
    }

    public void OnClicked()
    {
        Animations.CellClick(this);
        ChangeSelectedState();
    }

    public void OnPlayOrPauseClicked()
    {
        if(!PlayerService.IsPlaying)
            PlayerService.Play(_releaseModel);
        else
            PlayerService.Pause();
    }

    public void OnShowReleaseClicked() => Manager.ShowRelease(_releaseModel);

    public void ChangeSelectedState()
    {
        IsSelected = !IsSelected;

        _parentController.ChangeSelection(this); // Notify parent

        //_borderImage.DOFade(IsSelected ? 1 : 0, .25f);
        //.SetEase(IsSelected ? Ease.InSine : Ease.OutQuint);
        Animations.CellSelect(IsSelected, this);
    }

    void OnPlayerStateChanged(object sender, StateChangedEventArgs e) => StartCoroutine(SetPlayerState());

    void OnRectTransformDimensionsChange()
    {
        if(_rectTransform == null)
            return;

        var width = _rectTransform.rect.width;
        if(width == 0)
            return;

        var showText = width > 350;
        var scale = width / 100;

        if(scale > _maxPanelScale)
            scale = _maxPanelScale;

        if(m_actionPanel.localScale.x != scale)
            m_actionPanel.localScale = new Vector3(scale, scale, 0);

        if(showText != _showText)
        {
            _artistText.gameObject.SetActive(showText);
            _titleText.gameObject.SetActive(showText);
            _showText = showText;

            // Set anchors image to show or hide border
            _releaseImage.rectTransform.anchorMin = showText ? new Vector2(.05f, .05f) : Vector2.zero;
            _releaseImage.rectTransform.anchorMax = showText ? new Vector2(.95f, .95f) : Vector2.one;
        }
    }

    void ProcessImage(Sprite sprite)
    {
        _releaseImage.overrideSprite = sprite;
        _releaseImage.DOFade(1f, .5f).SetEase(Ease.InSine);
    }

    IEnumerator SetPlayerState()
    {
        Manager.AppState.ChangeState(_playPauseButton, PlayerService.IsActive, PlayerService.IsPlaying);

        yield return null;
    }

    #region Pointer Event Handlers (Handles pointer hover events)
    public void OnPointerEnter(PointerEventData eventData) => Animations.CellHoverEnter(this);

    public void OnPointerExit(PointerEventData eventData) => Animations.CellHoverExit(this);
    #endregion
}
