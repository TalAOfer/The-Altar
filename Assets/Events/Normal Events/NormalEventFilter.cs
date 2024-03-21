using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEventFilter : IEventTriggerFilter
{
    private readonly List<Decision> _emitterFilters;
    public NormalEventFilter(NormalEventFilterBlueprint blueprint)
    {
        if (blueprint.FilterByEmitter)
        {
            if (blueprint.EmitterFilter.DecisionType.HasFlag(DecisionType.Affinity))
            {
                AffinityDecision affinityFilter = new(Compare.Card, CompareTo.AnotherCard, blueprint.EmitterFilter.AffinityComparison);
                _emitterFilters.Add(affinityFilter);
            }

            if (blueprint.EmitterFilter.DecisionType.HasFlag(DecisionType.Color))
            {
                ColorDecision colorFilter = new(Compare.Card, CompareTo.Value, blueprint.EmitterFilter.PredefinedColor);
                _emitterFilters.Add(colorFilter);
            }

            if (blueprint.EmitterFilter.DecisionType.HasFlag(DecisionType.Parity))
            {
                ParityDecision parityDecision = new(Compare.Card, CompareTo.Value, blueprint.EmitterFilter.PredefinedParity);
                _emitterFilters.Add(parityDecision);
            }
        }
    }

    public bool Decide(Card triggerHolder, IEventData EventData)
    {
        NormalEventData NormalEventData = (NormalEventData)EventData;

        foreach (Decision decision in _emitterFilters)
        {
            if (decision.Decide(NormalEventData.Emitter, triggerHolder) == false)
            {
                return false;
            }
        }

        return true;
    }
}
