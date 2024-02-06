using DG.Tweening;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    // Original properties to reset before applying a new effect
    private Vector3 originalScale;
    private Vector3 originalPosition;
    [SerializeField] private TweenerSettings settings;
    void Start()
    {
        // Store the original scale and position
        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    // Method to trigger a jiggle effect
    public void TriggerJiggle()
    {
        ResetAnimations();
        transform.DOPunchScale(new Vector3(settings.jiggleStrength, settings.jiggleStrength, settings.jiggleStrength), settings.jiggleDuration, settings.jiggleVibrato, settings.jiggleElasticity).SetUpdate(true);
    }

    // Method to trigger a bounce effect
    public void TriggerBounce()
    {
        ResetAnimations();
        transform.DOPunchPosition(settings.bouncePunch, settings.bounceDuration, settings.bounceVibrato, settings.bounceElasticity, false).SetUpdate(true);
    }

    // Method to trigger a shake effect
    public void TriggerShake()
    {
        ResetAnimations();
        transform.DOShakePosition(settings.shakeDuration, settings.shakeStrength, settings.shakeVibrato, settings.shakeRandomness, false, true).SetUpdate(true);
    }

    // Reset animations and restore original properties
    private void ResetAnimations()
    {
        DOTween.Kill(transform); // Stop any DOTween animations on this transform
        transform.localScale = originalScale; // Reset to original scale
        transform.position = originalPosition; // Reset to original position
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }
}
