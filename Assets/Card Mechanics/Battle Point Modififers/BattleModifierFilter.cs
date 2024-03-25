using System;
using System.Collections.Generic;

[Serializable]
public class BattleModifierFilter
{
    private readonly List<Decision> _attackerFilters = new();
    private readonly List<Decision> _opponentFilters = new();
    private readonly AmountComparisonDecision _comparisonFilter;

    public BattleModifierFilter(BattleModifierFilterBlueprint blueprint)
    {
        if (blueprint.FilterByAttacker)
        {
            if (blueprint.AttackerFilter.DecisionType.HasFlag(DecisionType.PointComparison))
            {
                AmountComparisonDecision amountFilter = new(Compare.Card, CompareTo.Value, blueprint.AttackerFilter.comparison, blueprint.AttackerFilter.PredefinedAmount);
                _attackerFilters.Add(amountFilter);
            }

            if (blueprint.AttackerFilter.DecisionType.HasFlag(DecisionType.Color))
            {
                ColorDecision colorFilter = new(Compare.Card, CompareTo.Value, blueprint.AttackerFilter.PredefinedColor);
                _attackerFilters.Add(colorFilter);
            }

            if (blueprint.AttackerFilter.DecisionType.HasFlag(DecisionType.Parity))
            {
                ParityDecision parityDecision = new(Compare.Card, CompareTo.Value, blueprint.AttackerFilter.PredefinedParity);
                _attackerFilters.Add(parityDecision);
            }
        }

        if (blueprint.FilterByOpponent)
        {
            if (blueprint.OpponentFilter.DecisionType.HasFlag(DecisionType.PointComparison))
            {
                AmountComparisonDecision amountFilter = new(Compare.Card, CompareTo.Value, blueprint.OpponentFilter.comparison, blueprint.OpponentFilter.PredefinedAmount);
                _opponentFilters.Add(amountFilter);
            }

            if (blueprint.OpponentFilter.DecisionType.HasFlag(DecisionType.Color))
            {
                ColorDecision colorFilter = new(Compare.Card, CompareTo.Value, blueprint.OpponentFilter.PredefinedColor);
                _opponentFilters.Add(colorFilter);
            }

            if (blueprint.OpponentFilter.DecisionType.HasFlag(DecisionType.Parity))
            {
                ParityDecision parityDecision = new(Compare.Card, CompareTo.Value, blueprint.OpponentFilter.PredefinedParity);
                _opponentFilters.Add(parityDecision);
            }
        }

        if (blueprint.FilterByComparison)
        {
            _comparisonFilter = new(Compare.Card, CompareTo.AnotherCard, blueprint.ComparisonFilter.comparison);
        }
    }
    
    public bool Decide(Card card, Card opponentCard)
    {
        foreach (var filter in _attackerFilters)
        {
            if (filter.Decide(card) == false)
            {
                return false;
            }
        }

        foreach (var filter in _opponentFilters)
        {
            if (filter.Decide(opponentCard) == false)
            {
                return false;
            }
        }

        if (_comparisonFilter?.Decide(card, opponentCard) == false)
        {
            return false;
        }

        return true;
    }
}
