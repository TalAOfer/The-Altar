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

    public override IEnumerator ApplyEffect(Card target)
    {
        if (triggerType is EffectTrigger.Meditate)
        {
            data.events.OnEffectApplied.Raise(this, new EffectIndication("Effect is now active", target));
        }

        effectBlueprint.SpawnEffect(whenToTriggerAddedEffect, target);
        yield return null;
    }
}
