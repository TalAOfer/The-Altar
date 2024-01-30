using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Animation Config")]
public class AnimationSO : ScriptableObject
{
    public List<AnimationConfig> configs;
}

[System.Serializable]
public class AnimationConfig
{
    public AnimationType type;
    public AnimationStyle executionStyle;

    [ShowIf("executionStyle", AnimationStyle.Lerp)]
    public float duration;

    [ShowIf("executionStyle", AnimationStyle.Lerp)]
    public AnimationCurve curve;

    [ShowIf("type", AnimationType.Position)]
    public Vector2 targetPositionOffset;

    [ShowIf("type", AnimationType.Size)]
    public bool scaleWidth = true;
    [ShowIf("type", AnimationType.Size)]
    public float scaleFactorWidth;

    [ShowIf("type", AnimationType.Size)]
    public bool scaleHeight = true;
    [ShowIf("type", AnimationType.Size)]
    public float scaleFactorHeight;

    [ShowIf("type", AnimationType.Color)]
    public Color endColor;
}


public enum AnimationType
{
    Size,
    Position,
    Color
}

public enum AnimationStyle
{
    Instant,
    Lerp
}
