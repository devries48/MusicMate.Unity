using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/State/Panel Release State Data", fileName = "Panel State")]
public class PanelReleaseStateData : ScriptableObject
{
    [SerializeField] bool IsNormalState;
    [SerializeField] float _animationDuration = .1f;
    [SerializeField] RectTransformData _lengthData;
    [SerializeField] RectTransformData _imageData;
    [SerializeField] RectTransformData _tracksData;

    public void ApplyTransformData(ShowReleaseController controller)
    {
        var imagePanel = controller.m_imagePanel;
        var infoCanvas = controller.m_mainInfoPanel.GetComponent<CanvasGroup>();
        var tracks = controller.m_tracks.GetComponent<RectTransform>();
        var length = controller.m_total_length.GetComponent<RectTransform>();

        if (imagePanel.anchoredPosition != _imageData.anchoredPosition || imagePanel.sizeDelta != _imageData.sizeDelta)
        {
            imagePanel.DOAnchorPos(_imageData.anchoredPosition, _animationDuration);
            imagePanel.DOSizeDelta(_imageData.sizeDelta, _animationDuration);
        }

        if (tracks.anchoredPosition != _tracksData.anchoredPosition || tracks.sizeDelta != _tracksData.sizeDelta)
        {
            tracks.DOAnchorPos(_tracksData.anchoredPosition, _animationDuration);
            tracks.DOSizeDelta(_tracksData.sizeDelta, _animationDuration);
        }

        if (length.anchoredPosition != _lengthData.anchoredPosition || length.sizeDelta != _lengthData.sizeDelta)
        {
            length.DOAnchorPos(_lengthData.anchoredPosition, _animationDuration);
            length.DOSizeDelta(_lengthData.sizeDelta, _animationDuration);
        }

        var canvasFadeTo = IsNormalState ? 1 : 0;
        var titleFadeTo = IsNormalState ? 0 : 1;

        infoCanvas.DOFade(canvasFadeTo, _animationDuration)
                .SetEase(IsNormalState ? Ease.InQuint : Ease.OutQuint)
                .OnComplete(() => infoCanvas.interactable = IsNormalState);

        controller.m_artist_title.DOFade(titleFadeTo, _animationDuration)
             .SetEase(IsNormalState ? Ease.OutQuint : Ease.InQuint);
    }

    public void ApplyTransformDataInstant(ShowReleaseController controller)
    {
        var image = controller.m_imagePanel;

        if (image != null)
        {
            image.anchoredPosition = _imageData.anchoredPosition;
            image.sizeDelta = _imageData.sizeDelta;
        }

        if (controller.m_tracks != null)
        {
            var tracks = controller.m_tracks.GetComponent<RectTransform>();

            tracks.anchoredPosition = _tracksData.anchoredPosition;
            tracks.sizeDelta = _tracksData.sizeDelta;
        }

        if (controller.m_total_length != null)
        {
            var length = controller.m_total_length.GetComponent<RectTransform>();

            length.anchoredPosition = _lengthData.anchoredPosition;
            length.sizeDelta = _lengthData.sizeDelta;
        }

        var canvasAlpha = IsNormalState ? 1 : 0;
        var titleAlpha = IsNormalState ? 0 : 1;

        if (controller.m_mainInfoPanel != null)
        {
            var infoCanvas = controller.m_mainInfoPanel.GetComponent<CanvasGroup>();

            infoCanvas.alpha = canvasAlpha;
            infoCanvas.interactable = IsNormalState;
            infoCanvas.blocksRaycasts = IsNormalState;
        }

        if (controller.m_artist_title != null)
            controller.m_artist_title.alpha = titleAlpha;
    }

    public void SaveState(ShowReleaseController controller)
    {
        _imageData = GetData(controller.m_imagePanel);
        _tracksData = GetData(controller.m_tracks.GetComponent<RectTransform>());
        _lengthData = GetData(controller.m_total_length.GetComponent<RectTransform>());
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
