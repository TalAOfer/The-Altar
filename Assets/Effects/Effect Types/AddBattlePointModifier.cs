using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBattlePointModifier : Effect
{
    private readonly ModifierType _modifierType;
    private readonly BattlePointType _battlePointType;
    private readonly BattleModifierFilter _modifierFilter;
    private BattleAmountModifier _modifierInstance;

    public AddBattlePointModifier(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard) : base(blueprint, data, parentCard)
    {
        _modifierType = blueprint.modifierType;
        _battlePointType = blueprint.battlePointType;

        if (blueprint.filterBattleModifier)
        {
            _modifierFilter = new(blueprint.battleModifierfilter);
        }
    }

    protected override IEnumerator ApplyEffect(Card targetCard)
    {
        List<BattleAmountModifier> modifierList = _battlePointType is BattlePointType.Attack ?
        targetCard.attackPointsModifiers :
        targetCard.hurtPointsModifiers;
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

    protected override string GetEffectIndicationString(Card target, int amount)
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
