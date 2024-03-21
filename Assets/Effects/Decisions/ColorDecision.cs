using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDecision : Decision
{
    private readonly CardColor _predefinedColor;
    public ColorDecision(Compare compare, CompareTo compareTo, CardColor predefinedColor) : base(compare, compareTo)
    {
        _predefinedColor = predefinedColor;
    }

    public override bool Decide(Card baseCard, Card comparisonCard = null, int amount = 0)
    {
        if (_compare is Compare.Value)
        {
            Debug.Log("Value doesn't have a color. Change _compare to Object");
            return false;
        }

        CardColor baseColor = baseCard.cardColor;
        CardColor comparisonColor = _compareTo is CompareTo.AnotherCard ? 
            comparisonCard.cardColor : 
            _predefinedColor;

        return baseColor == comparisonColor;
    }

}
