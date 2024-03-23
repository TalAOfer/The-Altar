using System;
using System.Collections.Generic;

[Serializable]
public class LocalAmountTriggerFilter : IEventTriggerFilter
{
    private readonly List<Decision> _inflictorFilters;
    private readonly AmountComparisonDecision _amountFilter;
    public LocalAmountTriggerFilter(LocalAmountEventFilterBlueprint blueprint)
    {
        if (blueprint.FilterByInflictor)
        {
            if (blueprint.InflictorFilter.DecisionType.HasFlag(DecisionType.Affinity))
            {
                AffinityDecision affinityFilter = new(Compare.Card, CompareTo.AnotherCard, blueprint.InflictorFilter.AffinityComparison);
                _inflictorFilters.Add(affinityFilter);
            }

            if (blueprint.InflictorFilter.DecisionType.HasFlag(DecisionType.Color))
            {
                ColorDecision colorFilter = new(Compare.Card, CompareTo.Value, blueprint.InflictorFilter.PredefinedColor);
                _inflictorFilters.Add(colorFilter);
            }

            if (blueprint.InflictorFilter.DecisionType.HasFlag(DecisionType.Parity))
            {
                ParityDecision parityDecision = new(Compare.Card, CompareTo.Value, blueprint.InflictorFilter.PredefinedParity);
                _inflictorFilters.Add(parityDecision);
            }
        }

        if (blueprint.FilterByAmount)
        {
            _amountFilter = new AmountComparisonDecision(Compare.Value, CompareTo.Value, blueprint.AmountFilter.Comparison, blueprint.AmountFilter.PredefinedAmount);
        }
    }

    public bool Decide(Card triggerHolder, IEventData EventData)
    {
        AmountEventData DamageEventData = (AmountEventData)EventData;

        foreach (Decision decision in _inflictorFilters)
        {
            if (decision.Decide(DamageEventData.Inflictor, triggerHolder, DamageEventData.Amount) == false)
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