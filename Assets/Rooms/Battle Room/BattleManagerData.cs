using Sirenix.OdinInspector;
using UnityEngine;

//[CreateAssetMenu(menuName = "Data/Battle/Card Movement")]
public class BattleManagerData : ScriptableObject
{
    public float endBattleDelay = 1f;

    [Title("Readying")]
    public float readyingDuration = 0.5f;
    public float readyingDistance = 0.5f;

    [Title("Headbutt")]
    public float headbuttDuration = 0.5f;

    [Title("Impact")]
    public float impactFreezeDuration;

    [Title("BackOff")]
    public float backOffDuration = 0.5f;
}
