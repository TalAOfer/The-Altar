using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Decision
{
    protected readonly Compare _compare;
    protected readonly CompareTo _compareTo;

    public Decision(Compare compare, CompareTo compareTo)
    {
        _compare = compare;
        _compareTo = compareTo;
    }

    public abstract bool Decide(Card baseCard, Card comparisonCard = null, int amount = 0);
}

[Flags]
public enum DecisionType
{
    PointComparison = 1,
    Parity = 2,
    Color = 4,
    Affinity = 8,
}

public enum Compare
{
    Card, Value
}

public enum CompareTo
{
    AnotherCard, Value
}