using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : Effect
{
    public DrawCardEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard) : base(blueprint, data, trigger, parentCard)
    {
    }

    public override void ApplyEffect(Card target, int amount)
    {
        RaiseEffectAppliedEvent(target, amount);
        //actions.DrawCardToHand();
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Draw " + amount.ToString() + " cards";
    }
}
