using DG.Tweening;
using Sirenix.Serialization;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    // Original properties to reset before applying a new effect
    private Vector3 originalScale;
    private Vector3 originalPosition;
    void Start()
    {
        // Store the original scale and position
        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    public void TriggerTween(TweenBlueprint blueprint)
    {
        if (blueprint == null)
        {
            Debug.LogError("No blueprint was sent to tweener");
            return;
        }

        switch (blueprint.type)
        {
            case TweenType.None:
                break;
            case TweenType.Jiggle:
                TriggerJiggle(blueprint);
                break;
            case TweenType.Bounce:
                TriggerBounce(blueprint);
                break;
            case TweenType.Shake:
                TriggerShake(blueprint);
                break;
        }
    }

    // Method to trigger a jiggle effect
    private void TriggerJiggle(TweenBlueprint blueprint)
    {
        ResetAnimations();
        transform.DOPunchScale(new Vector3(blueprint.jiggleStrength, blueprint.jiggleStrength, blueprint.jiggleStrength), blueprint.jiggleDuration, blueprint.jiggleVibrato, blueprint.jiggleElasticity).SetUpdate(true);
    }

    // Method to trigger a bounce effect
    private void TriggerBounce(TweenBlueprint blueprint)
    {
        ResetAnimations();
        transform.DOPunchPosition(blueprint.bouncePunch, blueprint.bounceDuration, blueprint.bounceVibrato, blueprint.bounceElasticity, false).SetUpdate(true);
    }

    // Method to trigger a shake effect
    private void TriggerShake(TweenBlueprint blueprint)
    {
        ResetAnimations();
        transform.DOShakePosition(blueprint.shakeDuration, blueprint.shakeStrength, blueprint.shakeVibrato, blueprint.shakeRandomness, false, true).SetUpdate(true);
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
