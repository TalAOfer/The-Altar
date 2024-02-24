using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterBattlePointsApplier : EffectApplier
{
    private ModifierType modifierType;
    private BattlePointType battlePointType;
    public void Initialize(ModifierType modifierType, BattlePointType battlePointType)
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

        yield return null;
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
