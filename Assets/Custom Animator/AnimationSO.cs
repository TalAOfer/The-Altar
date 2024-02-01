using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName ="Animation Config")]
public class AnimationSO : ScriptableObject
{
    public List<CustomAnimation> anims = new();

    internal object Find(Func<object, bool> value)
    {
        throw new NotImplementedException();
    }
}

[System.Serializable]
public class CustomAnimation
{
    public string animName;
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
    public Color firstColor;

    [ShowIf("type", AnimationType.Color)]
    public Color secondColor;

    [ShowIf("type", AnimationType.Color)]
    public bool snapToFirst;
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
