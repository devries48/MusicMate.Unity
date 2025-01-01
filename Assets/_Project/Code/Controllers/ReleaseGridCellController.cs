#region Usings
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

public class ReleaseGridCellController : MusicMateBehavior
{
    #region Serialized Fields
    [SerializeField] TextMeshProUGUI _artistText;
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] Image _releaseImage;
    [SerializeField] Color _selectionColor;

    [Header("Panel Controls")]
    [SerializeField] RectTransform _panelControls;
    [SerializeField] Button _playPauseButton;
    [SerializeField] Button _showReleaseButton;
    #endregion

    #region Properties
    public bool IsSelected
    {
        get => _isSelected; set
        {
            _isSelected = value;
            _animator.SetBool("IsSelected", IsSelected);
        }
    }
    bool _isSelected = false;
    #endregion

    ReleaseResult _releaseModel;

    Button _button;
    RectTransform _rectTransform;
    Animator _animator;
    Image _borderImage;
    ReleaseResultController _parentController;

    bool _showText = false;
    readonly float _maxPanelScale = 2;
    readonly float _slideTime = .25f;

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        PlayerService.SubscribeToStateChanged(OnPlayerStateChanged);
        _showReleaseButton.onClick.AddListener(OnShowReleaseClicked);
        _button.onClick.AddListener(OnClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        PlayerService.UnsubscribeFromStateChanged(OnPlayerStateChanged);
        _showReleaseButton.onClick.RemoveListener(OnShowReleaseClicked);
        _button.onClick.AddListener(OnClicked);
    }

    protected override void InitializeComponents()
    {
        _rectTransform = GetComponent<RectTransform>();
        _animator = GetComponent<Animator>();
        _borderImage = GetComponent<Image>();
        _button = GetComponent<Button>();
    }

    protected override void InitializeValues()
    {
        _selectionColor.a = 0;
         _borderImage.color = _selectionColor;
    }
    #endregion

    public void Initialize(ReleaseResult model, ReleaseResultController controller)
    {
        _releaseModel = model;
        _parentController = controller;

        ApiService.DownloadImage(model.ThumbnailUrl, ProcessImage);

        _artistText.text = model.Artist.Text;
        _titleText.text = model.Title;
    }

    public void OnClicked() => ChangeSelectedState();

    public void OnPlayOrPauseClicked()
    {
        if (!PlayerService.IsPlaying)
            PlayerService.Play(_releaseModel);
        else
            PlayerService.Pause();
    }

    public void OnShowReleaseClicked() => Manager.ShowRelease(_releaseModel);

    public void ChangeSelectedState()
    {
        IsSelected = !IsSelected;

        _parentController.ChangeSelection(this); // Notify parent

        _borderImage.DOFade(IsSelected ? 1 : 0, .25f);
        //.SetEase(IsSelected ? Ease.InSine : Ease.OutQuint);

        _panelControls.DOPivotY(IsSelected ? 0 : 1, _slideTime)
            .SetEase(IsSelected ? Ease.InBack : Ease.OutBack)
            .SetDelay(.25f);
    }

    void OnPlayerStateChanged(object sender, StateChangedEventArgs e) => StartCoroutine(SetPlayerState());

    void OnRectTransformDimensionsChange()
    {
        if (_rectTransform == null)
            return;

        var width = _rectTransform.rect.width;
        var showText = width > 350;
        var scale = width / 100;

        if (scale > _maxPanelScale)
            scale = _maxPanelScale;

        if (_panelControls.localScale.x != scale)
            _panelControls.localScale = new Vector3(scale, scale, 0);

        if (showText != _showText)
        {
            _artistText.gameObject.SetActive(showText);
            _titleText.gameObject.SetActive(showText);
            _showText = showText;
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
}
