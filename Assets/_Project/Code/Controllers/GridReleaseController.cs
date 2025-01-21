using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class GridReleaseController : MusicMateBehavior
{
    [SerializeField] GameObject _prefabReleaseCell;
    [SerializeField] GameObject _prefabActionPanel;

    [Header("Thumbnail Size")]
    [SerializeField] ThumbnailSize _selectedSize;
    [SerializeField, Range(100, 149)] int _tiny = 100;
    [SerializeField, Range(150, 249)] int _small = 150;
    [SerializeField, Range(250, 349)] int _medium = 250;
    [SerializeField, Range(350, 449)] int _large = 350;

    RectTransform _parentTrans;
    GridLayoutGroup _releaseGrid;
    CellReleaseAnimator _selectedCell;

    internal CanvasGroup m_canvasGroup;

    const float _margin = 10f;

    #region Base Class Methods
    protected override void InitializeComponents()
    {
        _parentTrans = transform.parent.GetComponent<RectTransform>();
        _releaseGrid = GetComponent<GridLayoutGroup>();
        m_canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void InitializeValues()
    {
        m_canvasGroup.alpha = 0f;
    }
    #endregion

    public void SetResult(PagedResult<ReleaseResult> result) => StartCoroutine(ProcessResult(result?.Rows));

    /// <summary>
    /// Set the right margin for the result by setting the parent transforms' margin.
    /// The top margin is the bottom position of the toolbar + the border margin.
    /// </summary>
    /// <param name="margin"></param>
    public void SetRightMargin(bool isExpanded)
    {
        var margin = isExpanded ? (2 * _margin) + AudioPlayerService.Instance.PlayerWidth : _margin;
        var endval = new Vector2(-margin, -140);
        DOTween.To(() => _parentTrans.offsetMax, x => _parentTrans.offsetMax = x, endval, .5f);
    }

    public void ChangeSelection(CellReleaseAnimator cell)
    {
        if (!cell.IsSelected) return;

        if (_selectedCell != null && _selectedCell != cell && _selectedCell.IsSelected)
            _selectedCell.ChangeSelectedState();

        _selectedCell = cell;
    }

    public void ClearSelection()
    {
        if (_selectedCell != null)
            ChangeSelection(_selectedCell);
    }

    public RectTransform CreateActionPanel(CellReleaseAnimator release)
    {
        var rectPanel = Instantiate(_prefabActionPanel, release.transform).GetComponent<RectTransform>();
        rectPanel.anchoredPosition = Vector2.zero;
        rectPanel.pivot = new Vector2(rectPanel.pivot.x, 1);

        var controller = rectPanel.GetComponent<ActionPanelController>();
        controller.OnActionClicked += release.OnActionClicked;
        controller.Initialize(release);
        release.SetActionPanel(rectPanel);

        return release.m_actionPanel;
    }

    public void DestroyActionPanel(CellReleaseAnimator release)
    {
        var controller = release.m_actionPanel.GetComponent<ActionPanelController>();
        controller.OnActionClicked -= release.OnActionClicked;

        Destroy(release.m_actionPanel.gameObject);
    }

    IEnumerator ProcessResult(List<ReleaseResult> result)
    {
        CalculateGridColums();
        yield return null;

        var trans = _releaseGrid.transform;
        int childs = trans.childCount;

        for (int i = childs - 1; i >= 0; i--)
        {
            DestroyImmediate(trans.GetChild(i).gameObject);
            yield return null;
        }

        for (int i = 0; i < result.Count; i++)
        {
            var release = result[i];
            var cell = Instantiate(_prefabReleaseCell, trans);
            var controller = cell.GetComponent<CellReleaseAnimator>();
            controller.Initialize(release, this);

            cell.name = $"{i}_release";
            cell.SetActive(true);

            yield return null;
        }
    }

    void CalculateGridColums()
    {
        var cellSize = _selectedSize switch
        {
            ThumbnailSize.Tiny => _tiny,
            ThumbnailSize.Small => _small,
            ThumbnailSize.Medium => _medium,
            _ => _large
        };

        var minSpace = cellSize / 6f;
        var maxWidth = _parentTrans.rect.width;
        var offsetMax = Mathf.Abs(_parentTrans.offsetMax.x);

        if (offsetMax > _margin)
            maxWidth += (2 * _margin) + AudioPlayerService.Instance.PlayerWidth;

        var columns = (int)maxWidth / cellSize;
        var freeSpaceTot = maxWidth - (columns * cellSize);
        var cellSpaceTot = (columns - 2) * minSpace;

        if (freeSpaceTot - cellSpaceTot < 0)
        {
            columns--;
            freeSpaceTot = maxWidth - (columns * cellSize);
            cellSpaceTot = (columns - 2) * minSpace;
        }

        var cellSpace = minSpace;

        if (freeSpaceTot - cellSpaceTot > 0)
            cellSpace += (freeSpaceTot - cellSize) / (columns - 2);

        _releaseGrid.cellSize = new Vector2(cellSize, cellSize);
        _releaseGrid.spacing = new Vector2(cellSpace, minSpace);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (_parentTrans != null)
            CalculateGridColums();
    }
#endif
}