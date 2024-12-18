using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Settings")]
    [SerializeField] bool _isPrimary;
    [SerializeField] ButtonAnimationType _buttonType;

    [Header("Text Button")]
    [SerializeField] string _text;

    public ButtonInteractable Button { get; private set; }
    public UnityEvent OnButtonClick { get; private set; } = new UnityEvent();

    AnimationManager _animations;

    void Awake() => InitializeComponents();

    void Start() => InitializeValues();

    void OnEnable()
    {
        Button.onClick.AddListener(() => OnButtonClicked());
        Button.OnInteractableChanged += OnInteractableChanged;
    }

    void OnDisable()
    {
        Button.onClick.RemoveListener(() => OnButtonClicked());
        Button.OnInteractableChanged -= OnInteractableChanged;
    }

    void InitializeComponents()
    {
        _animations = AnimationManager.Instance;

        Button = (ButtonInteractable)GetComponent<Button>();
    }

    void InitializeValues()
    {
        Button.ImageComponent.color = _isPrimary ? MusicMateManager.Instance.AccentColor : MusicMateManager.Instance.DefaultColor;
        Button.TextComponent.color = _isPrimary ? MusicMateManager.Instance.AccentTextColor : MusicMateManager.Instance.TextColor;
        Button.TextComponent.text = _text;
    }

    void OnButtonClicked()
    {
        OnButtonClick?.Invoke();
        _animations.ButtonClicked(Button, _buttonType);
    }

    void OnInteractableChanged(bool isInteractable) => _animations.ButtonInteractableChanged(Button, isInteractable, _isPrimary, _buttonType);

    public void OnPointerEnter(PointerEventData eventData) => _animations.ButtonHoverEnter(Button, _buttonType);

    public void OnPointerExit(PointerEventData eventData) => _animations.ButtonHoverExit(Button, _buttonType);

#if UNITY_EDITOR
    void OnValidate()
    {
        InitializeComponents();
        InitializeValues();
    }
#endif

}
