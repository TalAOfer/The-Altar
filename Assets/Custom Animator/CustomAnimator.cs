using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private bool StartOnEnable;
    [ShowIf("StartOnEnable")]
    [SerializeField] private string animNameOnEnable;
    [SerializeField] private bool lerpColor;
    [ShowIf("lerpColor")]
    [SerializeField] private ColorLerpStrategy colorTarget;
    [ShowIf("colorTarget", ColorLerpStrategy.SpriteRenderer)]
    [SerializeField] private SpriteRenderer sr;
    [ShowIf("colorTarget", ColorLerpStrategy.Image)]
    [SerializeField] private Image image;
    [ShowIf("colorTarget", ColorLerpStrategy.Text)]
    [SerializeField] private TextMeshPro text;

    [SerializeField] private AnimationSO animations;

    private Vector3 defaultScale;
    private Vector3 defaultPosition;

    private Coroutine currentColorCoroutine;
    private Coroutine currentPositionCoroutine;
    private Coroutine currentScaleCoroutine;

    private void Awake()
    {
        defaultScale = target.localScale;
        defaultPosition = target.localPosition;
    }

    private void OnEnable()
    {
        if (StartOnEnable) PlayAnimation(animNameOnEnable);
    }


    [Button("Test Animation")]
    public void TestAnimation(string animName)
    {
        PlayAnimation(animName);
    }

    public void PlayAnimation(string animationName)
    {
        var animation = animations.anims.Find(config => config.animName == animationName);
        if (animation != null)
        {
            foreach (AnimationConfig config in animation.configs)
            {
                switch (config.type)
                {
                    case AnimationType.Position:
                        StopAndResetCoroutine(ref currentPositionCoroutine, ResetPositionToDefault);
                        if (config.executionStyle is AnimationStyle.Instant) SetPosition(config);
                        else currentPositionCoroutine = StartCoroutine(LerpToPosition(config));
                        break;
                    case AnimationType.Color:
                        StopAndResetCoroutine(ref currentColorCoroutine, () => SetColor(config, true));
                        if (config.executionStyle is AnimationStyle.Instant) SetColor(config);
                        else currentColorCoroutine = StartCoroutine(AnimateColor(config));
                        break;
                    case AnimationType.Size:
                        StopAndResetCoroutine(ref currentScaleCoroutine, ResetScaleToDefault);
                        if (config.executionStyle is AnimationStyle.Instant) SetScale(config);
                        else currentScaleCoroutine = StartCoroutine(LerpScale(config));
                        break;
                }
            }
        }
        
        else
        {
            Debug.Log("Animation name -" + animationName + "- isn't in dictionary");
        }
    }

    private void StopAndResetCoroutine(ref Coroutine coroutine, Action resetAction)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            resetAction?.Invoke();
            coroutine = null;
        }
    }

    private IEnumerator AnimateColor(AnimationConfig config)
    {
        switch (colorTarget)
        {
            case ColorLerpStrategy.None:
                break;
            case ColorLerpStrategy.SpriteRenderer:
                yield return StartCoroutine(LerpColorCoroutine(new SpriteRendererWrapper(sr), config));
                break;
            case ColorLerpStrategy.Image:
                yield return StartCoroutine(LerpColorCoroutine(new ImageWrapper(image), config));
                break;
            case ColorLerpStrategy.Text:
                yield return StartCoroutine(LerpColorCoroutine(new TextMeshProWrapper(text), config));
                break;
        }
    }

    private void SetColor(AnimationConfig config, bool useDefault = false)
    {
        Color colorToSet = useDefault ? config.firstColor : config.secondColor;

        switch (colorTarget)
        {
            case ColorLerpStrategy.None:
                break;
            case ColorLerpStrategy.SpriteRenderer:
                sr.color = colorToSet;
                break;
            case ColorLerpStrategy.Image:
                image.color = colorToSet;
                break;
            case ColorLerpStrategy.Text:
                text.color = colorToSet;
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
            colorable.color = config.firstColor;
            Color firstColor = colorable.color;
            Color secondColor = config.secondColor;

            while (elapsed < config.duration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / config.duration;
                // Apply the animation curve to normalized time
                float curveValue = config.curve.Evaluate(normalizedTime);
                // Interpolate color based on the curve value
                colorable.color = Color.Lerp(firstColor, secondColor, curveValue);
                yield return null;
            }

            Color endColor = config.snapToFirst ? firstColor : secondColor;
            colorable.color = endColor;
        }
    }
}