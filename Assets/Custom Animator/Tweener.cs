using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    // Original properties to reset before applying a new effect
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Tween activeTween;
    [SerializeField] private bool test;
    [ShowIf("test")][SerializeField] private TweenBlueprint testBlueprint;
    private void Start()
    {
        // Store the original scale and position
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
    }

    [ShowIf("test")]
    [Button]
    public void TestTween()
    {
        TriggerTween(testBlueprint);
    }

    private void SaveOriginalTransform()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
    }

    private void ResetToOriginalTransform()
    {
        transform.localScale = originalScale;
        transform.localPosition = originalPosition;
    }

    public Tween TriggerTween(TweenBlueprint blueprint)
    {
        Tween tween = null;

        if (blueprint == null)
        {
            Debug.LogError("No blueprint was sent to tweener");
            return null;
        }

        if (activeTween != null && !activeTween.IsComplete())
        {
            activeTween.Complete(); // This will jump to the end of the tween immediately
            ResetToOriginalTransform();
        }

        SaveOriginalTransform();

        switch (blueprint.type)
        {
            case TweenType.None:
                break;
            case TweenType.PunchScale:
                tween = TriggerPunchScale(blueprint);
                break;
            case TweenType.ShakeScale:
                tween = TriggerShakeScale(blueprint);
                break;
            case TweenType.PunchPosition:
                tween = TriggerPunchPosition(blueprint);
                break;
            case TweenType.ShakePosition:
                tween = TriggerShakePosition(blueprint);
                break;
        }

        activeTween = tween;

        activeTween?.OnComplete(() =>
            {
                ResetToOriginalTransform();
                activeTween = null; // Clear the active tween since it's completed
            });

        return tween;
    }

    // Method to trigger a jiggle effect
    private Tween TriggerPunchPosition(TweenBlueprint blueprint)
    {
        return transform.DOPunchPosition(blueprint.Punch, blueprint.Duration, blueprint.Vibrato, blueprint.Elasticity)
            .SetUpdate(true)
            .SetEase(blueprint.ease);
    }

    private Tween TriggerPunchScale(TweenBlueprint blueprint)
    {
        return transform.DOPunchScale(blueprint.Punch, blueprint.Duration, blueprint.Vibrato, blueprint.Elasticity)
            .SetUpdate(true)
            .SetEase(blueprint.ease);
    }

    private Tween TriggerShakePosition(TweenBlueprint blueprint)
    {
        return transform.DOShakePosition(blueprint.Duration, blueprint.Strength, blueprint.Vibrato, blueprint.Randomness)
            .SetUpdate(true)
            .SetEase(blueprint.ease);
    }

    private Tween TriggerShakeScale(TweenBlueprint blueprint)
    {
        return transform.DOShakeScale(blueprint.Duration, blueprint.Strength, blueprint.Vibrato, blueprint.Randomness)
            .SetUpdate(true)
            .SetEase(blueprint.ease);
    }
}
