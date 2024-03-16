using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ParityDecision : Decision
{
    private readonly Parity _parity;

    public ParityDecision(Compare compare, CompareTo compareTo, Parity parity) : base(compare, compareTo)
    {
        _parity = parity;
    }

    public override bool Decide(Card baseCard, Card comparisonCard = null, int amount = 0)
    {
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

        Parity ValueParity;

        if (parityCheckAmount % 2 == 0) ValueParity = Parity.Even;
        else ValueParity = Parity.Odd;

        return ValueParity == _parity;
    }
}

public enum Parity
{
    Even,
    Odd
}
