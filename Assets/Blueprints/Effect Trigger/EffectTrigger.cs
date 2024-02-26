using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEffectTrigger", menuName = "Effects/Effect Trigger")]
public class EffectTrigger : ScriptableObject
{
    public TriggerType TriggerType;
}

public enum TriggerType
{
    StartOfTurn,
    StartOfBattle,
    Support,
    BeforeAttacking,
    OnDeath,
    OnGlobalDeath,
    OnSurvive,
    Bloodthirst,
    Meditate,
}
