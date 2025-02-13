using UnityEngine;

public class ShowCatalogController : MusicMateBehavior, IShowDetails<DataResult, ArtistModel>
{
    [Header("Reference")]
    [SerializeField] DetailsAnimator _parent;

    internal CanvasGroup m_canvasGroup;
    private DataResult _currentArtist;

    #region Base Class Methods
    protected override void InitializeComponents() => m_canvasGroup = GetComponent<CanvasGroup>();
    #endregion

    public void OnInit(DataResult result)
    {
        if (_currentArtist != result)
        {
            _currentArtist = result;
        }
    }

    public void OnUpdated(ArtistModel model){}
}
