using UnityEngine;
using DG.Tweening;
using System;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "MusicMate/Logo Animation Settings", fileName = "Logo Animations")]
public class LogoAnimations : ScriptableObject
{
    [Header("Fade Animation")]
    public float delayBeforeFade = 0.25f;
    public float fadeDuration = 1.75f;
    public float particleDuration = 5f;
    public float particleStartLifetime = 5f;

    public async void PlayLogoFade(GameObject logo, Action onComplete = null)
    {
        var spriteRenderer = logo.GetComponent<SpriteRenderer>();
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
 
        particleMain.duration = particleDuration;
        particleMain.startLifetime = particleStartLifetime;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(delayBeforeFade);

        sequence.AppendCallback(() =>
        {
            if (!particleSystem.isPlaying)
                particleSystem.Play();
        });

        sequence.Append(spriteRenderer.DOFade(0, fadeDuration));

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