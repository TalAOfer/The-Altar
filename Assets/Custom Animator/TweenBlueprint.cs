using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TweenType
{
    None,
    PunchScale,
    ShakeScale,
    PunchPosition,
    ShakePosition
}

[CreateAssetMenu(menuName ="Tweener/Tween Blueprint")]
public class TweenBlueprint : ScriptableObject
{
    public TweenType type;

    public Ease ease;

    public float Duration = 0.25f;
    public float Strength = 0.1f;
    public int Vibrato = 10;
    [ShowIf("@ShowShakeVariables()")]
    public float Randomness = 25f;
    [ShowIf("@ShowPunchVariables()")]
    public float Elasticity = 1f;
    [ShowIf("@ShowPunchVariables()")]
    public Vector3 Punch = Vector3.up;

    private bool ShowPunchVariables()
    {
        return type is TweenType.PunchScale or TweenType.PunchPosition;
    }

    private bool ShowShakeVariables()
    {
        return type is TweenType.ShakeScale or TweenType.ShakePosition;
    }
}
