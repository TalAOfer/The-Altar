using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Decisions/Point Comparison Decision")]
public class PointComparisonDecision : Decision
{
    public enum Comparison
    {
        OtherBiggerThan, OtherEquals, OtherSmallerThan
    }

    public enum CompareTo
    {
        Me, Amount
    }

    public Comparison comparison;
    public CompareTo compareTo;

    [ShowIf("compareTo", CompareTo.Amount)]
    public int amount;

    public override bool Decide(EffectContext context)
    {
        bool isTrue = false;

        switch (comparison)
        {
            case Comparison.OtherBiggerThan:
                if (compareTo == CompareTo.Me) isTrue = context.OtherCard.points > context.InitiatingCard.points;
                else isTrue = context.OtherCard.points > amount;
                break;
            case Comparison.OtherEquals:
                if (compareTo == CompareTo.Me) isTrue = context.OtherCard.points == context.InitiatingCard.points;
                else isTrue = context.OtherCard.points == amount;
                break;
            case Comparison.OtherSmallerThan:
                if (compareTo == CompareTo.Me) isTrue = context.OtherCard.points < context.InitiatingCard.points;
                else isTrue = context.OtherCard.points < amount;
                break;
        }

        return isTrue;
    }
}


