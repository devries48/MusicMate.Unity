using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/State/Panel Release State Data", fileName = "Panel State")]
public class PanelReleaseStateData : ScriptableObject
{
    [SerializeField] bool IsNormalState;
    [SerializeField] float _animationDuration = .2f;
    [SerializeField] RectTransformData _imageData;
    [SerializeField] RectTransformData _tracksData;

    public void ApplyTransformData(ShowReleaseController controller)
    {
        var image = controller.m_imagePanel;
        var tracks = controller.m_tracks.GetComponent<RectTransform>();

        if (image.anchoredPosition != _imageData.anchoredPosition || image.sizeDelta != _imageData.sizeDelta)
            image.DOAnchorPos(_imageData.anchoredPosition, _animationDuration);

        if (tracks.anchoredPosition != _tracksData.anchoredPosition || tracks.sizeDelta != _tracksData.sizeDelta)
            image.DOSizeDelta(_tracksData.sizeDelta, _animationDuration);

        var canvasFadeTo = IsNormalState ? 1 : 0;
        var titleFadeTo = IsNormalState ? 0 : 1;

        foreach (var group in controller.m_hideWhenMaximized)
            group.DOFade(canvasFadeTo, _animationDuration)
                .SetEase(IsNormalState ? Ease.InQuint : Ease.OutQuint)
                .OnComplete(() => group.interactable = IsNormalState);

        if (controller.m_hideWhenNormal != null)
            controller.m_hideWhenNormal.DOFade(titleFadeTo, _animationDuration)
                 .SetEase(IsNormalState ? Ease.OutQuint : Ease.InQuint);
    }

    public void ApplyTransformDataInstant(ShowReleaseController controller)
    {
        var image = controller.m_imagePanel;
        var tracks = controller.m_tracks.GetComponent<RectTransform>();

        // Direct property assignments
        image.anchoredPosition = _imageData.anchoredPosition;
        image.sizeDelta = _imageData.sizeDelta;

        tracks.anchoredPosition = _tracksData.anchoredPosition;
        tracks.sizeDelta = _tracksData.sizeDelta;

        var canvasAlpha = IsNormalState ? 1 : 0;
        var titleAlpha = IsNormalState ? 0 : 1;

        foreach (var group in controller.m_hideWhenMaximized)
        {
            group.alpha = canvasAlpha;
            group.interactable = IsNormalState;
            group.blocksRaycasts = IsNormalState;
        }

        if (controller.m_hideWhenNormal != null)
            controller.m_hideWhenNormal.alpha = titleAlpha;
    }

    public void SaveState(ShowReleaseController controller)
    {
        _imageData = GetData(controller.m_imagePanel);
        _tracksData = GetData(controller.m_tracks.GetComponent<RectTransform>());
    }

    RectTransformData GetData(RectTransform rect)
    { return new RectTransformData { anchoredPosition = rect.anchoredPosition, sizeDelta = rect.sizeDelta }; }

    [System.Serializable]
    public class RectTransformData
    {
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
    }
}
