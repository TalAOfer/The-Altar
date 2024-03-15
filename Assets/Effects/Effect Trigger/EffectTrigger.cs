using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEffectTrigger", menuName = "Effects/Effect Trigger")]
public class EffectTrigger : ScriptableObject
{
    public bool isModifiable;
    public TriggerType TriggerType;
}

public enum TriggerType
{
    Rally,
    LastBreath,
    Bloodthirst,
    Siphon,

    Damage,
    LethalDamage,
    NonlethalDamage,
    Death,
    Change,
    Summon,
    Heal
}

[Serializable]
public class EffectTriggerModifiers
{
    [SerializeField] private bool _isConditionalByAffinity;
    [ShowIf("_isConditionalByAffinity")]
    [SerializeField] private Affinity _affinityModifier;

    [SerializeField] private bool _isConditionalByColor;
    [ShowIf("_isConditionalByColor")]
    [SerializeField] private CardColor _colorModifier;

    [SerializeField] private bool _isConditionalByPointsAmount;
    [SerializeField] private Comparison _comparison;
    [SerializeField] private int amount;

    [SerializeField] private bool _isConditionalByPointsNature;

    public bool Decide(Card triggerInitiatingCard)
    {
        if (!_isConditionalByAffinity && !_isConditionalByColor) return true;

        bool doesAffinityMatch = !_isConditionalByAffinity || triggerInitiatingCard.Affinity == _affinityModifier;

        bool doesColorMatch = !_isConditionalByColor || triggerInitiatingCard.cardColor == _colorModifier;

        return doesColorMatch && doesAffinityMatch;
    }
}
