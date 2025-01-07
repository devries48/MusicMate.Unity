#region Usings
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

/// <summary>
/// The ButtonAnimator class is a custom Unity behavior designed to manage buttons with animations, including hover
/// effects, expand/collapse states, and icon rotations. It builds upon the ButtonInteractable class.
/// </summary>
[RequireComponent(typeof(ButtonInteractable))]
public class ButtonAnimator : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler
{
    #region Serialized Fields
    [SerializeField] bool _interactable;
    [SerializeField] bool _isPrimary;
    [SerializeField] ButtonAnimationType _buttonType;
    [SerializeField] string _text;   // Text Button
    [SerializeField] Sprite _icon;   // Image Button
    [SerializeField] bool _isToggle; // Expand/Collapse Button
    [SerializeField] bool _isExpanded;
    [SerializeField] string _headerText;
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

    protected override void InitializeComponents() { Button = (ButtonInteractable)GetComponent<Button>(); }

    protected override void InitializeValues()
    {
        try
        {
            Button.interactable = _interactable;

            if (Button.TextComponent != null)
            {
                Button.TextComponent.color = _isPrimary ? Button.Colors.AccentTextColor : Button.Colors.TextColor;
                Button.TextComponent.text = _buttonType == ButtonAnimationType.ExpandCollapseButton
                    ? _headerText
                    : _text;
            }

            if (Button.ImageComponent != null)
            {
                if (_buttonType == ButtonAnimationType.DefaultImageButton ||
                    _buttonType == ButtonAnimationType.LargeImageButton)
                {
                    if (_icon != null)
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
                }
                else if (_buttonType == ButtonAnimationType.ExpandCollapseButton)
                {
                    InitializeExpandCollapseButton();
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

    void InitializeExpandCollapseButton()
    {
        Button.ImageComponent.gameObject.SetActive(_interactable);
        Button.ImageComponent.color = Button.Colors.TextColor;

        // Initialize rotation based on state
        RotateIcon(_isExpanded);

        // Adjust PlaceholderTransform anchors based on _headerText
        if (Button.PlaceholderTransform != null)
        {
            if (string.IsNullOrEmpty(_headerText))
            {
                // Center the placeholder
                Button.PlaceholderTransform.anchorMin = new Vector2(0.5f, 0.5f);
                Button.PlaceholderTransform.anchorMax = new Vector2(0.5f, 0.5f);
                Button.PlaceholderTransform.pivot = new Vector2(0.5f, 0.5f);
                Button.PlaceholderTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                // Default alignment (e.g., right/middle)
                Button.PlaceholderTransform.anchorMin = new Vector2(1f, 0.5f);
                Button.PlaceholderTransform.anchorMax = new Vector2(1f, 0.5f);
                Button.PlaceholderTransform.pivot = new Vector2(1f, 0.5f);
                Button.PlaceholderTransform.anchoredPosition = Vector2.zero; // Adjust as necessary
            }

        }
    }
    #endregion

        /// <summary>
        /// Sets whether the button is interactable. This controls whether the user can interact with the button.
        /// </summary>
        /// <param name="interactable">Indicates whether the button should be interactable.</param>
    public void SetInteractable(bool interactable) => Button.interactable = interactable;

    /// <summary>
    /// Toggles the expanded state of the button and rotates the icon to reflect the new state.
    /// </summary>
    /// <param name="isExpanded">Indicates whether the button should be expanded (true) or collapsed (false).</param>
    public void SetExpanded(bool isExpanded)
    {
        if (!_isToggle)
            return;

        if (_isExpanded != isExpanded)
        {
            _isExpanded = isExpanded;
            RotateIcon(_isExpanded);
        }
    }

    public void SetHeader(string title)
    {
        _headerText = title;
        Button.TextComponent.text = title;
    }

    /// <summary>
    /// Rotates the button's icon based on the expanded state.
    /// </summary>
    /// <notes>
    /// If isExpanded is true, the icon rotates 180 degrees. If isExpanded is false, the icon resets to 0 degrees.
    /// </notes>
    void RotateIcon(bool isExpanded)
    {
        if (Button.ImageComponent == null)
            return;

        float targetRotation = isExpanded ? 180f : 0f;
        Button.ImageComponent.rectTransform.localRotation = Quaternion.Euler(0, 0, targetRotation);
    }

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
        if (Application.isPlaying) return;
        if (EditorApplication.isCompiling) return;

        Button = (ButtonInteractable)GetComponent<Button>();
        if (Button != null)
            InitializeValues();
    }
#endif

    #endregion
}
