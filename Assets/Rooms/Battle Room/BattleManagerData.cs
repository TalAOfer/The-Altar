using Sirenix.OdinInspector;
using UnityEngine;

//[CreateAssetMenu(menuName = "Data/Battle/Card Movement")]
public class BattleManagerData : ScriptableObject
{
    public float endBattleDelay = 1f;

    [Title("Readying")]
    public float readyingSpeed = 0.5f;
    public float readyingDistance = 0.5f;
    public AnimationCurve readyingCurve;

    [Title("Headbutt")]
    public float headbuttSpeed = 0.5f;
    public AnimationCurve headbuttCurve;

    [Title("Impact")]
    public float impactFreezeDuration = 0.15f;

    [Title("BackOff")]
    public float backOffSpeed = 0.5f;
    public AnimationCurve backoffCurve;
}
