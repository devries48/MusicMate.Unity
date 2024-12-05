using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReleaseGridCellController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _artistText;
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] Image _releaseImage;
    [SerializeField] Color _selectionColor;

    [Header("Panel Controls")]
    [SerializeField] RectTransform _panelControls;
    [SerializeField] Button _playPauseButton;
    [SerializeField] Button _showReleaseButton;

    IApiService _apiService;
    IAudioPlayerService _playerService;
    IMusicMateManager _manager;
    ReleaseResult _releaseModel;

    RectTransform _rectTransform;
    Animator _animator;
    Image _borderImage;
    ReleaseResultController _parentController;

    public bool IsSelected
    {
        get => _isSelected; set
        {
            _isSelected = value;
            _animator.SetBool("IsSelected", IsSelected);
        }
    }
    bool _isSelected = false;

    bool _showText = false;
    readonly float _maxPanelScale = 2;
    readonly float _slideTime = .25f;

    void Awake() => _playerService = AudioPlayerService.Instance;

    void OnEnable() => _playerService.SubscribeToStateChanged(OnPlayerStateChanged);

    void OnDisable() => _playerService.UnsubscribeFromStateChanged(OnPlayerStateChanged);

    void Start()
    {
        _apiService = ApiService.Instance.GetClient();
        _manager = MusicMateManager.Instance;

        _selectionColor.a = 0;
        _rectTransform = GetComponent<RectTransform>();
        _animator = GetComponent<Animator>();
        _borderImage = GetComponent<Image>();
        _borderImage.color = _selectionColor;

        var button = GetComponent<Button>();
        button.onClick.AddListener(() => OnClicked());

        _showReleaseButton.onClick.AddListener(() => OnShowReleaseClicked());

    }

    public void Initialize(ReleaseResult model, ReleaseResultController controller)
    {
        _releaseModel = model;
        _parentController = controller;

        _apiService = ApiService.Instance.GetClient();
        _apiService.DownloadImage(model.ThumbnailUrl, ProcessImage);

        _artistText.text = model.Artist.Text;
        _titleText.text = model.Title;
    }

    public void OnClicked() => ChangeSelectedState();

    public void OnPlayOrPauseClicked()
    {
        if (!_playerService.IsPlaying)
            _playerService.Play(_releaseModel);
        else
            _playerService.Pause();
    }

    public void OnShowReleaseClicked() => _manager.ShowRelease(_releaseModel);

    public void ChangeSelectedState()
    {
        IsSelected = !IsSelected;

        _parentController.ChangeSelection(this); // Notify parent

        _borderImage.DOFade(IsSelected ? 1 : 0,.25f);
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
        _manager.AppState.ChangeState(_playPauseButton, _playerService.IsActive, _playerService.IsPlaying);

        yield return null;
    }

}
