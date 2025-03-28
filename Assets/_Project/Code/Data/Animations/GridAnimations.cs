using DG.Tweening;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Animations/Grid Animations", fileName = "Grid Animations")]
public class GridAnimations : ScriptableObject, IGridAnimations
{
    [Header("Grid Click")]
    [SerializeField] float _clickScale = 0.9f;
    [SerializeField] float _clickDuration = 0.2f;

    [Header("Cell Hover")]
    [SerializeField] float _cellHoverScale = 1.1f;
    [SerializeField] float _cellHoverEnterTime = 0.1f;
    [SerializeField] Ease _cellHoverEnterEase = Ease.OutQuint;
    [SerializeField] float _cellHoverExitTime = 0.2f;
    [SerializeField] Ease _cellHhoverExitEase = Ease.InCubic;

    [Header("Grid Cell Select")]
    [SerializeField] float _cellShowPanelTime = 0.15f;
    [SerializeField] Ease _cellShowPanelEase = Ease.OutBack;
    [SerializeField] float _cellHidePanelTime = 0.25f;
    [SerializeField] Ease _cellHidePanelEase = Ease.OutCirc;

    [Header("Grid Row Select")]
    [SerializeField] float _rowShowPanelTime = 0.1f;
    [SerializeField] Ease _rowShowPanelEase = Ease.OutBack;
    [SerializeField] float _rowHidePanelTime = 0.2f;
    [SerializeField] Ease _rowHidePanelEase = Ease.OutCirc;

    public void PlayCellHoverEnter(CellReleaseAnimator cell) => Scale(
        cell.transform,
        _cellHoverScale,
        _cellHoverEnterTime,
        _cellHoverEnterEase);

    public void PlayCellHoverExit(CellReleaseAnimator cell) => Scale(
        cell.transform,
        1f,
        _cellHoverExitTime,
        _cellHhoverExitEase);

    public void PlayCellClick(CellReleaseAnimator cell)
    {
        if (cell.IsSelected)
            return;

        var duration = _cellShowPanelTime / 2;

        cell.transform
            .DOScale(_clickScale, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(
                () =>
                {
                    cell.transform.DOScale(_cellHoverScale, duration).SetEase(Ease.OutBack);
                });
    }

    public void PlayRowClick(RowTrackAnimator row)
    {
        row.transform
            .DOScale(_clickScale, _clickDuration / 2)
            .SetEase(Ease.OutBack)
            .OnComplete(() => row.transform.DOScale(1, _clickDuration / 2).SetEase(Ease.OutBack));
    }
    public void PlayCellSelect(bool isSelected, CellReleaseAnimator cell) { CellSelect(isSelected, cell); }

    void CellSelect(bool isSelected, CellReleaseAnimator cell, bool isAbort = false, Action onComplete = null)
    {
        var duration = isSelected ? _cellShowPanelTime : _cellHidePanelTime;

        if (isAbort) duration = 0;

        if (isSelected)
        {
            cell.transform
                .DOScale(_clickScale, duration / 2)
                .SetEase(Ease.OutBack)
                .OnComplete(
                    () =>
                    {
                        cell.m_actionPanel = cell.m_parent.CreateActionPanel(cell);
                        cell.m_actionPanel
                            .DOPivotY(0, duration)
                            .SetEase(_cellShowPanelEase)
                            .OnComplete(() => onComplete?.Invoke());

                        cell.transform.DOScale(_cellHoverScale, duration / 2).SetEase(Ease.OutBack);
                    });
        }
        else
        {
            PlayCellHoverExit(cell);
            cell.m_actionPanel
                .DOPivotY(1, duration)
                .SetEase(_cellHidePanelEase)
                .OnComplete(
                    () =>
                    {
                        cell.m_parent.DestroyActionPanel(cell);
                        onComplete?.Invoke();
                    })
                .Play();
        }
    }

    public void AbortCellSelect(CellReleaseAnimator cell)
    {
        CellSelect(
            false,
            cell,
            true,
            () =>
            {
                cell.IsSelected = false;
            });
    }


    public void PlayShowActionPanel(RectTransform panel, RowTrackAnimator row)
    {
        panel.gameObject.SetActive(true);
        panel.DOScale(1, _rowShowPanelTime).SetEase(_rowShowPanelEase);

        PlayRowClick(row);
    }

    public void PlayHideActionPanel(RectTransform panel)
    {
        panel.DOScale(1, _rowHidePanelTime)
            .SetEase(_rowHidePanelEase)
            .OnComplete(() => panel.gameObject.SetActive(false));
    }

    void Scale(Transform trans, float scale, float duration, Ease ease) => trans.DOScale(scale, duration).SetEase(ease);
}

