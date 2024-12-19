using System;
using TMPro;
using UnityEngine.UI;

public class ButtonInteractable : Button
{
    public ColorSettings Colors;

    public Image ImageComponent
    {
        get
        {
            if (_imageComponent == null)
                _imageComponent = GetComponent<Image>();

            return _imageComponent;
        }
    }

    public TextMeshProUGUI TextComponent
    {
        get
        {
            if (_textComponent == null)
                _textComponent = GetComponentInChildren<TextMeshProUGUI>();

            return _textComponent;
        }
    }


    public event Action<bool> OnInteractableChanged;

    bool _interactable;
    Image _imageComponent;
    TextMeshProUGUI _textComponent;

    protected override void Awake()
    {
        base.Awake();

        _interactable = interactable;
    }

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