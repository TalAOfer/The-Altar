using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterBattlePointsApplier : EffectApplier
{
    private float amount;
    private ModifierType modifierType;
    private BattlePointType battlePointType;
    public void Initialize(float amount, ModifierType modifierType, BattlePointType battlePointType)
    {
        this.amount = amount;
        this.modifierType = modifierType;
        this.battlePointType = battlePointType;
    }

    public override IEnumerator ApplyEffect(Card targetCard)
    {
        List<BattlePointModifier> modifierList = battlePointType is BattlePointType.Attack ?
        targetCard.attackPointsModifiers :
        targetCard.hurtPointsModifiers;

        modifierList.Add(new BattlePointModifier(modifierType, amount));

        yield return null;
    }
}
