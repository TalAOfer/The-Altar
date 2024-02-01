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

    public override IEnumerator ApplyEffect(Card targetCard)
    {
        List<BattlePointModifier> modifierList = battlePointType is BattlePointType.Attack ?
        targetCard.attackPointsModifiers :
        targetCard.hurtPointsModifiers;
        data.events.OnEffectApplied.Raise(this, new EffectIndication("Deals +" + GetAmount().ToString(), targetCard));
        modifierList.Add(new BattlePointModifier(modifierType, GetAmount()));

        yield return null;
    }
}
