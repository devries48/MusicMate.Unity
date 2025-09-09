using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class GridReleaseController : MusicMateBehavior,IGridController
{
    [SerializeField] GameObject prefabReleaseCell;
    [SerializeField] GameObject prefabActionPanel;
    
    [Header("Thumbnail Size")]
    [SerializeField] ThumbnailSize selectedSize;
    [SerializeField, Range(100, 149)] int tiny = 100;
    [SerializeField, Range(150, 249)] int small = 150;
    [SerializeField, Range(250, 349)] int medium = 250;
    [SerializeField, Range(350, 449)] int large = 350;

    RectTransform _parentTrans;
    GridLayoutGroup _releaseGrid;
    CellReleaseAnimator _selectedCell;

    public bool SidePanelExpanded { get; private set; }

    internal CanvasGroup m_canvasGroup;
    const float SetMarginDuration = .5f;
    const float Margin = 10f;

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

    public void SetResult(PagedResultOld<ReleaseResult> result) => StartCoroutine(ProcessResult(result?.Rows));

    /// <summary>
    /// Set the right margin for the result by setting the parent transforms' margin.
    /// The top margin is the bottom position of the toolbar + the border margin.
    /// </summary>
    public void SetRightMargin(bool isExpanded, float delay = 0)
    {
        SidePanelExpanded = isExpanded;
        var margin = isExpanded ? (2 * Margin) + Constants.SidePanelWidth : Margin;
        var endVal = new Vector2(-margin, -140);
        DOTween.To(() => _parentTrans.offsetMax, x => _parentTrans.offsetMax = x, endVal, SetMarginDuration).SetDelay(delay);
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

    public RectTransform CreateActionPanel(ICellAnimator releaseCell)
    {
        var rectPanel = Instantiate(prefabActionPanel, releaseCell.CellTransform).GetComponent<RectTransform>();
        rectPanel.anchoredPosition = Vector2.zero;
        rectPanel.pivot = new Vector2(rectPanel.pivot.x, 1);

        var controller = rectPanel.GetComponent<ActionPanelController>();
        var cell = (CellReleaseAnimator)releaseCell;
        controller.OnActionClicked += cell.OnActionClicked;
        controller.Initialize(cell);
        cell.SetActionPanel(rectPanel);

        return releaseCell.ActionPanel;
    }

    public void DestroyActionPanel(ICellAnimator releaseCell)
    {
        var controller = releaseCell.ActionPanel.GetComponent<ActionPanelController>();
        var cell = (CellReleaseAnimator)releaseCell;
        controller.OnActionClicked -= cell.OnActionClicked;

        Destroy(releaseCell.ActionPanel.gameObject);
    }

    IEnumerator ProcessResult(List<ReleaseResult> result)
    {
        CalculateGridColumns();
        yield return null;

        var trans = _releaseGrid.transform;
        var count = trans.childCount;

        for (var i = count - 1; i >= 0; i--)
        {
            DestroyImmediate(trans.GetChild(i).gameObject);
            yield return null;
        }

        for (int i = 0; i < result.Count; i++)
        {
            var release = result[i];
            var cell = Instantiate(prefabReleaseCell, trans);
            var animator = cell.GetComponent<CellReleaseAnimator>();
            animator.Initialize(release, this);

            cell.name = $"{i}_release";
            cell.SetActive(true);

            yield return null;
        }
    }

    void CalculateGridColumns()
    {
        var cellSize = selectedSize switch
        {
            ThumbnailSize.Tiny => tiny,
            ThumbnailSize.Small => small,
            ThumbnailSize.Medium => medium,
            _ => large
        };

        var minSpace = cellSize / 6f;
        var maxWidth = _parentTrans.rect.width;
        var offsetMax = Mathf.Abs(_parentTrans.offsetMax.x);

        if (offsetMax > Margin)
            maxWidth += (2 * Margin) + AudioPlayerService.Instance.PlayerWidth;

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
            CalculateGridColumns();
    }
#endif
}