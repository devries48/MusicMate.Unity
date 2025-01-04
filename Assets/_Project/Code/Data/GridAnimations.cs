using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Grid Animations", fileName = "Grid Animations")]
public class GridAnimations : ScriptableObject
{
    [Header("General Settings")]
    [SerializeField, Tooltip("Default animation duration")] float _animationTime = 0.2f;

    [Header("Grid Cell")]
    [SerializeField] float _cellHoverScale = 1.1f;
    [SerializeField] float _cellHoverEnterTime = 0.05f;
    [SerializeField] Ease _cellHoverEnterEase = Ease.OutQuint;
    [SerializeField] float _cellHoverExitTime = 0.15f;
    [SerializeField] Ease _cellHoverExitEase = Ease.InCubic;
    [SerializeField] float _cellClickScale = 0.9f;

    public void PlayHover(CellReleaseAnimator cell)
    {
        cell.transform.DOScale(_cellHoverScale, _cellHoverEnterTime).SetEase(_cellHoverEnterEase);
    }

    public void PlayNormal(CellReleaseAnimator cell)
    {
        cell.transform.DOScale(1f, _cellHoverExitTime).SetEase(_cellHoverExitEase);
    }

    public void PlayInteractable(
        ButtonInteractable button,
        Color32 backgroundColor,
        Color32 foreGroundColor,
        ButtonAnimationType buttonType)
    {
    }
}

