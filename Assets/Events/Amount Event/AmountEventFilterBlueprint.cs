using Sirenix.OdinInspector;
using System;
using Unity.VisualScripting;

[Serializable]
public class AmountEventFilterBlueprint
{
    public bool FilterByReceiver = false;
    [ShowIf("FilterByReceiver")]
    public AmountEventObjectFilterBlueprint RecieverFilter;

    public bool FilterByAmount = false;
    [ShowIf("FilterByAmount")]
    public AmountEventAmountFilter AmountFilter;

    [ShowInInspector]
    public string Descrtiption => GetDescription();

    public string GetCardFilterDescription()
    {
        string description = "";

        if (FilterByReceiver)
        {
            if (RecieverFilter.DecisionType.HasFlag(DecisionType.Affinity))
            {
                description += RecieverFilter.AffinityComparison.ToString().ToLower();
                description += " ";
            }

            if (RecieverFilter.DecisionType.HasFlag(DecisionType.Color))
            {
                description += RecieverFilter.PredefinedColor.ToString().ToLower();
                description += " ";
            }

            if (RecieverFilter.DecisionType.HasFlag(DecisionType.Parity))
            {
                description += RecieverFilter.PredefinedParity.ToString().ToLower();
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

            description += AmountFilter.ComparisonObject is AmountEventComparisonObject.Value ?
                AmountFilter.PredefinedAmount.ToString() + " " :
                "NotImplemented ";
        }

        return description;
    }
    public string GetDescription()
    {
        string description = "Whenever a ";

        description += GetCardFilterDescription();

        description += "card has taken ";

        description += GetAmountFilterDescription();

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

[Serializable]
public class AmountEventObjectFilterBlueprint
{
    public DecisionType DecisionType;

    [ShowIf("@ShouldShowDamageDecisionComparisonObject()")]
    public AmountEventComparisonObject DecisionComparisonObject;

    [ShowIf("@ShouldShowComparisonType()")]
    public Comparison comparison;

    [ShowIf("@ShouldShowAmount()")]
    public int PredefinedAmount;

    [ShowIf("@ShouldShowColor()")]
    public CardColor PredefinedColor;

    [ShowIf("@ShouldShowAffinityComparison()")]
    public AffinityComparison AffinityComparison;

    [ShowIf("@ShouldShowParity()")]
    public Parity PredefinedParity;

    #region Inspector Helpers
    private bool ShouldShowComparisonType() => DecisionType.HasFlag(DecisionType.PointComparison);
    private bool ShouldShowDamageDecisionComparisonObject() => DecisionType.HasFlag(DecisionType.PointComparison) && DecisionComparisonObject != AmountEventComparisonObject.Value;
    private bool ShouldShowAmount() => DecisionType.HasFlag(DecisionType.PointComparison) && DecisionComparisonObject == AmountEventComparisonObject.Value;
    private bool ShouldShowColor() => DecisionType.HasFlag(DecisionType.Color);
    private bool ShouldShowAffinityComparison() => DecisionType.HasFlag(DecisionType.Affinity);
    private bool ShouldShowParity() => DecisionType.HasFlag(DecisionType.Parity);

    #endregion
}

[Serializable]
public class AmountEventAmountFilter
{
    public Comparison Comparison;

    public AmountEventComparisonObject ComparisonObject;

    [ShowIf("ComparisonObject", AmountEventComparisonObject.Value)]
    public int PredefinedAmount;
}

public enum AmountEventComparisonObject
{
    TriggerHolder,
    Reciever,
    Inflictor,
    Value
}
