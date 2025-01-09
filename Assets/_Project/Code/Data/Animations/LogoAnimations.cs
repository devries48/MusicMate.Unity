using UnityEngine;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "MusicMate/Animations/Logo Animations", fileName = "Logo Animations")]
public class LogoAnimations : ScriptableObject
{
    [Header("Fade")]
    [SerializeField] float _delayBeforeFade = 0.25f;
    [SerializeField] float _fadeDuration = 1.75f;
    [SerializeField] float _particleDuration = 5f;
    [SerializeField] float _particleStartLifetime = 5f;

    public async void PlayLogoFade(GameObject logo, Action onComplete = null)
    {
        var spriteRenderer = logo.GetComponent<Image>();
        var particleSystem = logo.GetComponentInChildren<ParticleSystem>();

        if (!spriteRenderer)
        {
            Debug.LogError("No SpriteRenderer found on the logo!");
            return;
        }

        if (!particleSystem)
        {
            Debug.LogError("No ParticleSystem found as a child of the logo!");
            return;
        }
        var particleMain = particleSystem.main;
 
        particleMain.duration = _particleDuration;
        particleMain.startLifetime = _particleStartLifetime;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(_delayBeforeFade);

        sequence.AppendCallback(() =>
        {
            if (!particleSystem.isPlaying)
                particleSystem.Play();
        });

        sequence.Append(spriteRenderer.DOFade(0, _fadeDuration));

        await sequence.AsyncWaitForCompletion();
        await WaitForParticlesToEnd(particleSystem);

        onComplete?.Invoke();
    }

    async Task WaitForParticlesToEnd(ParticleSystem particleSystem)
    {
        while (particleSystem.IsAlive(true))
            await Task.Yield();
    }
}