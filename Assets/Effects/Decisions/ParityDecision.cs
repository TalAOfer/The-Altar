using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ParityDecision : Decision
{
    private readonly Parity _wantedParity;

    public ParityDecision(Compare compare, CompareTo compareTo, Parity parity) : base(compare, compareTo)
    {
        _wantedParity = parity;
    }

    public override bool Decide(Card baseCard, Card comparisonCard = null, int amount = 0)
    {
        int parityCheckAmount = 0;

        switch (_compare)
        {
            case Compare.Card:
                parityCheckAmount = baseCard.points;
                break;
            case Compare.Value:
                parityCheckAmount = amount;
                break;
        }

        Parity ValueParity;

        if (parityCheckAmount % 2 == 0) ValueParity = Parity.Even;
        else ValueParity = Parity.Odd;

        return ValueParity == _wantedParity;
    }
}

public enum Parity
{
    Even,
    Odd
}
