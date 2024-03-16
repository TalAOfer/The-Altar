using System;
using System.Collections.Generic;

[Serializable]
public class DamageTriggerFilter
{
    private readonly List<Decision> _recieverFilters;
    private readonly AmountComparisonDecision _amountFilter;
    public DamageTriggerFilter(DamageTriggerFilterBlueprint blueprint)
    {
        if (blueprint.FilterByReceiver)
        {
            if (blueprint.RecieverFilter.DecisionType.HasFlag(DecisionType.Affinity))
            {
                AffinityDecision affinityFilter = new(Compare.Object, CompareTo.AnotherObject, blueprint.RecieverFilter.AffinityComparison);
                _recieverFilters.Add(affinityFilter);
            }

            if (blueprint.RecieverFilter.DecisionType.HasFlag(DecisionType.Color))
            {
                ColorDecision colorFilter = new(Compare.Object, CompareTo.Value, blueprint.RecieverFilter.PredefinedColor);
                _recieverFilters.Add(colorFilter);
            }

            if (blueprint.RecieverFilter.DecisionType.HasFlag(DecisionType.Parity))
            {
                ParityDecision parityDecision = new(Compare.Object, CompareTo.Value, blueprint.RecieverFilter.PredefinedParity);
                _recieverFilters.Add(parityDecision);
            }
        }

        if (blueprint.FilterByAmount)
        {
            _amountFilter = new AmountComparisonDecision(Compare.Value, CompareTo.Value, blueprint.AmountFilter.Comparison, blueprint.AmountFilter.PredefinedAmount);
        }
    }

    public bool Decide(Card triggerHolder, DamageEventData DamageEventData)
    {
        foreach (Decision decision in _recieverFilters)
        {
            if (decision.Decide(DamageEventData.Reciever, triggerHolder, DamageEventData.Amount) == false)
            {
                return false;
            }
        }

        if (_amountFilter.Decide(null, null, DamageEventData.Amount) == false) 
        {
            return false;
        } 

        return true;
    }
}
