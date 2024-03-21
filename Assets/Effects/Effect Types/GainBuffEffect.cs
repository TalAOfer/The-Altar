using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainBuffEffect : Effect
{
    private readonly BuffType _buffType;
    public GainBuffEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
        _buffType = blueprint.BuffType;
    }

    protected override IEnumerator ApplyEffect(Card target)
    {
        int amount = GetAmount(target);
        
        switch (_buffType)
        {
            case BuffType.Might:
                target.GainMight(amount);
                break;
            case BuffType.Armor:
                target.GainArmor(amount);
                break;
        }

        yield return null;
    }

    protected override string GetEffectIndicationString(Card target, int amount)
    {
        return "Got " + amount + " " + _buffType.ToString();
    }
}

public enum BuffType
{
    Might,
    Armor
}
