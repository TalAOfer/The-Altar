using Sirenix.OdinInspector;
using System;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public class Decision
{
    public enum DecisionType
    {
        PointComparison,
        Color
    }

    public enum Compare
    {
        Target,
        Opponent
    }
    public enum Comparison
    {
        BiggerThan, Equals, SmallerThan, BiggerOrEquals, SmallerOrEquals
    }

    public enum CompareTo
    {
        Opponent, Amount
    }

    public DecisionType decisionType;
    public Compare compare;
    [ShowIf("decisionType", DecisionType.PointComparison)]
    public Comparison comparison;
    [ShowIf("decisionType", DecisionType.PointComparison)]
    public CompareTo compareTo;

    [ShowIf("compareTo", CompareTo.Amount)]
    public int amount;

    [ShowIf("decisionType", DecisionType.Color)]
    public CardColor color;

    public bool Decide(Card targetCard, Card opponentCard)
    {
        bool isTrue = false;
        if (decisionType is DecisionType.PointComparison) isTrue = DecideByPointComparison(targetCard, opponentCard);
        if (decisionType is DecisionType.Color) isTrue = DecideByColor(targetCard, opponentCard);
        return isTrue;
    }

    public bool DecideByPointComparison(Card targetCard, Card opponentCard)
    {
        bool isTrue = false;
        int targetValue = targetCard.points;
        int opponentValue = 0;
        if (opponentCard != null) opponentValue = opponentCard.points;

        if (compareTo == CompareTo.Amount)
        {
            int comparedValue = (compare == Compare.Target) ? targetValue : opponentValue;

            switch (comparison)
            {
                case Comparison.BiggerThan:
                    isTrue = comparedValue > amount;
                    break;
                case Comparison.Equals:
                    isTrue = comparedValue == amount;
                    break;
                case Comparison.SmallerThan:
                    isTrue = comparedValue < amount;
                    break;
                case Comparison.BiggerOrEquals:
                    isTrue = comparedValue >= amount;
                    break;
                case Comparison.SmallerOrEquals:
                    isTrue = comparedValue <= amount;
                    break;
            }
        }
        else if (compareTo == CompareTo.Opponent)
        {
            switch (comparison)
            {
                case Comparison.BiggerThan:
                    isTrue = targetValue > opponentValue;
                    break;
                case Comparison.Equals:
                    isTrue = targetValue == opponentValue;
                    break;
                case Comparison.SmallerThan:
                    isTrue = targetValue < opponentValue;
                    break;
                case Comparison.BiggerOrEquals:
                    isTrue = targetValue >= opponentValue;
                    break;
                case Comparison.SmallerOrEquals:
                    isTrue = targetValue <= opponentValue;
                    break;
            }
        }

        return isTrue;
    }

    public bool DecideByColor(Card targetCard, Card opponentCard)
    {
        Card card = compare is Compare.Target ? targetCard : opponentCard;
        return card.cardColor == color;
    }
}
