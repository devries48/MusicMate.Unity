using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] bool _isPrimary;
    [SerializeField] ButtonAnimationType _buttonType;

    AnimationManager _animations;
    ButtonInteractable _button;

    void Awake()
    {
        _animations = AnimationManager.Instance;
        _button = (ButtonInteractable)GetComponent<Button>();
     }

    void OnEnable()
    {
        _button.onClick.AddListener(() => OnButtonClicked());
        _button.OnInteractableChanged += OnInteractableChanged;
    }

    void OnDisable()
    {
        _button.onClick.RemoveListener(() => OnButtonClicked());
        _button.OnInteractableChanged -= OnInteractableChanged;
    }

    void OnButtonClicked() => _animations.ButtonClicked(_button, _buttonType);

    void OnInteractableChanged(bool isInteractable) => _animations.ButtonInteractableChanged(_button,isInteractable, _isPrimary, _buttonType);

    public void OnPointerEnter(PointerEventData eventData)
    {
        _animations.ButtonHoverEnter(_button, _buttonType);
    }

    public void OnPointerExit(PointerEventData eventData) => _animations.ButtonHoverExit(_button, _buttonType);
}
