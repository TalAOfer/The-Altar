using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmountTrigger
{
    public TriggerType triggerType;
    public AmountEventFilterBlueprint filter;

    [ShowInInspector]
    public string Descrtiption => GetDescription();

    public string GetDescription()
    {
        string description = "Whenever a ";

        if (filter.RecieverFilter.DecisionType.HasFlag(DecisionType.Affinity))
        {
            description += filter.RecieverFilter.AffinityComparison.ToString().ToLower();
            description += " ";
        }

        if (filter.RecieverFilter.DecisionType.HasFlag(DecisionType.Color))
        {
            description += filter.RecieverFilter.PredefinedColor.ToString().ToLower();
            description += " ";
        }

        if (filter.RecieverFilter.DecisionType.HasFlag(DecisionType.Parity))
        {
            description += filter.RecieverFilter.PredefinedParity.ToString().ToLower();
            description += " ";
        }

        //description += decisionType + decisionObject;

        description += "card has taken ";

        if (filter.FilterByAmount)
        {
            description += GetComparisonText(filter.AmountFilter.Comparison) + " ";

            description += filter.AmountFilter.ComparisonObject is AmountEventComparisonObject.Value ?
                filter.AmountFilter.PredefinedAmount.ToString() + " " :
                "NotImplemented ";
        }

        description += "damage";

        return description;
    }
    private string GetComparisonText(Comparison comparison)
    {
        return comparison switch
        {
            Comparison.BiggerThan => "more than",
            Comparison.Equals => "exactly",
            Comparison.SmallerThan => "less than",
            Comparison.BiggerOrEquals => "or more",
            Comparison.SmallerOrEquals => "or less",
            _ => "",
        };
    }
}
