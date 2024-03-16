using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffinityDecision : Decision
{
    private readonly AffinityComparison _affinityComparison;

    public AffinityDecision(Compare compare, CompareTo compareTo, AffinityComparison affinityComparison) : base(compare, compareTo)
    {
        _affinityComparison = affinityComparison;
    }

    public override bool Decide(Card baseCard, Card comparisonCard = null, int amount = 0)
    {
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

        bool isFriendly = baseCard.Affinity == comparisonCard.Affinity;
        if (_affinityComparison is AffinityComparison.Friendly) return isFriendly;
        else return !isFriendly;
    }
}
public enum AffinityComparison
{
    Friendly,
    Enemy
}