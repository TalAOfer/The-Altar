using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialDamageEffect : Effect
{
    public InitialDamageEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard) : base(blueprint, data, trigger, parentCard)
    {
    }

    public override IEnumerator Trigger()
    {
        _events.OnDamageEffect.Raise(ParentCard);
        yield break;
    }

    public override IEnumerator ApplyEffect(Card target)
    {
        yield break;
    }
}
