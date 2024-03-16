using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmountComparisonDecision : Decision
{
    private readonly Comparison _comparison;
    private readonly int _predefinedAmount;

    public AmountComparisonDecision(Compare compare, CompareTo compareTo, Comparison comparison, int predefinedAmount = 0) : base(compare, compareTo)
    {
        _comparison = comparison;
        _predefinedAmount = predefinedAmount;
    }

    public override bool Decide(Card baseCard, Card comparisonCard = null, int amount = 0)
    {
        int baseValue = _compare is Compare.Object ? baseCard.points : amount;
        int comparisonValue = _compareTo is CompareTo.AnotherObject ? comparisonCard.points : _predefinedAmount;

        return _comparison switch
        {
            Comparison.BiggerThan => baseValue > comparisonValue,
            Comparison.Equals => baseValue == comparisonValue,
            Comparison.SmallerThan => baseValue < comparisonValue,
            Comparison.BiggerOrEquals => baseValue >= comparisonValue,
            Comparison.SmallerOrEquals => baseValue <= comparisonValue,
            _ => false,
        };
    }
}
public enum Comparison
{
    BiggerThan, Equals, SmallerThan, BiggerOrEquals, SmallerOrEquals
}
