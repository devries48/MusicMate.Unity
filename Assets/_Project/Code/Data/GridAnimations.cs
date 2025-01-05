using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Grid Animations", fileName = "Grid Animations")]
public class GridAnimations : ScriptableObject
{
    [Header("Grid Cell Click")]
    [SerializeField] float _cellClickScale = 0.8f;

    [Header("Grid Cell Hover")]
    [SerializeField] float _cellHoverScale = 1.1f;
    [SerializeField] float _cellHoverEnterTime = 0.1f;
    [SerializeField] Ease _cellHoverEnterEase = Ease.OutQuint;
    [SerializeField] float _cellHoverExitTime = 0.2f;
    [SerializeField] Ease _cellHoverExitEase = Ease.InCubic;

    [Header("Grid Cell Select")]
    [SerializeField] float _cellShowPanelTime = 0.15f;
    [SerializeField] Ease _cellShowPanelEase = Ease.OutBack;
    [SerializeField] float _cellHidePanelTime = 0.25f;
    [SerializeField] Ease _cellHidePanelEase = Ease.OutCirc;

    public void PlayCellHoverEnter(CellReleaseAnimator cell) => cell.transform
        .DOScale(_cellHoverScale, _cellHoverEnterTime)
        .SetEase(_cellHoverEnterEase);

    public void PlayCellHoverExit(CellReleaseAnimator cell) => cell.transform
        .DOScale(1f, _cellHoverExitTime)
        .SetEase(_cellHoverExitEase);

    public void PlayCellClick(CellReleaseAnimator cell)
    {
        if (cell.IsSelected)
            return;

        var duration = _cellShowPanelTime / 2;

        cell.transform.DOScale(_cellClickScale, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                cell.transform.DOScale(_cellHoverScale, duration ).SetEase(Ease.OutBack);
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
                .DOScale(_cellClickScale, duration / 2)
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
}

