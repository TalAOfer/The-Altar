using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TweenType
{
    None,
    Jiggle,
    Bounce,
    Shake,
    Scale
}

[CreateAssetMenu(menuName ="Tweener/Tween Blueprint")]
public class TweenBlueprint : ScriptableObject
{
    public TweenType type;

    public Ease ease;

    [ShowIf("type", TweenType.Jiggle)]
    public float jiggleDuration = 0.5f;
    [ShowIf("type", TweenType.Jiggle)]
    public float jiggleStrength = 0.1f;
    [ShowIf("type", TweenType.Jiggle)]
    public int jiggleVibrato = 10;
    [ShowIf("type", TweenType.Jiggle)]
    public float jiggleElasticity = 1f;

    [ShowIf("type", TweenType.Bounce)]
    public float bounceDuration = 0.5f;
    [ShowIf("type", TweenType.Bounce)]
    public Vector3 bouncePunch = new(0, 0.2f, 0);
    [ShowIf("type", TweenType.Bounce)]
    public int bounceVibrato = 10;
    [ShowIf("type", TweenType.Bounce)]
    public float bounceElasticity = 1f;

    [ShowIf("type", TweenType.Shake)]
    public float shakeDuration = 0.5f;
    [ShowIf("type", TweenType.Shake)]
    public float shakeStrength = 0.1f;
    [ShowIf("type", TweenType.Shake)]
    public int shakeVibrato = 10;
    [ShowIf("type", TweenType.Shake)]
    public float shakeRandomness = 90f;

    [ShowIf("type", TweenType.Scale)]
    public float scaleDuration = 0.5f;
    [ShowIf("type", TweenType.Scale)]
    public float scaleAmount = 2f;
}
