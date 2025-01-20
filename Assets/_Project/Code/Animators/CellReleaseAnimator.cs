#region Usings
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion

public class CellReleaseAnimator : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Fields
    RectTransform _rectTransform;
    Coroutine _clickCoroutine;

    Image _borderImage;
    Image _releaseImage;

    TextMeshProUGUI _artistText;
    TextMeshProUGUI _titleText;

    public bool IsSelected { get; set; } = false;

    bool _showText = true;
    readonly float _maxPanelScale = 2;

    internal GridReleaseController m_parent;
    internal RectTransform m_actionPanel;
    internal ReleaseResult m_release;
    #endregion

    #region Base Class Methods
    protected override void InitializeComponents()
    {
        _rectTransform = GetComponent<RectTransform>();

        transform.Find("Border").TryGetComponent(out _borderImage);
        transform.Find("Image").TryGetComponent(out _releaseImage);
        transform.Find("Artist").TryGetComponent(out _artistText);
        transform.Find("Title").TryGetComponent(out _titleText);
    }

    protected override void InitializeValues()
    {
        _borderImage.gameObject.SetActive(false);
    }
    #endregion

    public void Initialize(ReleaseResult model, GridReleaseController controller)
    {
        m_release = model;
        m_parent = controller;

        ApiService.DownloadImage(model.ThumbnailUrl, ProcessImage);

        _artistText.text = model.Artist.Text;
        _titleText.text = model.Title;
    }

    public void SetActionPanel(RectTransform rectPanel)
    {
        m_actionPanel = rectPanel;
        OnRectTransformDimensionsChange();
    }

    public void ShowRelease() => Manager.ShowRelease(m_release);

    public void ChangeSelectedState()
    {
        IsSelected = !IsSelected;

        m_parent.ChangeSelection(this); // Notify parent

        Animations.Grid.PlayCellSelect(IsSelected, this);
    }

    void OnRectTransformDimensionsChange()
    {
        if (_rectTransform == null )
            return;

        var width = _rectTransform.rect.width;
        if (width == 0)
            return;

        var showText = width > 350;

        if (showText != _showText)
        {
            _artistText.gameObject.SetActive(showText);
            _titleText.gameObject.SetActive(showText);
            _showText = showText;

            var rectTransform = _releaseImage.rectTransform;

            if (showText)
            {
                // Add a border of 20 around the image
                rectTransform.offsetMin = new Vector2(20, 20); // Left and Bottom
                rectTransform.offsetMax = new Vector2(-20, -20); // Right and Top
            }
            else
            {
                // Remove the border (image fully stretched)
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }
        }

        if (m_actionPanel != null)
        {
            var scale = width / 100;

            if (scale > _maxPanelScale)
                scale = _maxPanelScale;

            if (m_actionPanel.localScale.x != scale)
                m_actionPanel.localScale = new Vector3(scale, scale, 0);
        }
    }

    void ProcessImage(Sprite sprite)
    {
        _releaseImage.overrideSprite = sprite;
        _releaseImage.DOFade(1f, .5f).SetEase(Ease.InSine);
    }


    #region Pointer Event Handlers (Handles pointer hover and click events)
    public void OnPointerEnter(PointerEventData eventData) => Animations.Grid.PlayCellHoverEnter(this);

    public void OnPointerExit(PointerEventData eventData) => Animations.Grid.PlayCellHoverExit(this);

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            if (_clickCoroutine != null)
                StopCoroutine(_clickCoroutine);

            HandleDoubleClick();
        }
        else
            _clickCoroutine = StartCoroutine(HandleSingleClick());
    }

    IEnumerator HandleSingleClick()
    {
        yield return new WaitForSeconds(Constants.DoubleClickThreshold);
        ChangeSelectedState();
    }

    void HandleDoubleClick()
    {
        m_parent.ClearSelection();

        Animations.Grid.PlayCellClick(this);
        ShowRelease();
    }
     #endregion
}
