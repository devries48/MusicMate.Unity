using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowArtistController : MusicMateBehavior
{
    [Header("Reference")]
    [SerializeField] DetailsAnimator _parent;

    [Header("Elements")]
    [SerializeField] TextMeshProUGUI _artistText;
    [SerializeField] Image _artistImage;

    public DataResult CurrentArtist { get; private set; } = null;

    internal CanvasGroup m_canvasGroup;

    readonly Color32 _initialBackgroundColor = new(255, 255, 255, 3);


    #region Base Class Methods
    protected override void InitializeComponents()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void UnregisterEventHandlers()
    {
        StopAllCoroutines();
    }

    protected override void ApplyColors()
    {
        ChangeState(true, _artistText);
    }
    #endregion

    public void SetArtist(DataResult artist)
    {
        if (artist != CurrentArtist)
        {
            _parent.StartSpinner();
            StartCoroutine(GetArtistCore(artist));
        }
    }

    IEnumerator GetArtistCore(DataResult artist)
    {
        _artistImage.overrideSprite = null;
        _artistImage.color = _initialBackgroundColor;
        _artistText.text = artist.Text;

        yield return null;

        ApiService.GetArtist(artist.Id, (model) =>
        {
            //ApiService.DownloadImage(model.ThumbnailUrl, ProcessImage);
            CurrentArtist=artist;

            _parent.StopSpinner();
        });
    }

    void ProcessImage(Sprite sprite)
    {
        _artistImage.overrideSprite = sprite;
        _artistImage.DOFade(1f, .5f).SetEase(Ease.InSine);
    }

}
