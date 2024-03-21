using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEffectTrigger", menuName = "Effects/Effect Trigger")]
public class EffectTrigger : ScriptableObject
{
    public TriggerType TriggerType;
    public TriggerArchetype TriggerArchetype;
    public bool IsRigid;
    //[ShowIf("IsRigid")]
    public string TriggerName;
    public bool IsModifiable;
    //[HideIf("IsRigid")]
    public string TriggerText;
    //[HideIf("IsRigid")]
    public string TriggerVerb;

    //[HideIf("IsRigid")]
    [ShowInInspector]
    public string TriggerBaseText => GetTriggerBaseText();

    public string GetTriggerBaseText()
    {
        string triggerBasetext = "";
        switch (TriggerArchetype)
        {
            case TriggerArchetype.LocalAmountEvents:
                triggerBasetext = "Whenever this card " + TriggerVerb + " " + TriggerText;
                break;
            case TriggerArchetype.GlobalAmountEvents:
                triggerBasetext = "Whenever a {amountCardFilter} card " + TriggerVerb + " {amountFilter} " + TriggerText;
                break;
            case TriggerArchetype.LocalNormalEvents:
                triggerBasetext = "Whenever this card " + TriggerVerb + " " + TriggerText;
                break;
            case TriggerArchetype.GlobalNormalEvents:
                triggerBasetext = "Whenever a {normalCardFilter} card " + TriggerVerb + " " + TriggerText;
                break;
        }
        return triggerBasetext;
    }
}

public enum TriggerType
{
    Rally = 0,
    LastBreath = 1,
    Bloodthirst = 2,
    Retaliate = 3,
    Siphon = 4,
    OnChange = 5,

    SelfDamage = 6,
    SelfHeal = 7,
    
    GlobalDamage = 8,
    GlobalDeath = 9,
    GlobalHeal = 11,

    GlobalSummon = 10,
}

public enum TriggerArchetype
{
    LocalNormalEvents = 0,
    GlobalNormalEvents = 1,
    LocalAmountEvents = 2,
    GlobalAmountEvents = 3,
}

