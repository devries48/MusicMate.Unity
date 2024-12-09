using TMPro;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "MusicMate/Input Animation Settings", fileName = "Input Animations")]
public class InputAnimations : ScriptableObject
{
    [Header("InputText Animation")]
    public float TextTransitionDuration = .2f;

    public void SetColor(TMP_InputField input, Color targetColor) => input.textComponent.DOColor(targetColor, TextTransitionDuration);

    public void SetBackgroundColor(TMP_InputField input, Color targetColor) => input.image.DOColor(targetColor, TextTransitionDuration);
}