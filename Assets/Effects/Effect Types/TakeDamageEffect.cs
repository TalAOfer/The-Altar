using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageEffect : Effect
{
    public TakeDamageEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard) : base(blueprint, data, trigger, parentCard)
    {
    }

    public override IEnumerator ApplyEffect(Card target)
    {
        target.TakeDamage(GetAmount(target));
        yield return null;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "";
    }
}
