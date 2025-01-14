using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TracklistController : MusicMateBehavior
{
    [SerializeField] bool _isPlaylist = true;
    [SerializeField] GameObject _prefabTrackTemplate;
    [SerializeField] GameObject _prefabTrackActions;
    [SerializeField] VerticalLayoutGroup _verticalLayout;

    Transform _trans;
     List<TrackResult> _tracklist;
    int _currentIndex = -1;

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        if (_isPlaylist) PlayerService.SubscribeToActionChanged(OnActionChanged);
    }

    protected override void UnregisterEventHandlers()
    {
        if (_isPlaylist) PlayerService.UnsubscribeFromActionChanged(OnActionChanged);
    }

    protected override void InitializeComponents()
    {
        _trans = _verticalLayout.transform;
    }

    protected override void InitializeValues()
    {
        DestroyItems();
        _tracklist = new List<TrackResult>();
    }
    #endregion

    public void SetRelease(ReleaseModel release)
    {
        _tracklist= release.GetAllTracks();
        _currentIndex = 0;
        StartCoroutine(SetTracklist());
    }

    void SelectItem()
    {
        var index= _isPlaylist ? PlayerService.CurrentIndex : _currentIndex;
        var item = _trans.Find($"{index}_item");
        var ctrl = item.GetComponent<RowTrackAnimator>();

        ctrl.IsSelected = true;
    }

    void DestroyItems()
    {
        for (int i = _trans.childCount - 1; i >= 0; i--)
            DestroyImmediate(_trans.GetChild(i).gameObject);
    }

    IEnumerator SetTracklist()
    {
        DestroyItems();
        yield return null;

        var list = _isPlaylist ? PlayerService.GetPlaylist(): _tracklist;

        for (int i = 0; i < list.Count; i++)
        {
            var track = list[i];
            var item = Instantiate(_prefabTrackTemplate, _verticalLayout.transform);
            var controller = item.GetComponent<RowTrackAnimator>();
            controller.Initialize(track, i + 1);

            item.name = $"{i}_item";
            item.SetActive(true);

            yield return null;
        }
        SelectItem();
    }

    void OnActionChanged(object sender, ActionChangedEventArgs e)
    {
        if (e.Action == PlayerAction.PlaylistChanged)
        {
            if (e.PlaylistAction == PlaylistAction.NewList)
                StartCoroutine(SetTracklist());

            if (e.PlaylistAction == PlaylistAction.None)
                SelectItem();
        }
    }

}
