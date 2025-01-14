using DG.Tweening;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Animations/Grid Animations", fileName = "Grid Animations")]
public class GridAnimations : ScriptableObject
{
    [Header("Grid Click")]
    [SerializeField] float _clickScale = 0.8f;

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

    public void PlayCellHoverEnter(CellReleaseAnimator cell) => Scale(cell.transform, _cellHoverScale, _cellHoverEnterTime, _cellHoverEnterEase);
    
    public void PlayCellHoverExit(CellReleaseAnimator cell) => Scale(cell.transform, 1f, _cellHoverExitTime , _cellHhoverExitEase);

    public void PlayCellClick(CellReleaseAnimator cell)
    {
        if (cell.IsSelected)
            return;

        var duration = _cellShowPanelTime / 2;

        cell.transform.DOScale(_clickScale, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                cell.transform.DOScale(_cellHoverScale, duration).SetEase(Ease.OutBack);
            });
    }

    public void PlayRowClick(RowTrackAnimator row)
    {
       // if (row.IsSelected)
       //     return;

        var duration = _cellShowPanelTime / 2;

        row.transform.DOScale(_clickScale, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                row.transform.DOScale(_cellHoverScale, duration).SetEase(Ease.OutBack);
            });
    }

    public void PlayCellSelect(bool isSelected, CellReleaseAnimator cell)
    {
        if (isSelected)
            cell.m_panelControls.gameObject.SetActive(true);

        var duration = isSelected ? _cellShowPanelTime : _cellHidePanelTime;
        var showPanel = cell.m_panelControls
            .DOPivotY(isSelected ? 0 : 1, duration)
            .SetEase(isSelected ? _cellShowPanelEase : _cellHidePanelEase)
            .Pause();

        if (isSelected)
        {
            cell.transform
                .DOScale(_clickScale, duration / 2)
                .SetEase(Ease.OutBack)
                .OnComplete(
                    () => cell.transform
                        .DOScale(_cellHoverScale, duration / 2)
                        .SetEase(Ease.OutBack)
                        .OnComplete(() => showPanel.Play()));
        }
        else
        {
            PlayCellHoverExit(cell);

            showPanel.OnComplete(
                () =>
                {
                    cell.m_panelControls.gameObject.SetActive(false);
                })
                .Play();
        }
    }

    internal void PlayRowSelect(bool isSelected, RowTrackAnimator row)
    {
        throw new NotImplementedException();
    }

    void Scale(Transform trans, float scale, float duration, Ease ease) => trans.DOScale(scale, duration).SetEase(ease);

}

