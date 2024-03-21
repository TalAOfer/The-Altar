using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEffectEffect : Effect
{
    public EffectBlueprint _effectBlueprint;

    public AddEffectEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
        _effectBlueprint = blueprint.effectBlueprint.Value;
    }

    protected override IEnumerator ApplyEffect(Card target)
    {
        int amount = GetAmount(target);

        RaiseEffectAppliedEvent(target, amount);
        

        _effectBlueprint.InstantiateEffect(target, _data);
        yield break;
    }

    protected override string GetEffectIndicationString(Card target, int amount)
    {
        return "Effect is now active";
    }
}
