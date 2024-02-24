using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName ="Card Visual Data")]
public class CardData : ScriptableObject
{
    [FoldoutGroup("Visuals")]
    [Title("Card Change Shader")]
    public float overallFadeDuration = 2f;
    [FoldoutGroup("Visuals")]
    public AnimationCurve overallFadeCurve;
    [FoldoutGroup("Visuals")]
    public float spritesFadeDuration = 1.25f;
    [FoldoutGroup("Visuals")]
    public AnimationCurve spritesFadeCurve;
    [FoldoutGroup("Visuals")]
    public Vector3 playerDamageVisualizerPosition;
    [FoldoutGroup("Visuals")]
    public Vector3 enemyDamageVisualizerPosition;

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
    public AnimationCurve readyingCurve;
    [FoldoutGroup("Movement")]
    [Title("Headbutt")]
    public float headbuttSpeed = 0.5f;
    [FoldoutGroup("Movement")]
    public AnimationCurve headbuttCurve;
    [FoldoutGroup("Movement")]
    [Title("Impact")]
    public float impactFreezeDuration = 0.15f;
    [FoldoutGroup("Movement")]
    [Title("BackOff")]
    public float backOffSpeed = 0.5f;
    [FoldoutGroup("Movement")]
    public AnimationCurve backoffCurve;
    [FoldoutGroup("Movement")]
    [Title("Dehighlight")]
    public float DehiglightSpeed = 1f;
    [FoldoutGroup("Movement")]
    public AnimationCurve DehighlightCurve;
    [FoldoutGroup("Movement")]
    [Title("Hand")]
    public float ReorderSpeed = 1f;
    [FoldoutGroup("Movement")]
    public AnimationCurve ReorderCurve;
    [Title("Hand")]
    public float BattleDrawbackSpeed = 5f;
    [FoldoutGroup("Movement")]
    public AnimationCurve BattleDrawbackCurve;
    [Title("Draw")]
    public float drawCardSpeed = 10;
    public AnimationCurve drawCardCurve;
}
