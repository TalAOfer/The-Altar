using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName ="Card Visual Data")]
public class CardVisualData : ScriptableObject
{
    public float overallFadeDuration = 2f;
    public float spritesFadeDuration = 1.25f;
    public AnimationCurve overallFadeCurve;
    public AnimationCurve spritesFadeCurve;
    public Vector3 playerDamageVisualizerPosition;
    public Vector3 enemyDamageVisualizerPosition;
}
