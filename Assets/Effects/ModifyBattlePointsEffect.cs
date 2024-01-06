using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyBattlePointsEffect : Effect
{
    private float amount;
    private ModifierType modifierType;
    private BattlePointType battlePointType;
    private bool isConditional;
    private Decision decision;
    private WhoToChange whoToChange;
    public void Initialize(WhoToChange whoToChange, float amount, ModifierType modifierType, BattlePointType battlePointType, bool isConditional, Decision decision)
    {
        this.whoToChange = whoToChange;
        this.amount = amount;
        this.modifierType = modifierType;
        this.battlePointType = battlePointType;
        this.isConditional = isConditional;
        this.decision = decision;
    }
    public override IEnumerator Apply(EffectContext context)
    {
        if (isConditional && !decision.Decide(context)) yield break;
        Card cardToChange = whoToChange is WhoToChange.Initiating ? 
        context.InitiatingCard : 
        context.OtherCard;

        List<BattlePointModifier> modifierList = battlePointType is BattlePointType.Attack ? 
        cardToChange.attackPointsModifiers :
        cardToChange.hurtPointsModifiers;

        modifierList.Add(new BattlePointModifier(modifierType, amount));

        yield return null;
    }
}