using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class UICustomAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform target;
    [SerializeField] private AnimationSO animationSO;
    [SerializeField] private bool StartOnEnable;

    private float defaultRectSizeX;
    private float defaultRectSizeY;
    private Vector3 defaultRectPosition;

    private void Awake()
    {
        defaultRectSizeX = target.sizeDelta.x;
        defaultRectSizeY = target.sizeDelta.y;
        defaultRectPosition = target.anchoredPosition;
    }

    //private void OnEnable()
    //{
    //    if (StartOnEnable) StartAnimation();
    //}

    [Button]
    public void StartAnimation()
    {
        StopAllCoroutines();
        ResetSizeToDefault();
        ResetPositionToDefault();

        foreach (AnimationConfig config in animationSO.configs)
        {
            switch (config.type)
            {
                case AnimationType.Size:
                    if (config.executionStyle == AnimationStyle.Instant) SetSize(config);
                    else StartCoroutine(LerpSize(config));
                    break;
                case AnimationType.Position:
                    if (config.executionStyle is AnimationStyle.Instant) SetPosition(config);
                    else StartCoroutine(LerpToPosition(config));
                    break;
            }
        }
    }

    private void ResetSizeToDefault()
    {
        target.sizeDelta = new Vector2(defaultRectSizeX, defaultRectSizeY);
    }

    private void ResetPositionToDefault()
    {
        target.anchoredPosition = defaultRectPosition;
    }

    private void SetSize(AnimationConfig config)
    {
        float newWidth = defaultRectSizeX * config.scaleFactorWidth;
        float newHeight = defaultRectSizeY * config.scaleFactorHeight;
        target.sizeDelta = new Vector2(newWidth, newHeight);
    }

    private IEnumerator LerpSize(AnimationConfig config)
    {
        if (config == null) yield break;

        float time = 0;
        Vector2 originalSize = target.sizeDelta;
        Vector2 targetSize = new Vector2(
            defaultRectSizeX * config.scaleFactorWidth,
            defaultRectSizeY * config.scaleFactorHeight);

        while (time < config.duration)
        {
            time += Time.deltaTime;
            target.sizeDelta = Vector2.Lerp(originalSize, targetSize, config.curve.Evaluate(time / config.duration));
            yield return null;
        }

        target.sizeDelta = originalSize; // Reset to the original size
    }

    private void SetPosition(AnimationConfig config)
    {
        Vector2 originalPosition = target.anchoredPosition;
        Vector2 targetPosition = originalPosition + config.targetPositionOffset;
        target.anchoredPosition = targetPosition;
    }

    private IEnumerator LerpToPosition(AnimationConfig config)
    {
        if (config == null) yield break;

        Vector2 originalPosition = target.anchoredPosition;
        Vector2 targetPosition = originalPosition + config.targetPositionOffset;
        float time = 0;

        while (time < config.duration)
        {
            time += Time.deltaTime;
            target.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, config.curve.Evaluate(time / config.duration));
            yield return null;
        }
    }
}
