using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtistDetailsController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] ShowReleaseController _releaseDetails;

    [Header("Elements")]
    [SerializeField] TextMeshProUGUI _artistText;
    [SerializeField] Image _artistImage;

    [Header("Spinner")]
    [SerializeField] Image _spinnerBackground;
    [SerializeField] Image _spinner;

    internal CanvasGroup m_canvasGroup;

    IMusicMateManager _manager;
    IMusicMateApiService _apiService;
    bool _loading;

    readonly float _speed = 1.5f;
    readonly Color32 _initialBackgroundColor = new(255, 255, 255, 3);

    void Awake()
    {
        _manager = MusicMateManager.Instance;
        _apiService = MusicMateApiService.Instance.GetClient();
    }

    void OnEnable()
    {
            StartCoroutine(GetArtistCore());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        _loading = false;
    }

    void Update()
    {
        if (_loading)
            _spinner.transform.Rotate(new Vector3(0, 0, -1 * _speed));
    }

    public void CloseDetails()
    {
        _manager.AppState.InvokeStateChanged(MusicMateStatePart.Previous);
    }

    IEnumerator GetArtistCore()
    {
        var data = _releaseDetails.CurrentRelease.Artist;

        if (data.Text == _artistText.text)
            yield break;

        _spinner.DOFade(1, .1f);
        _spinnerBackground.DOFade(1, .1f);

        _loading = true;

        yield return null;

        _artistImage.overrideSprite = null;
        _artistImage.color = _initialBackgroundColor;
        _artistText.text = data.Text;

        yield return null;

        //_apiService.GetArtist(data.Id, (model) =>
        //{
        //    print("Show artist: " + data.Text);

        //    _apiService.DownloadImage(model.ThumbnailUrl, ProcessImage);

            _spinner.DOFade(0, .25f);
            _spinnerBackground.DOFade(0, .25f);

            _loading = false;

        //});
    }

    void ProcessImage(Sprite sprite)
    {
        _artistImage.overrideSprite = sprite;
        _artistImage.DOFade(1f, .5f).SetEase(Ease.InSine);
    }

}
