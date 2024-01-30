using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CustomAnimator : MonoBehaviour
{
    private enum ColorLerpStrategy
    {
        None,
        SpriteRenderer,
        Image,
        Text
    }

    [SerializeField] private Transform target;
    [SerializeField] private AnimationSO animationSO;
    [SerializeField] private bool StartOnEnable;
    [SerializeField] private bool lerpColor;
    [ShowIf("lerpColor")]
    [SerializeField] private ColorLerpStrategy colorTarget;
    [ShowIf("colorTarget", ColorLerpStrategy.SpriteRenderer)]
    [SerializeField] private SpriteRenderer sr;
    [ShowIf("colorTarget", ColorLerpStrategy.Image)]
    [SerializeField] private Image image;
    [ShowIf("colorTarget", ColorLerpStrategy.Text)]
    [SerializeField] private TextMeshPro text;

    private Vector3 defaultScale;
    private Vector3 defaultPosition;

    private void Awake()
    {
        defaultScale = target.localScale;
        defaultPosition = target.localPosition;
    }

    private void OnEnable()
    {
        if (StartOnEnable) StartAnimation();
    }

    [Button("Test Animation")]
    public void StartAnimation()
    {
        StopAllCoroutines();
        ResetScaleToDefault();
        ResetPositionToDefault();

        foreach (AnimationConfig config in animationSO.configs)
        {
            switch (config.type)
            {
                case AnimationType.Position:
                    if (config.executionStyle is AnimationStyle.Instant) SetPosition(config);
                    else StartCoroutine(LerpToPosition(config));
                    break;
                case AnimationType.Color:
                    if (config.executionStyle is AnimationStyle.Instant) SetColor(config);
                    else AnimateColor(config);
                    break;
                case AnimationType.Size:
                    if (config.executionStyle is AnimationStyle.Instant) SetScale(config);
                    else StartCoroutine(LerpScale(config));
                    break;
            }
        }
    }

    private void AnimateColor(AnimationConfig config)
    {
        switch (colorTarget)
        {
            case ColorLerpStrategy.None:
                break;
            case ColorLerpStrategy.SpriteRenderer:
                StartCoroutine(LerpColorCoroutine(new SpriteRendererWrapper(sr), config));
                break;
            case ColorLerpStrategy.Image:
                StartCoroutine(LerpColorCoroutine(new ImageWrapper(image), config));
                break;
            case ColorLerpStrategy.Text:
                StartCoroutine(LerpColorCoroutine(new TextMeshProWrapper(text), config));
                break;
        }
    }

    private void SetColor(AnimationConfig config)
    {
        switch (colorTarget)
        {
            case ColorLerpStrategy.None:
                break;
            case ColorLerpStrategy.SpriteRenderer:
                sr.color = config.endColor;
                break;
            case ColorLerpStrategy.Image:
                image.color = config.endColor;
                break;
            case ColorLerpStrategy.Text:
                text.color = config.endColor;
                break;
        }
    }

    private void ResetScaleToDefault()
    {
        target.localScale = defaultScale;
    }

    private void ResetPositionToDefault()
    {
        target.localPosition = defaultPosition;
    }

    private void SetScale(AnimationConfig config)
    {
        Vector3 newScale = new Vector3(
            config.scaleWidth ? defaultScale.x * config.scaleFactorWidth : defaultScale.x,
            config.scaleHeight ? defaultScale.y * config.scaleFactorHeight : defaultScale.y,
            defaultScale.z);
        target.localScale = newScale;
    }

    private IEnumerator LerpScale(AnimationConfig config)
    {
        if (config == null) yield break;

        float time = 0;
        Vector3 originalScale = target.localScale;
        Vector3 targetScale = new Vector3(
            config.scaleWidth ? originalScale.x * config.scaleFactorWidth : originalScale.x,
            config.scaleHeight ? originalScale.y * config.scaleFactorHeight : originalScale.y,
            originalScale.z);

        while (time < config.duration)
        {
            time += Time.deltaTime;
            target.localScale = Vector3.Lerp(originalScale, targetScale, config.curve.Evaluate(time / config.duration));
            yield return null;
        }

        // Optionally, reset to original scale or keep the target scale here
    }

    private void SetPosition(AnimationConfig config)
    {
        Vector3 targetPosition = defaultPosition + (Vector3)config.targetPositionOffset;
        target.localPosition = targetPosition;
    }

    private IEnumerator LerpToPosition(AnimationConfig config)
    {
        if (config == null) yield break;

        Vector3 originalPosition = target.localPosition;
        Vector3 targetPosition = originalPosition + (Vector3)config.targetPositionOffset;
        float time = 0;

        while (time < config.duration)
        {
            time += Time.deltaTime;
            target.localPosition = Vector3.Lerp(originalPosition, targetPosition, config.curve.Evaluate(time / config.duration));
            yield return null;
        }
    }

    private IEnumerator LerpColorCoroutine(IColorable colorable, AnimationConfig config)
    {
        if (colorable != null)
        {
            float elapsed = 0;
            Color startColor = colorable.color;
            Color endColor = config.endColor;

            while (elapsed < config.duration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / config.duration;
                // Apply the animation curve to normalized time
                float curveValue = config.curve.Evaluate(normalizedTime);
                // Interpolate color based on the curve value
                colorable.color = Color.Lerp(startColor, endColor, curveValue);
                yield return null;
            }

            colorable.color = startColor; // Set back to original color
        }
    }
}