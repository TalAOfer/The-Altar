using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColorEffect : Effect
{
    private readonly CardColor _color;

    public SetColorEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
        _color = blueprint.color;
    }

    protected override IEnumerator ApplyEffect(Card target)
    {
        target.cardColor = _color;
        RaiseEffectAppliedEvent(target, 0);
        yield break;
    }

    protected override string GetEffectIndicationString(Card target, int amount)
    {
        return "Change to " + _color.ToString();
    }
}
