using System;
using TMPro;
using UnityEngine.UI;

public class ButtonInteractable : Button
{
    public event Action<bool> OnInteractableChanged;

    public Image ImageComponent { get; private set; }
    public TextMeshProUGUI TextComponent { get; private set; }

    protected SelectionState _prevState;

    protected override void Awake()
    {
        base.Awake();

        ImageComponent = GetComponent<Image>();
        TextComponent = GetComponentInChildren<TextMeshProUGUI>();
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        if (state != _prevState)
        {
            OnInteractableChanged?.Invoke(interactable);
            _prevState = state;
        }
    }
}