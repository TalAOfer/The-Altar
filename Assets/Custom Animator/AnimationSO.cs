using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName ="Animation Config")]
public class AnimationSO : ScriptableObject
{
    public List<CustomAnimation> anims = new();
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

    // Position
    [ShowIf("type", AnimationType.Position)]
    public PositionLerpStrategy positionStrategy;

    [ShowIf("@ShouldShowPosOffset()")]
    public Vector2 targetPositionOffset;

    [ShowIf("@ShouldShowStartPos()")]
    public Vector2 startPos;

    [ShowIf("@ShouldShowEndPos()")]
    public Vector2 endPos;

    //Scale
    [ShowIf("type", AnimationType.Size)]
    public bool scaleWidth = true;
    [ShowIf("type", AnimationType.Size)]
    public float scaleFactorWidth;

    [ShowIf("type", AnimationType.Size)]
    public bool scaleHeight = true;
    [ShowIf("type", AnimationType.Size)]
    public float scaleFactorHeight;

    //Color
    [ShowIf("type", AnimationType.Color)]
    public Color firstColor;

    [ShowIf("type", AnimationType.Color)]
    public Color secondColor;

    [ShowIf("type", AnimationType.Color)]
    public bool snapToFirst;

    private bool ShouldShowPosOffset()
    {
        return positionStrategy is PositionLerpStrategy.Offset && type is AnimationType.Position;
    }

    private bool ShouldShowStartPos()
    {
        return positionStrategy is PositionLerpStrategy.Vector2ToVector2 && type is AnimationType.Position;
    }

    private bool ShouldShowEndPos()
    {
        return positionStrategy is PositionLerpStrategy.CurrentPosToVector2 && type is AnimationType.Position;
    }
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

public enum PositionLerpStrategy
{
    Offset,
    CurrentPosToVector2,
    Vector2ToVector2
}
