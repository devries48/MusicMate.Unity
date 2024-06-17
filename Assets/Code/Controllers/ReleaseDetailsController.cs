using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReleaseDetailsController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] Marquee _artistMarquee;
    [SerializeField] Marquee _titleMarquee;
    [SerializeField] Image _releaseImage;

    [Header("Spinner")]
    [SerializeField] Image _scanSpinnerBackground;
    [SerializeField] Image _scanSpinner;

    public ReleaseResult CurrentRelease { get; private set; } = null;

    internal CanvasGroup m_canvasGroup;

    IMusicMateManager _manager;
    IApiService _apiService;
    bool _loading;

    readonly float _speed = 1.5f;
    readonly Color32 _initialBackgroundColor = new(255, 255, 255, 3);

    void Awake()
    {
        gameObject.SetActive(false);

        _manager = MusicMateManager.Instance;
        _apiService = ApiService.Instance.GetClient();
    }

    void OnEnable()
    {
        if (_loading)
            StartCoroutine(GetReleaseCore());
    }

    void Update()
    {
        if (_loading)
            _scanSpinner.transform.Rotate(new Vector3(0, 0, -1 * _speed));
    }

    public void GetRelease(ReleaseResult release)
    {
        if (release != CurrentRelease)
        {
            _loading = true;
            CurrentRelease = release;
        }
    }

    public void CloseDetails()
    {
        _manager.ChangeVisiblePart(VisiblePart.ReleaseResult);
    }

    IEnumerator GetReleaseCore()
    {
        _scanSpinner.DOFade(1, .1f);
        _scanSpinnerBackground.DOFade(1, .1f);

        _loading = true;

        yield return null;

        _releaseImage.overrideSprite = null;
        _releaseImage.color = _initialBackgroundColor;
        _artistMarquee.SetText(CurrentRelease.Artist.Text);
        _titleMarquee.SetText(CurrentRelease.Title);

        yield return null;

        _apiService.GetRelease(CurrentRelease.Id, (model) =>
        {
            print("Show release: " + CurrentRelease.Title);

            _apiService.DownloadImage(model.ThumbnailUrl, ProcessImage);

            _scanSpinner.DOFade(0, .25f);
            _scanSpinnerBackground.DOFade(0, .25f);

            _loading = false;

        });
        // get complete release
        //_manager.ChangeState(_releaseImage, false);

    }

    void ProcessImage(Sprite sprite)
    {
        _releaseImage.overrideSprite = sprite;
        _releaseImage.DOFade(1f, .5f).SetEase(Ease.InSine);
    }

}
