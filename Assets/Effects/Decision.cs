using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class Decision
{
    public Compare _compare;
    private CompareTo _compareTo;

    private Comparison _comparison;
    private DecisionType decisionType;
    private int _predefinedAmount;
    private CardColor _predefinedColor;
    private Parity _parity;
    private AffinityComparison _affinityComparison;


    public Decision(Compare compare, CompareTo compareTo)
    {
        _compare = compare;
        _compareTo = compareTo;
    }

    public bool Decide(Card baseCard, Card comparisonCard = null, int amount = 0)
    {
        if (_compareTo is CompareTo.AnotherObject && comparisonCard == null) return false;

        switch (decisionType)
        {
            case DecisionType.PointComparison:
                if (_compareTo == CompareTo.AnotherObject && comparisonCard == null) return false;

                int baseAmount = (_compare == Compare.Object) ? baseCard.points : amount;
                int comparisonAmount = (_compareTo == CompareTo.AnotherObject) ? comparisonCard.points : this._predefinedAmount;

                return DecideByPointComparison(baseAmount, comparisonAmount);

            case DecisionType.Parity:
                int parityCheckAmount = 0;
                switch (_compare)
                {
                    case Compare.Object:
                        parityCheckAmount = baseCard.points;
                        break;
                    case Compare.Value:
                        parityCheckAmount = amount;
                        break;
                }
                return DecideByParity(parityCheckAmount);

            case DecisionType.Color:
                if (_compare is Compare.Value)
                {
                    Debug.Log("Value doesn't have a color. Change _compare to Object");
                    return false;
                }

                CardColor baseColor = baseCard.cardColor;
                CardColor comparisonColor = _compareTo is CompareTo.AnotherObject ? comparisonCard.cardColor : _predefinedColor;

                return DecideByColor(baseColor, comparisonColor);

            case DecisionType.Affinity:
                if (_compare is Compare.Value)
                {
                    Debug.Log("Value doesn't have an affinity. Change Compare to Object");
                    return false;
                }

                if (_compareTo is CompareTo.Value)
                {
                    Debug.Log("Affinity can only be compared with another card. Change CompareTo to Object");
                    return false;
                }

                return DecideByAffinity(baseCard.Affinity, comparisonCard.Affinity);

            default:
                return false;
        }

    }

    public bool DecideByPointComparison(int baseValue, int comparisonValue = 0)
    {
        switch (_comparison)
        {
            case Comparison.BiggerThan:
                return baseValue > comparisonValue;
            case Comparison.Equals:
                return baseValue == comparisonValue;
            case Comparison.SmallerThan:
                return baseValue < comparisonValue;
            case Comparison.BiggerOrEquals:
                return baseValue >= comparisonValue;
            case Comparison.SmallerOrEquals:
                return baseValue <= comparisonValue;

            default:
                return false;
        }
    }

    public bool DecideByColor(CardColor color, CardColor comparisonColor)
    {
        return color == comparisonColor;
    }

    private bool DecideByParity(int value)
    {
        Parity ValueParity;

        if (value % 2 == 0) ValueParity = Parity.Even;
        else ValueParity = Parity.Odd;

        return ValueParity == _parity;
    }
    private bool DecideByAffinity(Affinity affinity, Affinity comparedAffinity)
    {
        bool isFriendly = affinity == comparedAffinity;
        if (_affinityComparison is AffinityComparison.Friendly) return isFriendly;
        else return !isFriendly;
    }
}
public enum DecisionType
{
    PointComparison,
    Parity,
    Color,
    Affinity,
}

public enum AffinityComparison
{
    Friendly,
    Enemy
}

public enum Parity
{
    Even,
    Odd
}

public enum Comparison
{
    BiggerThan, Equals, SmallerThan, BiggerOrEquals, SmallerOrEquals
}

public enum Compare
{
    Object, Value
}
public enum CompareTo
{
    AnotherObject, Value
}