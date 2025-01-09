using TMPro;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Animations/Input Animations", fileName = "Input Animations")]
public class InputAnimations : ScriptableObject
{
    [Header("InputText")]
    [SerializeField] float _textTransitionDuration = .2f;

    public void SetColor(TMP_InputField input, Color targetColor) => input.textComponent.DOColor(targetColor, _textTransitionDuration);

    public void SetBackgroundColor(TMP_InputField input, Color targetColor) => input.image.DOColor(targetColor, _textTransitionDuration);
}