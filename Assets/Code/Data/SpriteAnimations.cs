using UnityEngine;
using DG.Tweening;
using System;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "MusicMate/Sprite Animation Settings", fileName = "Sprite Animations")]
public class SpriteAnimations : ScriptableObject
{

    public void PlaySpriteGlance(GameObject logo, Action onComplete = null)
    {
        onComplete?.Invoke();
    }

}