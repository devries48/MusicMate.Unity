using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class GridImportController : MusicMateBehavior, IGridController
{
    [SerializeField] GameObject prefabImportCell;
    [SerializeField] GameObject prefabActionPanel;
    
    RectTransform _parentTrans;
    GridLayoutGroup _importGrid;
    CellImportAnimator _selectedCell;

    public bool SidePanelExpanded { get; private set; }

    //internal CanvasGroup m_canvasGroup;
    const float SetMarginDuration = .5f;
    const float Margin = 10f;

    #region Base Class Methods
    protected override void InitializeComponents()
    {
        _parentTrans = transform.parent.GetComponent<RectTransform>();
        _importGrid = GetComponent<GridLayoutGroup>();
        //m_canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void InitializeValues()
    {
        //m_canvasGroup.alpha = 0f;
    }
    #endregion

    public void SetResult(PagedResult<ImportReleaseResult> result) => StartCoroutine(ProcessResult(result?.Items));

    /// <summary>
    /// Set the right margin for the result by setting the parent transforms' margin.
    /// The top margin is the bottom position of the toolbar and the border margin.
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
        // if (!cell.IsSelected) return;
        //
        // if (_selectedCell != null && _selectedCell != cell && _selectedCell.IsSelected)
        //     _selectedCell.ChangeSelectedState();
        //
        // _selectedCell = cell;
    }

    public void ClearSelection()
    {
        // if (_selectedCell != null)
        //     ChangeSelection(_selectedCell);
    }

    public RectTransform CreateActionPanel(ICellAnimator importCell)
    {
        var rectPanel = Instantiate(prefabActionPanel, importCell.CellTransform).GetComponent<RectTransform>();
        rectPanel.anchoredPosition = Vector2.zero;
        rectPanel.pivot = new Vector2(rectPanel.pivot.x, 1);

        var controller = rectPanel.GetComponent<ActionPanelController>();
        if (!controller)
            return null;
        
        var cell = (CellReleaseAnimator)importCell;
        controller.OnActionClicked += cell.OnActionClicked;
        controller.Initialize(cell);
        cell.SetActionPanel(rectPanel);

        return importCell.ActionPanel;
    }

    public void DestroyActionPanel(ICellAnimator importCell)
    {
        var controller = importCell.ActionPanel.GetComponent<ActionPanelController>();
        var cell = (CellReleaseAnimator)importCell;
        controller.OnActionClicked -= cell.OnActionClicked;

        Destroy(importCell.ActionPanel.gameObject);
    }

    IEnumerator ProcessResult(IReadOnlyList<ImportReleaseResult> result)
    {
        CalculateGridColumns();
        yield return null;

        var trans = _importGrid.transform;
        var count = trans.childCount;

        for (var i = count - 1; i >= 0; i--)
        {
            DestroyImmediate(trans.GetChild(i).gameObject);
            yield return null;
        }

        for (var i = 0; i < result.Count; i++)
        {
            var import = result[i];
            var cell = Instantiate(prefabImportCell, trans);
            var animator = cell.GetComponent<CellImportAnimator>();
            animator.Initialize(import, this);

            cell.name = $"{i}_import";
            cell.SetActive(true);

            yield return null;
        }
    }

    void CalculateGridColumns()
    {
        /*
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
    */
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (_parentTrans != null)
            CalculateGridColumns();
    }
#endif
}