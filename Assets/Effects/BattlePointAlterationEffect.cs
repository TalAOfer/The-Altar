using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePointAlterationEffect : Effect
{
    public float amount;
    public ModifierType modifierType;
    public PointAlterationType alterationType;
    public void Initialize(float amount, ModifierType modifierType, PointAlterationType alterationType)
    {
        this.amount = amount;
        this.modifierType = modifierType;
        this.alterationType = alterationType;
    }
    public override IEnumerator Apply(EffectContext context)
    {
        int pointsToAlter = alterationType == PointAlterationType.hurtPoints
            ? context.InitiatingCard.hurtPoints :
            context.InitiatingCard.attackPoints;

        int roundedCalcPoints = 0;
        // Multiply int with float
        switch (modifierType)
        {
            case ModifierType.Addition:
                roundedCalcPoints = (int)(pointsToAlter + amount);
                break;
            case ModifierType.Mult:
                float calcPoints = pointsToAlter * amount;
                roundedCalcPoints = Mathf.CeilToInt(calcPoints);
                break;
            case ModifierType.Replace:
                roundedCalcPoints = (int)amount;
                break;
        }

        switch (alterationType)
        {
            case PointAlterationType.hurtPoints:
                context.InitiatingCard.hurtPoints = roundedCalcPoints;
                break;
            case PointAlterationType.attackPoints:
                context.InitiatingCard.attackPoints = roundedCalcPoints;
                break;
        }

        yield return null;
    }
}

public enum PointAlterationType
{
    hurtPoints,
    attackPoints
}
