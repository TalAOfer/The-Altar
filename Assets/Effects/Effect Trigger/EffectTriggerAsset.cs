using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEffectTrigger", menuName = "Effects/Effect Trigger")]
public class EffectTriggerAsset : ScriptableObject
{
    public TriggerType TriggerType;
    public TriggerArchetype TriggerArchetype;
    public bool IsRigid;
    public bool IsOnePerTurn;
    [ShowIf("IsRigid")]
    public string TriggerName;
    public bool IsModifiable;
    [HideIf("IsRigid")]
    public string TriggerText;
    [HideIf("IsRigid")]
    public string TriggerVerb;

    [HideIf("IsRigid")]
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
    Rally,
    LastBreath,
    Bloodthirst,
    Retaliate,
    Siphon,
    OnChange,

    SelfDamage,
    SelfNonLethalDamage,
    SelfLethalDamage,
    GlobalDamage,
    GlobalLethalDamage,
    GlobalNonLethalDamage,

    SelfHeal,
    GlobalHeal,
    
    GlobalDeath,
    
    GlobalSummon,

}

public enum TriggerArchetype
{
    LocalNormalEvents = 0,
    GlobalNormalEvents = 1,
    LocalAmountEvents = 2,
    GlobalAmountEvents = 3,
}

