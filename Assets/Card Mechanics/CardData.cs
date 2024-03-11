using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName ="Card Visual Data")]
public class CardData : ScriptableObject
{
    [FoldoutGroup("Abilities")]
    public Vector3 leftCardPosition = new(-4 , -2);
    public Vector3 rightCardPosition = new(4 , -2);

    [FoldoutGroup("Visuals")]
    [Title("Card Change Shader")]
    public float overallFadeDuration = 2f;
    [FoldoutGroup("Visuals")]
    public AnimationCurve overallFadeCurve;
    [FoldoutGroup("Visuals")]
    public float spritesFadeDuration = 1.25f;
    [FoldoutGroup("Visuals")]
    public AnimationCurve spritesFadeCurve;

    [FoldoutGroup("Movement")]
    [Title("Battle")]
    [FoldoutGroup("Movement")]
    public float endBattleDelay = 1f;
    [FoldoutGroup("Movement")]
    [Title("Readying")]
    public float readyingSpeed = 0.5f;
    [FoldoutGroup("Movement")]
    public float readyingDistance = 0.5f;
    [FoldoutGroup("Movement")]
    public Ease readyingCurve;
    [FoldoutGroup("Movement")]
    [Title("Headbutt")]
    public float headbuttSpeed = 0.5f;
    [FoldoutGroup("Movement")]
    public Ease headbuttCurve;
    [FoldoutGroup("Movement")]
    [Title("Impact")]
    public float impactFreezeDuration = 0.15f;
    [FoldoutGroup("Movement")]
    [Title("BackOff")]
    public float backOffSpeed = 0.5f;
    [FoldoutGroup("Movement")]
    public Ease backoffCurve;
    [FoldoutGroup("Movement")]
    [Title("Dehighlight")]
    public float DehiglightSpeed = 1f;
    [FoldoutGroup("Movement")]
    public Ease DehighlightCurve;
    [FoldoutGroup("Movement")]
    [Title("Hand")]
    public float ReorderSpeed = 1f;
    [FoldoutGroup("Movement")]
    public Ease ReorderCurve;
    [Title("Hand")]
    public float BattleDrawbackSpeed = 5f;
    [FoldoutGroup("Movement")]
    public Ease BattleDrawbackCurve;
    [Title("Draw")]
    public float drawCardSpeed = 10;
    public Ease drawCardCurve;
}

public class CardTween
{
    public float speed;
    public Ease ease;
} 
