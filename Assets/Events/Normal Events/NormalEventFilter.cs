using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEventFilter : IEventTriggerFilter
{
    private readonly List<Decision> _filters = new();
    public NormalEventFilter(NormalEventFilterBlueprint blueprint)
    {
        if (blueprint.ShouldFilter)
        {
            if (blueprint.Filter.DecisionType.HasFlag(DecisionType.Affinity))
            {
                AffinityDecision affinityFilter = new(Compare.Card, CompareTo.AnotherCard, blueprint.Filter.AffinityComparison);
                _filters.Add(affinityFilter);
            }

            if (blueprint.Filter.DecisionType.HasFlag(DecisionType.Color))
            {
                ColorDecision colorFilter = new(Compare.Card, CompareTo.Value, blueprint.Filter.PredefinedColor);
                _filters.Add(colorFilter);
            }

            if (blueprint.Filter.DecisionType.HasFlag(DecisionType.Parity))
            {
                ParityDecision parityDecision = new(Compare.Card, CompareTo.Value, blueprint.Filter.PredefinedParity);
                _filters.Add(parityDecision);
            }
        }
    }

    public bool Decide(Card triggerHolder, IEventData EventData)
    {
        NormalEventData NormalEventData = (NormalEventData)EventData;

        foreach (Decision decision in _filters)
        {
            if (decision.Decide(NormalEventData.Emitter, triggerHolder) == false)
            {
                return false;
            }
        }

        return true;
    }
}
