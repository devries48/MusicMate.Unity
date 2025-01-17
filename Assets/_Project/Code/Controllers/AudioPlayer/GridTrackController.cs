using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridTrackController : MusicMateBehavior
{
    [SerializeField] bool _isPlaylist = true;
    [SerializeField] GameObject _prefabTrackRow;
    [SerializeField] GameObject _prefabActionPanel;
    [SerializeField] VerticalLayoutGroup _verticalLayout;

    Transform _trans;
    List<TrackResult> _tracklist;
    RowTrackAnimator _selectedRow;
    RectTransform _actionPanel;
    ActionPanelController _actionPanelController;
    int _currentIndex = -1;

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        if(_isPlaylist)
            PlayerService.SubscribeToActionChanged(OnActionChanged);
    }

    protected override void UnregisterEventHandlers()
    {
        if(_isPlaylist)
            PlayerService.UnsubscribeFromActionChanged(OnActionChanged);
    }

    protected override void InitializeComponents() { _trans = _verticalLayout.transform; }

    protected override void InitializeValues()
    {
        DestroyItems();

        _tracklist = new List<TrackResult>();

        _actionPanel = Instantiate(_prefabActionPanel, transform).GetComponent<RectTransform>();
        _actionPanelController = _actionPanel.GetComponent<ActionPanelController>();
        _actionPanel.gameObject.SetActive(false);
    }
    #endregion

    public void SetRelease(ReleaseModel release)
    {
        _actionPanel.gameObject.SetActive(false);
        _tracklist = release.GetAllTracks();
        _currentIndex = 0;

        StartCoroutine(SetTracklist());
    }

    public void ChangeSelection(RowTrackAnimator row)
    {
        //if(_selectedRow == row || !row.IsSelected)
        //    return;

        if(_selectedRow != null)
            _selectedRow.IsSelected = false;

        _selectedRow = row;

        if (row.IsSelected)
            ShowActionPanel(row);
        else
        {
            _selectedRow = null;
            HideActionPanel();
        }
    }

    public void ClearSelection()
    {
        if (_selectedRow != null)
            ChangeSelection(_selectedRow);
    }

    void HideActionPanel()
    {
        Animations.HideActionPanel(_actionPanel);
    }

    void ShowActionPanel(RowTrackAnimator track)
    {
        _actionPanel.SetParent(transform, false); 
        _actionPanel.anchoredPosition = CalculatePanelPosition(track.GetComponent<RectTransform>());
        _actionPanel.localScale = Vector3.zero;

        _actionPanelController.Initialize(track);

        Animations.ShowActionPanel(_actionPanel, track);
    }

    /// <summary>
    /// This track is active in the playlist
    /// </summary>
    void ActivateItem()
    {
        var index = _isPlaylist ? PlayerService.CurrentIndex : _currentIndex;
        var item = _trans.Find($"{index}_item");
        var ctrl = item.GetComponent<RowTrackAnimator>();

        ctrl.IsActive = true;
    }

    void DestroyItems()
    {
        for(int i = _trans.childCount - 1; i >= 0; i--)
            DestroyImmediate(_trans.GetChild(i).gameObject);
    }

    Vector2 CalculatePanelPosition(RectTransform trackRect)
    {
        Vector3[] trackCorners = new Vector3[4];
        trackRect.GetWorldCorners(trackCorners);

        var rightEdgeCenter = (trackCorners[2] + trackCorners[3]) / 2; // Right edge center
        var parentRect = transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            Camera.main.WorldToScreenPoint(rightEdgeCenter),
            Camera.main,
            out Vector2 localPosition
        );

        float xOffset = -_actionPanel.rect.width / 2; // Center horizontally to the right edge
        float yOffset = 0; // Adjust vertical alignment

        return localPosition + new Vector2(xOffset, yOffset);
    }

    IEnumerator SetTracklist()
    {
        DestroyItems();
        yield return null;

        var list = _isPlaylist ? PlayerService.GetPlaylist() : _tracklist;

        for(int i = 0; i < list.Count; i++)
        {
            var track = list[i];
            var item = Instantiate(_prefabTrackRow, _verticalLayout.transform);
            var controller = item.GetComponent<RowTrackAnimator>();
            controller.Initialize(track, i + 1, this);

            item.name = $"{i}_item";
            item.SetActive(true);

            yield return null;
        }

        if(_isPlaylist)
            ActivateItem();
    }

    void OnActionChanged(object sender, ActionChangedEventArgs e)
    {
        if(_isPlaylist && e.Action == PlayerAction.PlaylistChanged)
        {
            if(e.PlaylistAction == PlaylistAction.NewList)
                StartCoroutine(SetTracklist());

            if(e.PlaylistAction == PlaylistAction.None)
                ActivateItem();
        }
    }
}
