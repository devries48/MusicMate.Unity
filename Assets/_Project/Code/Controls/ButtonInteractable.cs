using System;
using TMPro;
using UnityEngine.UI;

public class ButtonInteractable : Button
{
    public event Action<bool> OnInteractableChanged;

    public Image ImageComponent { get; private set; }
    public TextMeshProUGUI TextComponent { get; private set; }

    bool _interactable;

    protected override void Awake()
    {
        base.Awake();

        ImageComponent = GetComponent<Image>();
        TextComponent = GetComponentInChildren<TextMeshProUGUI>();

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