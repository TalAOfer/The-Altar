using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : Effect
{
    public DrawCardEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard) : base(blueprint, data, trigger, parentCard)
    {
    }

    public override IEnumerator ApplyEffect(Card target)
    {
        int amount = GetAmount(target);
        RaiseEffectAppliedEvent(target, amount);
        _data.DrawCardsToHand(amount);
        yield break; 
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Draw " + amount.ToString() + " cards";
    }
}
