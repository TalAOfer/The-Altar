using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    // Original properties to reset before applying a new effect
    private Vector3 originalScale;
    private Vector3 originalPosition;
    [SerializeField] private bool test;
    [ShowIf("test")] [SerializeField] private TweenBlueprint testBlueprint;
    void Start()
    {
        // Store the original scale and position
        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    [ShowIf("test")]
    [Button]
    public void TestTween()
    {
        TriggerTween(testBlueprint);
    }

    public Tween TriggerTween(TweenBlueprint blueprint)
    {
        Tween tween = null;

        if (blueprint == null)
        {
            Debug.LogError("No blueprint was sent to tweener");
            return null;
        }

        switch (blueprint.type)
        {
            case TweenType.None:
                break;
            case TweenType.Jiggle:
                tween = TriggerJiggle(blueprint);
                break;
            case TweenType.Bounce:
                tween = TriggerBounce(blueprint);
                break;
            case TweenType.Shake:
                tween = TriggerShake(blueprint);
                break;
            case TweenType.Scale:
                tween = TriggerScale(blueprint);
                break;
        }

        return tween;
    }

    // Method to trigger a jiggle effect
    private Tween TriggerJiggle(TweenBlueprint blueprint)
    {
        ResetAnimations();
        return transform.DOPunchScale(new Vector3(blueprint.jiggleStrength, blueprint.jiggleStrength, blueprint.jiggleStrength), blueprint.jiggleDuration, blueprint.jiggleVibrato, blueprint.jiggleElasticity)
            .SetUpdate(true)
            .SetEase(blueprint.ease);
    }

    // Method to trigger a bounce effect
    private Tween TriggerBounce(TweenBlueprint blueprint)
    {
        ResetAnimations();
        return transform.DOPunchPosition(blueprint.bouncePunch, blueprint.bounceDuration, blueprint.bounceVibrato, blueprint.bounceElasticity, false)
            .SetUpdate(true)
            .SetEase(blueprint.ease);
    }

    // Method to trigger a shake effect
    private Tween TriggerShake(TweenBlueprint blueprint)
    {
        ResetAnimations();
        return transform.DOShakePosition(blueprint.shakeDuration, blueprint.shakeStrength, blueprint.shakeVibrato, blueprint.shakeRandomness, false, true)
            .SetUpdate(true)
            .SetEase(blueprint.ease);
    }

    private Tween TriggerScale(TweenBlueprint blueprint)
    {
        ResetAnimations();
        return transform.DOScale(blueprint.scaleAmount, blueprint.scaleDuration)
            .SetUpdate(true)
            .SetEase(blueprint.ease);
    }

    // Reset animations and restore original properties
    [ShowIf("test")]
    [Button("Reset To Original")]
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
