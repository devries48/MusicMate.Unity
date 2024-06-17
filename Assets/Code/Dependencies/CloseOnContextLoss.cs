using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CloseOnContextLoss : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool _inContext;
    RectTransform _rectTransform;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.rotation = Quaternion.Euler(-90, 0, 0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_inContext)
            Hide();
    }

    public void Show()
    {
        _rectTransform.gameObject.SetActive(true);
        _rectTransform.DORotate(new Vector3(0, 0, 0), .25f).SetEase(Ease.OutSine);
    }

    public void Hide()
    {
        _rectTransform.DORotate(new Vector3(-90, 0, 0), .25f).SetEase(Ease.InSine)
            .OnComplete(() => _rectTransform.gameObject.SetActive(false));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _inContext = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _inContext = false;
    }
}