using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEffectApplier : EffectApplier
{
    public EffectBlueprint effectBlueprint;
    public EffectTrigger whenToTriggerAddedEffect;
    public void Initialize(EffectBlueprint effectBlueprint, EffectTrigger whenToTriggerAddedEffect)
    {
        this.effectBlueprint = effectBlueprint;
        this.whenToTriggerAddedEffect = whenToTriggerAddedEffect;
    }

    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        if (triggerType is EffectTrigger.Meditate)
        {
            RaiseEffectAppliedEvent(target, amount);
        }

        effectBlueprint.SpawnEffect(whenToTriggerAddedEffect, target);
        yield return null;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Effect is now active";
    }
}
