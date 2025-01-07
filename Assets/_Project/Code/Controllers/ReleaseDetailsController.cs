using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShowReleaseController : MusicMateBehavior
{
    [Header("Parent")]
    [SerializeField] ShowDetailsAnimator _showDetails;

    [Header("Elements")]
    [SerializeField] Marquee _artist;
    [SerializeField] Marquee _title;
    [SerializeField] Image _releaseImage;

    public ReleaseResult CurrentRelease { get; private set; } = null;

    internal CanvasGroup m_canvasGroup;

    readonly Color32 _initialBackgroundColor = new(255, 255, 255, 3);

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
        _releaseImage.overrideSprite = null;
        _releaseImage.color = _initialBackgroundColor;
        _artist.SetText(CurrentRelease.Artist.Text);
        _title.SetText(CurrentRelease.Title);

        yield return null;

        ApiService.GetRelease(CurrentRelease.Id, (model) =>
        {
            print("Show release: " + CurrentRelease.Title);

            ApiService.DownloadImage(model.ThumbnailUrl, ProcessImage);
            _showDetails.StopSpinner();

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
