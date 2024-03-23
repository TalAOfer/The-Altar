using Sirenix.OdinInspector;
using System;

[Serializable]
public class LocalAmountEventFilterBlueprint
{
    public bool FilterByInflictor = false;
    [ShowIf("FilterByInflictor")]
    public AmountEventObjectFilterBlueprint InflictorFilter;

    public bool FilterByAmount = false;
    [ShowIf("FilterByAmount")]
    public AmountEventAmountFilterBlueprint AmountFilter;

    [ShowInInspector]
    public string Descrtiption => GetDescription();

    public string GetCardFilterDescription()
    {
        string description = "";

        if (FilterByInflictor)
        {
            if (InflictorFilter.DecisionType.HasFlag(DecisionType.Affinity))
            {
                description += InflictorFilter.AffinityComparison.ToString().ToLower();
                description += " ";
            }

            if (InflictorFilter.DecisionType.HasFlag(DecisionType.Color))
            {
                description += InflictorFilter.PredefinedColor.ToString().ToLower();
                description += " ";
            }

            if (InflictorFilter.DecisionType.HasFlag(DecisionType.Parity))
            {
                description += InflictorFilter.PredefinedParity.ToString().ToLower();
                description += " ";
            }
        }

        return description;
    }
    public string GetAmountFilterDescription()
    {
        string description = "";

        if (FilterByAmount)
        {
            description += GetComparisonText(AmountFilter.Comparison) + " ";

            description += AmountFilter.PredefinedAmount.ToString();
        }

        return description;
    }
    public string GetDescription()
    {
        string description = "Whenever this card has taken ";

        description += GetAmountFilterDescription();

        description += " {effect} from a ";

        description += GetCardFilterDescription();

        description += " card";

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
