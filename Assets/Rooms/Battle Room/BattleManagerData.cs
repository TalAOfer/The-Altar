using Sirenix.OdinInspector;
using UnityEngine;

//[CreateAssetMenu(menuName = "Data/Battle/Card Movement")]
public class BattleManagerData : ScriptableObject
{
    public float endBattleDelay = 1f;

    [Title("Formation")]
    public float toFormationDuration = 0.5f;
    public float battleCardScale = 1.25f;

    [Title("Readying")]
    public float readyingDuration = 0.5f;
    public float readyingDistance = 0.5f;

    [Title("Headbutt")]
    public float headbuttDuration = 0.5f;
    public float headButtDistance = 0.5f;

    [Title("BackOff")]
    public float backOffDuration = 0.5f;
}
