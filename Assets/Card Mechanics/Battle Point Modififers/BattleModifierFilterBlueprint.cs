using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BattleModifierFilterBlueprint
{
    public bool FilterByAttacker;
    [ShowIf("FilterByAttacker")]
    public BattleModifierObjectFilter AttackerFilter;

    public bool FilterByOpponent;
    [ShowIf("FilterByOpponent")]
    public BattleModifierObjectFilter OpponentFilter;

    public bool FilterByComparison;
    [ShowIf("FilterByComparison")]
    public BattleModifierComparisonFilter ComparisonFilter;

    [ShowInInspector]
    public string Descrtiption => GetDescription();

    public string GetDescription()
    {
        string description = "against ";

        if (FilterByComparison && ComparisonFilter != null)
        {
            description += GetComparisonText(ComparisonFilter.comparison) + " ";
        }

        if (FilterByAttacker && AttackerFilter != null)
        {
            if (AttackerFilter.DecisionType.HasFlag(DecisionType.Color))
            {
                description += AttackerFilter.PredefinedColor.ToString().ToLower();
                description += " ";
            }

            if (AttackerFilter.DecisionType.HasFlag(DecisionType.Parity))
            {
                description += AttackerFilter.PredefinedParity.ToString().ToLower();
                description += " ";
            }
        }

        if (FilterByOpponent && OpponentFilter != null)
        {
            if (OpponentFilter.DecisionType.HasFlag(DecisionType.Color))
            {
                description += OpponentFilter.PredefinedColor.ToString().ToLower();
                description += " ";
            }

            if (OpponentFilter.DecisionType.HasFlag(DecisionType.Parity))
            {
                description += OpponentFilter.PredefinedParity.ToString().ToLower();
                description += " ";
            }
        }

        description += "cards ";

        if (FilterByAttacker && AttackerFilter != null && AttackerFilter.DecisionType.HasFlag(DecisionType.PointComparison))
        {
            description += "with " + GetComparisonToValueText(AttackerFilter.comparison) + " ";

            description += AttackerFilter.PredefinedAmount.ToString() + " points";
        }

        if (FilterByOpponent && OpponentFilter != null && OpponentFilter.DecisionType.HasFlag(DecisionType.PointComparison))
        {
            description += "with " + GetComparisonToValueText(OpponentFilter.comparison) + " ";

            description += OpponentFilter.PredefinedAmount.ToString() + " points";
        }

        return description;
    }

    private string GetComparisonToValueText(Comparison comparison)
    {
        return comparison switch
        {
            Comparison.BiggerThan => "higher than",
            Comparison.Equals => "exactly",
            Comparison.SmallerThan => "lower than",
            Comparison.BiggerOrEquals => "or more",
            Comparison.SmallerOrEquals => "or less",
            _ => "",
        };
    }

    private string GetComparisonText(Comparison comparison)
    {
        return comparison switch
        {
            Comparison.BiggerThan => "higher",
            Comparison.Equals => "equal",
            Comparison.SmallerThan => "lower",
            Comparison.BiggerOrEquals => "equal or higher",
            Comparison.SmallerOrEquals => "equal or lower",
            _ => "",
        };
    }
}

[Serializable]
public class BattleModifierObjectFilter
{
    public DecisionType DecisionType;

    [ShowIf("@ShouldShowComparisonType()")]
    public Comparison comparison;

    [ShowIf("@ShouldShowAmount()")]
    public int PredefinedAmount;

    [ShowIf("@ShouldShowColor()")]
    public CardColor PredefinedColor;

    [ShowIf("@ShouldShowParity()")]
    public Parity PredefinedParity;

    private bool ShouldShowComparisonType() => DecisionType.HasFlag(DecisionType.PointComparison);
    private bool ShouldShowDamageDecisionComparisonObject() => DecisionType.HasFlag(DecisionType.PointComparison);
    private bool ShouldShowAmount() => DecisionType.HasFlag(DecisionType.PointComparison);
    private bool ShouldShowColor() => DecisionType.HasFlag(DecisionType.Color);
    private bool ShouldShowAffinityComparison() => DecisionType.HasFlag(DecisionType.Affinity);
    private bool ShouldShowParity() => DecisionType.HasFlag(DecisionType.Parity);
}

[Serializable]
public class BattleModifierComparisonFilter
{
    public Comparison comparison;
}