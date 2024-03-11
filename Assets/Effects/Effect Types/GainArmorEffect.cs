using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainArmorEffect : Effect
{
    public GainArmorEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard) : base(blueprint, data, trigger, parentCard)
    {
    }

    public override IEnumerator ApplyEffect(Card target)
    {
        int amount = GetAmount(target);
        target.GainArmor(amount);
        yield return null;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        return "Got " + amount + " armor";
    }
}
