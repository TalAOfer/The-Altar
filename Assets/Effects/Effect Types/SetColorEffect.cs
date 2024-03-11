using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColorEffect : Effect
{
    private readonly CardColor _color;

    public SetColorEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard, CardColor color) : base(blueprint, data, trigger, parentCard)
    {
        _color = color;
    }

    public override IEnumerator ApplyEffect(Card target)
    {
        target.cardColor = _color;
        RaiseEffectAppliedEvent(target, 0);
        yield break;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Change to " + _color.ToString();
    }
}
