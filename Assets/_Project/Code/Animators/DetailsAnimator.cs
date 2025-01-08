using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DetailsAnimator : MusicMateBehavior
{
    [Header("Show Details Controllers")]
    [SerializeField] ShowReleaseController _releaseDetails;
    [SerializeField] ArtistDetailsController _artistDetails;

    [Header("Elements")]
    [SerializeField] ButtonInteractable _closeButton;
    [SerializeField] Image _spinnerBackground;
    [SerializeField] Image _spinner;

    public bool IsLoading { get; set; }

    readonly float _speed = 1.5f;

    protected override void RegisterEventHandlers()
    {
        _closeButton.onClick.AddListener(OnCloseClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        _closeButton.onClick.RemoveListener(OnCloseClicked);
    }

    void Update()
    {
        if (IsLoading)
            _spinner.transform.Rotate(new Vector3(0, 0, -1 * _speed));
    }

    public void SetRelease(ReleaseResult release)
    {
        _releaseDetails.SetRelease(release);
    }

    public void StartSpinner()
    {
        _spinner.DOFade(1, .1f);
        _spinnerBackground.DOFade(1, .1f);

        IsLoading = true;
    }

    public void StopSpinner()
    {
        _spinner.DOFade(0, .25f).SetDelay(.25f);
        _spinnerBackground.DOFade(0, .25f).SetDelay(.25f).OnComplete(() =>
        {
            IsLoading = false;
        });
    }

    public void CloseDetails()
    {
        Animations.PanelShowDetailsVisible(false, this);
        Manager.AppState.ChangeVisiblePart(VisiblePart.ReleaseResult);
    }

    void OnCloseClicked() => CloseDetails();

    /*
        void MoveReleaseDetails(bool show, float delay = 0)
        {
            var pivotTo = show ? .5f : 2f;
            var easing = show ? Ease.OutBack : Ease.InBack;
            var rect = _releaseDetails.gameObject.GetComponent<RectTransform>();

            rect.DOPivotY(pivotTo, _popupTime).SetEase(easing).SetDelay(delay);

            _state.ReleaseDetails = show ? State.States.visible : State.States.moved;
        }

        void VisibleArtistDetails(bool show, float delay = 0)
        {
            if (show)
                ScaleIn(_artistDetails.transform, delay);
            else
                ScaleOut(_artistDetails.transform);

            _state.ReleaseDetailsArtist = show ? State.States.visible : State.States.hidden;
        }

    

        void ScaleIn(Transform trans, float delay = 0)
        {
            trans.localScale = Vector3.zero;
            trans.gameObject.SetActive(true);
            trans.DOScale(1, _popupTime).SetEase(Ease.OutBack).SetDelay(delay);
        }

        void ScaleOut(Transform trans)
        {
            trans.DOScale(0, _popupTime).SetEase(Ease.InBack)
                .OnComplete(() => trans.gameObject.SetActive(false));
        }

           void OnVisiblePartChanged(object sender, VisiblePartChangedEventArgs e)
        {
            var p = e.Part;
            if (p == VisiblePart.Previous)
            {
                if (_state.ReleaseDetailsArtist == State.States.visible)
                {
                    p = VisiblePart.ReleaseDetails;
                    VisibleArtistDetails(false);
                }
            }

            switch (p)
            {
                case VisiblePart.ReleaseResult:
                    VisibleReleaseResult(true);

                    if (_state.ReleaseDetails == State.States.visible)
                        VisibleReleaseDetails(false);

                    break;

                case VisiblePart.ReleaseDetails:
                    if (_state.ReleaseDetails == State.States.moved)
                        MoveReleaseDetails(true, _popupTime);
                    else
                        VisibleReleaseDetails(true);

                    if (_state.ReleaseDetailsArtist == State.States.visible)
                        VisibleArtistDetails(false);

                    if (_state.ReleaseResult == State.States.visible)
                        VisibleReleaseResult(false);

                    break;

                case VisiblePart.ArtistDetails:
                    VisibleArtistDetails(true, _popupTime);
                    MoveReleaseDetails(false);
                    break;

                default:
                    break;
            }
        }

    */

}
