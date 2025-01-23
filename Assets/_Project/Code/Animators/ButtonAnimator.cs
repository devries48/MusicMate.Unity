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
    [SerializeField] Sprite _stateIcon;   // Image State Button
    [SerializeField] bool _isToggle; // Expand/Collapse Button
    [SerializeField] bool _isExpanded;
    [SerializeField] bool _isStateOn;
    [SerializeField] string _headerText;
    #endregion

    #region Properties
    public ButtonInteractable Button { get; private set; }

    public bool IsStateOn { get { return _isStateOn; } }

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

            if(Button.TextComponent != null)
            {
                Button.TextComponent.text = _buttonType == ButtonAnimationType.ExpandCollapseButton
                    ? _headerText
                    : _text;
            }

            if(Button.ImageComponent != null)
            {
                if(_buttonType == ButtonAnimationType.DefaultImageButton ||
                    _buttonType == ButtonAnimationType.LargeImageButton ||
                    _buttonType == ButtonAnimationType.StateImageButton)
                {
                    if(_icon != null)
                    {
                        SetIcon();

                        var scale = _buttonType == ButtonAnimationType.LargeImageButton
                            ? Animations.Button.ImageButtonLargeScale
                            : Animations.Button.ImageButtonScale;

                        Button.transform.localScale = new Vector3(scale, scale, scale);
                    } else if(_buttonType == ButtonAnimationType.ExpandCollapseButton)
                    {
                        InitializeExpandCollapseButton();
                    }
                }
            }
        } catch(System.Exception)
        {
            //  Debug.LogError("Button InitializeValues Error (" + gameObject.gameObject.name + "/" + gameObject.name + ")");
        }
    }

    void InitializeExpandCollapseButton()
    {
        Button.ImageComponent.gameObject.SetActive(_interactable);

        // Initialize rotation based on state
        RotateIcon(_isExpanded);

        // Adjust PlaceholderTransform anchors based on _headerText
        if(Button.PlaceholderTransform != null)
        {
            if(string.IsNullOrEmpty(_headerText))
            {
                // Center the placeholder
                Button.PlaceholderTransform.anchorMin = new Vector2(0.5f, 0.5f);
                Button.PlaceholderTransform.anchorMax = new Vector2(0.5f, 0.5f);
                Button.PlaceholderTransform.pivot = new Vector2(0.5f, 0.5f);
                Button.PlaceholderTransform.anchoredPosition = Vector2.zero;
            } else
            {
                // Default alignment (e.g., right/middle)
                Button.PlaceholderTransform.anchorMin = new Vector2(1f, 0.5f);
                Button.PlaceholderTransform.anchorMax = new Vector2(1f, 0.5f);
                Button.PlaceholderTransform.pivot = new Vector2(1f, 0.5f);
                Button.PlaceholderTransform.anchoredPosition = Vector2.zero; // Adjust as necessary
            }
        }
    }

    protected override void ApplyColors()
    {
        if (Button==null)
            return;

        if(Button.TextComponent != null)
            Button.TextComponent.color = _isPrimary ? Manager.AppColors.AccentTextColor : Manager.AppColors.TextColor;

        if(Button.ImageComponent != null)
        {
            if(_buttonType == ButtonAnimationType.DefaultImageButton ||
                _buttonType == ButtonAnimationType.LargeImageButton ||
                _buttonType == ButtonAnimationType.StateImageButton)
            {
                if(_icon != null)
                {
                    Button.ImageComponent.color = !Button.interactable
                        ? Manager.AppColors.DisabledIconColor
                        : _isPrimary ? Manager.AppColors.AccentColor : Manager.AppColors.IconColor;
                }
            } else if(_buttonType != ButtonAnimationType.ExpandCollapseButton)
                Button.ImageComponent.color = Manager.AppColors.TextColor;
            else
                Button.ImageComponent.color = _isPrimary ? Manager.AppColors.AccentColor : Manager.AppColors.DefaultColor;
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
        if(!_isToggle)
            return;

        if(_isExpanded != isExpanded)
        {
            _isExpanded = isExpanded;
            RotateIcon(_isExpanded);
        }
    }

    public void SetState(bool isOn)
    {
        if(_isStateOn != isOn)
        {
            _isStateOn = isOn;
            SetIcon();
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
        if(Button.ImageComponent == null)
            return;

        float targetRotation = isExpanded ? 180f : 0f;
        Button.ImageComponent.rectTransform.localRotation = Quaternion.Euler(0, 0, targetRotation);
    }

    void SetIcon()
    {
        var icon = _icon;
        if(_buttonType == ButtonAnimationType.StateImageButton && _isStateOn)
            icon = _stateIcon;

        Button.ImageComponent.sprite = icon;
    }

    void OnButtonClicked()
    {
        Animations.Button.PlayClicked(Button, _buttonType);

        if(_buttonType == ButtonAnimationType.StateImageButton)
            SetState(!_isStateOn);

        OnButtonClick?.Invoke();
    }

    void OnInteractableChanged(bool isInteractable) => Animations.Button
        .PlayInteractableChanged(Button, isInteractable, _isPrimary, _buttonType);

    #region Pointer Event Handlers (Handles pointer hover events)
    public void OnPointerEnter(PointerEventData eventData) => Animations.Button.PlayHoverEnter(Button, _buttonType);

    public void OnPointerExit(PointerEventData eventData) => Animations.Button.PlayHoverExit(Button, _buttonType);
    #endregion

    #region Editor-Specific Code

#if UNITY_EDITOR

    // Suppress message: "SendMessage cannot be called during Awake, CheckConsistency, or OnValidate”  
    void OnValidate() { EditorApplication.delayCall += OnValidateDelayed; }

    void OnValidateDelayed()
    {
        if(Application.isPlaying)
            return;
        if(EditorApplication.isCompiling)
            return;

        InitializeValues();
        ApplyColors();
    }
#endif

    #endregion
}
