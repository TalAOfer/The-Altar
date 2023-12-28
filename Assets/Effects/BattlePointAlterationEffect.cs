using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePointAlterationEffect : Effect
{
    public float mult;
    public PointAlterationType alterationType;
    public void Initialize(float mult, PointAlterationType alterationType)
    {
        this.mult = mult;
        this.alterationType = alterationType;
    }
    public override IEnumerator Apply(EffectContext context)
    {
        int pointsToAlter = alterationType == PointAlterationType.hurtPoints
            ? context.InitiatingCard.hurtPoints :
            context.InitiatingCard.attackPoints;

        // Multiply int with float
        float calcPoints = pointsToAlter * mult;

        // Round up
        int roundedCalcPoints = Mathf.CeilToInt(calcPoints);

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
