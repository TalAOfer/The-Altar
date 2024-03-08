using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugEffect : Effect
{
    public DebugEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard) : base(blueprint, data, trigger, parentCard)
    {
    }

    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        RaiseEffectAppliedEvent(target, amount);
        yield break;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Happened!";
    }
}
