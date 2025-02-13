using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowArtistController : MusicMateBehavior, IShowDetails<DataResult, ArtistModel>
{
    [Header("Reference")]
    [SerializeField] DetailsAnimator _parent;

    [Header("Elements")]
    [SerializeField] TextMeshProUGUI _artistText;
    [SerializeField] Image _artistImage;

    internal CanvasGroup m_canvasGroup;
    private DataResult _currentArtist;

    #region Base Class Methods
    protected override void InitializeComponents() => m_canvasGroup = GetComponent<CanvasGroup>();

    protected override void ApplyColors() => ChangeState(true, _artistText);
    #endregion

    public void OnInit(DataResult result)
    {
        if (_currentArtist != result)
        {
            _currentArtist = result;

            _parent.InitImage(_artistImage);
            _artistText.text = result.Text;
        }
    }

    public void OnUpdated(ArtistModel model)
    {
        _parent.GetImage(model.ThumbnailUrl, _artistImage);
    }
}
