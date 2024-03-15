using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEffectEffect : Effect
{
    public EffectBlueprint _effectBlueprint;
    public EffectTrigger _whenToTriggerAddedEffect;

    public AddEffectEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard, EffectBlueprint effectBlueprint, EffectTrigger whenToTriggerAddedEffect) : base(blueprint, data, trigger, parentCard)
    {
        _effectBlueprint = effectBlueprint;
        _whenToTriggerAddedEffect = whenToTriggerAddedEffect;
    }

    public override IEnumerator ApplyEffect(Card target)
    {
        int amount = GetAmount(target);

        RaiseEffectAppliedEvent(target, amount);
        

        _effectBlueprint.InstantiateEffect(_whenToTriggerAddedEffect, target, _data);
        yield break;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Effect is now active";
    }
}
