using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class ReleaseResultController : MonoBehaviour
{
    [SerializeField] GameObject _prefabReleaseTemplate;
    
    [Header("Thumbnail Size")]
    [SerializeField] ThumbnailSize _selectedSize;
    [SerializeField, Range(100, 149)] int _tiny = 100;
    [SerializeField, Range(150, 249)] int _small = 150;
    [SerializeField, Range(250, 349)] int _medium = 250;
    [SerializeField, Range(350, 449)] int _large = 350;

    internal CanvasGroup m_canvasGroup;
    RectTransform _parentTrans;

    GridLayoutGroup _releaseGrid;
    ReleaseGridCellController _selectedCell;

    const float _margin = 10f;

    public enum ThumbnailSize { Tiny, Small, Medium, Large };

    void Awake()
    {
        _parentTrans = transform.parent.GetComponent<RectTransform>();
        _releaseGrid = GetComponent<GridLayoutGroup>();
        m_canvasGroup = GetComponent<CanvasGroup>();
        m_canvasGroup.alpha = 0f;
    }

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

    public void ChangeSelection(ReleaseGridCellController cell)
    {
        if (!cell.IsSelected) return;

        if (_selectedCell != null && _selectedCell != cell && _selectedCell.IsSelected)
            _selectedCell.ChangeSelectedState();

        _selectedCell = cell;
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
            var cell = Instantiate(_prefabReleaseTemplate, trans);
            var controller = cell.GetComponent<ReleaseGridCellController>();
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

        var minSpace = cellSize / 10f;
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