using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainPointsEffect : Effect
{
    public GainPointsEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard) : base(blueprint, data, trigger, parentCard)
    {
    }

    public override IEnumerator ApplyEffect(Card target)
    {
        int amount = GetAmount(target);
        RaiseEffectAppliedEvent(target, amount);
        target.GainPoints(amount);
        yield break;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Gained +" + amount.ToString() + " points";
    }
}
    
