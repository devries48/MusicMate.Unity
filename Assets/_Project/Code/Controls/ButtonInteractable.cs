using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A custom button class that extends Unity's Button functionality,
/// providing additional features such as color settings, a placeholder transform,
/// and easier access to associated UI components (text and image).
/// </summary>
public class ButtonInteractable : Button
{
    /// <summary>
    /// The color settings used to style the button's text and image.
    /// </summary>
    public ColorSettings Colors1;

    /// <summary>
    /// Gets the button's foreground image for a text button or the icon for other button types.
    /// </summary>
    public Image ImageComponent
    {
        get
        {
            if (_imageComponent == null)
            {
                _imageComponent = GetComponent<Image>();

                if (_imageComponent == null)
                {
                    var trans = transform.Find("Icon");
                    if (trans == null)
                        trans = transform.Find("Background/Placeholder/Icon");

                    trans.TryGetComponent(out _imageComponent);
                }
            }

            return _imageComponent;
        }
    }

    /// <summary>
    /// Gets the TextMeshPro component associated with the button, if present.
    /// </summary>
    public TextMeshProUGUI TextComponent
    {
        get
        {
            if (_textComponent == null)
                _textComponent = GetComponentInChildren<TextMeshProUGUI>();

            return _textComponent;
        }
    }

    /// <summary>
    /// Gets the RectTransform of the placeholder, typically used for positioning the button's icon or image.
    /// </summary>
    public RectTransform PlaceholderTransform
    {
        get
        {
            if (_placeholderTransform == null)
            {
                var trans = transform.Find("Background/Placeholder");
                if (trans != null)
                    trans.TryGetComponent(out _placeholderTransform);
            }

            return _placeholderTransform;
        }
    }

    /// <summary>
    /// Event triggered when the button's interactable state changes.
    /// </summary>
    public event Action<bool> OnInteractableChanged;

    // Private fields to store cached components and state.
    private bool _interactable;
    private Image _imageComponent;
    private TextMeshProUGUI _textComponent;
    private RectTransform _placeholderTransform;

    /// <summary>
    /// Unity's Awake method, called when the script instance is being loaded.
    /// Initializes internal state and caches the initial interactable state.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _interactable = interactable;
    }

    /// <summary>
    /// Overrides the base DoStateTransition method to add functionality
    /// for detecting changes in the interactable state and triggering the associated event.
    /// </summary>
    /// <param name="state">The new selection state.</param>
    /// <param name="instant">Whether the transition should occur instantly.</param>
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        if (_interactable != interactable)
        {
            OnInteractableChanged?.Invoke(interactable);
            _interactable = interactable;
        }
    }
}
