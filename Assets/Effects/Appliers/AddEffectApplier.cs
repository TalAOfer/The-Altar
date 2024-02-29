using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEffectApplier : EffectApplier
{
    public EffectBlueprintReference effectBlueprint;
    public EffectTrigger whenToTriggerAddedEffect;
    public void Initialize(EffectBlueprintReference effectBlueprint, EffectTrigger whenToTriggerAddedEffect)
    {
        this.effectBlueprint = effectBlueprint;
        this.whenToTriggerAddedEffect = whenToTriggerAddedEffect;
    }

    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        if (triggerType.TriggerType is TriggerType.Meditate)
        {
            RaiseEffectAppliedEvent(target, amount);
        }

        effectBlueprint.Value.InstantiateEffect(whenToTriggerAddedEffect, target);
        yield return null;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Effect is now active";
    }
}
