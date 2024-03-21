using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageEffect : Effect
{
    public TakeDamageEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
    }

    protected override IEnumerator ApplyEffect(Card target)
    {
        target.TakeDamage(GetAmount(target), ParentCard);
        yield return null;
    }

    protected override string GetEffectIndicationString(Card target, int amount)
    {
        return "";
    }
}
