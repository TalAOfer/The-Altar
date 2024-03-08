using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterBattlePointsEffect : Effect
{
    private ModifierType modifierType;
    private BattlePointType battlePointType;

    public AlterBattlePointsEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard, ModifierType modifierType, BattlePointType battlePointType) : base(blueprint, data, trigger, parentCard)
    {
        this.modifierType = modifierType;
        this.battlePointType = battlePointType;
    }

    public override IEnumerator ApplyEffect(Card targetCard, int amount)
    {
        List<BattlePointModifier> modifierList = battlePointType is BattlePointType.Attack ?
        targetCard.attackPointsModifiers :
        targetCard.hurtPointsModifiers;
        RaiseEffectAppliedEvent(targetCard, amount);
        modifierList.Add(new BattlePointModifier(modifierType, amount));
        yield break;
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        string sign = GetMathSign();
        return "Deals " + sign + amount.ToString();
    }

    private string GetMathSign()
    {
        string sign = "";

        switch (modifierType)
        {
            case ModifierType.Addition:
                sign = "+";
                break;
            case ModifierType.Subtraction:
                sign = "-";
                break;
            case ModifierType.Mult:
                sign = "X";
                break;
            case ModifierType.Division:
                sign = "/";
                break;
            case ModifierType.Replace:
                break;
        }

        return sign;
    }
}
