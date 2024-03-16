using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterBattlePointsEffect : Effect
{
    private readonly ModifierType _modifierType;
    private readonly BattlePointType _battlePointType;
    private BattleAmountModifier _modifierInstance;
    private BattleModifierFilter _modifierFilter;

    public AlterBattlePointsEffect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard, ModifierType modifierType, BattlePointType battlePointType, BattleModifierFilterBlueprint modifierFilterBlueprint) : base(blueprint, data, trigger, parentCard)
    {
        _modifierType = modifierType;
        _battlePointType = battlePointType;
        _modifierFilter = new(modifierFilterBlueprint);
    }

    public override IEnumerator ApplyEffect(Card targetCard)
    {
        List<BattleAmountModifier> modifierList = _battlePointType is BattlePointType.Attack ?
        targetCard.attackPointsModifiers :
        targetCard.hurtPointsModifiers;
        if (!_isConditional) _modifierFilter = null;
        _modifierInstance = new BattleAmountModifier(_modifierType, ParentCard, _battlePointType, _defaultAmount, _amountStrategy, _data, _modifierFilter);
        modifierList.Add(_modifierInstance);
        yield break;
    }

    public override void OnRemoveEffect(Card targetCard)
    {
        List<BattleAmountModifier> modifierList = _battlePointType is BattlePointType.Attack ?
        targetCard.attackPointsModifiers :
        targetCard.hurtPointsModifiers;

        modifierList.Remove(_modifierInstance);
    }

    public override string GetEffectIndicationString(Card target, int amount)
    {
        string sign = GetMathSign();
        return "Deals " + sign + amount.ToString();
    }

    private string GetMathSign()
    {
        string sign = "";

        switch (_modifierType)
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
