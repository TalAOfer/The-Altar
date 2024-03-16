using Sirenix.OdinInspector;
using System;

[Serializable]
public class DamageTriggerFilterBlueprint
{
    public bool FilterByReceiver = false;
    [ShowIf("FilterByReceiver")]
    public DamageObjectTriggerFilter RecieverFilter;

    public bool FilterByAmount = false;
    [ShowIf("FilterByAmount")]
    public DamageAmountTriggerFilter AmountFilter;

    [ShowInInspector]
    public string Descrtiption => GetDescription();

    public string GetDescription()
    {
        string description = "After a ";

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

        //description += decisionType + decisionObject;

        description += "card has taken ";

        if (FilterByAmount)
        {
            description += GetComparisonText(AmountFilter.Comparison) + " ";

            description += AmountFilter.DecisionComparisonObject is DamageDecisionComparisonObject.Value ?
                AmountFilter.PredefinedAmount.ToString() + " " :
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

[Serializable]
public class DamageObjectTriggerFilter
{
    public DecisionType DecisionType;

    [ShowIf("@ShouldShowDamageDecisionComparisonObject()")]
    public DamageDecisionComparisonObject DecisionComparisonObject;

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

    private bool ShouldShowComparisonType() => DecisionType.HasFlag(DecisionType.PointComparison);
    private bool ShouldShowDamageDecisionComparisonObject() => DecisionType.HasFlag(DecisionType.PointComparison) && DecisionComparisonObject is not DamageDecisionComparisonObject.Value;
    private bool ShouldShowAmount() => DecisionType.HasFlag(DecisionType.PointComparison) && DecisionComparisonObject is DamageDecisionComparisonObject.Value;
    private bool ShouldShowColor() => DecisionType.HasFlag(DecisionType.Color);
    private bool ShouldShowAffinityComparison() => DecisionType.HasFlag(DecisionType.Affinity);
    private bool ShouldShowParity() => DecisionType.HasFlag(DecisionType.Parity);
}

[Serializable]
public class DamageAmountTriggerFilter
{
    public Comparison Comparison;

    public DamageDecisionComparisonObject DecisionComparisonObject;

    [ShowIf("DecisionComparisonObject", DamageDecisionComparisonObject.Value)]
    public int PredefinedAmount;
}

public enum DamageDecisionComparisonObject
{
    TriggerHolder,
    Reciever,
    Inflictor,
    Value
}
