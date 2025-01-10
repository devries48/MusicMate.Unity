using DG.Tweening;
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

    //[Header("State Actions")]
    public TextMeshProUGUI m_hideWhenNormal;
    public CanvasGroup[] m_hideWhenMaximized;

    //[Header("Elements")]
    public RectTransform m_imagePanel;
    [SerializeField] Image _image;
    [SerializeField] Marquee _artist;
    [SerializeField] Marquee _title;
    public PlaylistController m_tracks;

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
        _image.overrideSprite = null;
        _image.color = _initialBackgroundColor;
        _artist.SetText(CurrentRelease.Artist.Text);
        _title.SetText(CurrentRelease.Title);

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
}
