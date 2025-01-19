using TMPro;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Animations/Input Animations", fileName = "Input Animations")]
public class InputAnimations : ScriptableObject,IInputAnimations
{
    [Header("InputText")]
    [SerializeField] float _textTransitionDuration = .2f;

    IMusicMateManager _manager;

    public void Initialize(IMusicMateManager manager) => _manager = manager;

    public void PlayTextNormal(TMP_InputField input)
    {
        SetColor(input, _manager.TextColor);
        SetBackgroundColor(input, _manager.BackgroundColor);
    }

    public void PlayTextSelect(TMP_InputField input)
    {
        SetColor(input, _manager.AccentColor);
    }

    public void PlayTextHighlight(TMP_InputField input)
    {
        SetColor(input, _manager.AccentColor);
        SetBackgroundColor(input, _manager.DefaultColor);
    }

    public void SetColor(TMP_InputField input, Color targetColor) => input.textComponent.DOColor(targetColor, _textTransitionDuration);

    public void SetBackgroundColor(TMP_InputField input, Color targetColor) => input.image.DOColor(targetColor, _textTransitionDuration);
}