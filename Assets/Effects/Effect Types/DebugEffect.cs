using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugEffect : Effect
{
    public DebugEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
    }

    protected override IEnumerator ApplyEffect(Card target)
    {
        RaiseEffectAppliedEvent(target, 0);
        yield break;
    }

    protected override string GetEffectIndicationString(Card target, int amount)
    {
        return "Happened!";
    }
}
