using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColorEffect : Effect
{
    public CardColor color;

    public SetColorEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard, CardColor color) : base(blueprint, data, trigger, parentCard)
    {
        this.color = color;
    }

    public override IEnumerator ApplyEffect(Card target, int amount)
    {
        target.cardColor = color;
        RaiseEffectAppliedEvent(target, amount);
        yield break;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Change to " + color.ToString();
    }
}
