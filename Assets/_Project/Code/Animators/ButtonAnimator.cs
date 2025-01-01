#region Usings
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

[RequireComponent(typeof(ButtonInteractable))]
public class ButtonAnimator : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler
{
    #region Serialized Fields
    [SerializeField] bool _interactable;
    [SerializeField] bool _isPrimary;
    [SerializeField] ButtonAnimationType _buttonType;

    [Header("Text Button")]
    [SerializeField] string _text;

    [Header("Image Button")]
    [SerializeField] Sprite _icon;
    #endregion

    #region Properties
    public ButtonInteractable Button { get; private set; }

    public UnityEvent OnButtonClick { get; private set; } = new UnityEvent();
    #endregion

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        Button.onClick.AddListener(OnButtonClicked);
        Button.OnInteractableChanged += OnInteractableChanged;
    }

    protected override void UnregisterEventHandlers()
    {
        Button.onClick.RemoveListener(OnButtonClicked);
        Button.OnInteractableChanged -= OnInteractableChanged;
    }

    protected override void InitializeComponents()
    {
        Button = (ButtonInteractable)GetComponent<Button>();
    }

    protected override void InitializeValues()
    {
        try
        {
            Button.interactable = _interactable;

            if (Button.TextComponent != null)
            {
                Button.TextComponent.color = _isPrimary ? Button.Colors.AccentTextColor : Button.Colors.TextColor;
                Button.TextComponent.text = _text;
            }

            if (Button.ImageComponent != null && _icon != null)
            {
                if (_buttonType == ButtonAnimationType.DefaultImageButton ||
                    _buttonType == ButtonAnimationType.LargeImageButton)
                {
                    Button.ImageComponent.sprite = _icon;
                    Button.ImageComponent.color = !Button.interactable
                        ? Button.Colors.DisabledIconColor
                        : _isPrimary ? Button.Colors.AccentColor : Button.Colors.IconColor;

                    var scale = _buttonType == ButtonAnimationType.DefaultImageButton
                        ? Animations.ImageButtonScale
                        : Animations.ImageButtonLargeScale;

                    Button.transform.localScale = new Vector3(scale, scale, scale);
                }
                else
                    Button.ImageComponent.color = _isPrimary ? Button.Colors.AccentColor : Button.Colors.DefaultColor;
            }
        }
        catch (System.Exception)
        {
          //  Debug.LogError("Button InitializeValues Error (" + gameObject.gameObject.name + "/" + gameObject.name + ")");
        }
    }
    #endregion

    public void SetInteractable(bool interactable) => Button.interactable = interactable;

    void OnButtonClicked()
    {
        OnButtonClick?.Invoke();
        Animations.ButtonClicked(Button, _buttonType);
    }

    void OnInteractableChanged(bool isInteractable) => Animations.ButtonInteractableChanged(
        Button,
        isInteractable,
        _isPrimary,
        _buttonType);

    #region Pointer Event Handlers (Handles pointer hover events)
    public void OnPointerEnter(PointerEventData eventData) => Animations.ButtonHoverEnter(Button, _buttonType);

    public void OnPointerExit(PointerEventData eventData) => Animations.ButtonHoverExit(Button, _buttonType);
    #endregion

    #region Editor-Specific Code
#if UNITY_EDITOR
    void OnValidate()
    {
        Button = (ButtonInteractable)GetComponent<Button>();
        if (Button != null)
            InitializeValues();
    }
#endif

    #endregion
}
