using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantPointsEffect : Effect
{
    public GrantPointsEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
    }

    protected override IEnumerator ApplyEffect(Card target)
    {
        int amount = GetAmount(target);
        RaiseEffectAppliedEvent(target, amount);
        target.GainPoints(amount);
        yield break;
    }

    protected override string GetEffectIndicationString(Card target, int amount)
    {
        return "Gained +" + amount.ToString() + " points";
    }
}
    
