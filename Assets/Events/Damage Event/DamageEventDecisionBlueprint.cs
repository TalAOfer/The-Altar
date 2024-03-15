using Sirenix.OdinInspector;
using System;

[Serializable]
public class DamageEventDecisionBlueprint
{
    public DecisionType DecisionType;
    public DamageDecisionObject DamageDecisionObject;
    public DamageDecisionObject DamageDecisionComparisonObject;

    [ShowIf("DecisionType", DecisionType.PointComparison)]
    public Comparison comparison;

    [ShowIf("@ShouldShowAmount()")]
    public int PredeterminedAmount;

    [ShowIf("@ShouldShowColor()")]
    public CardColor predeterminedColor;

    [ShowIf("DecisionType", DecisionType.Affinity)]
    public AffinityComparison affinityComparison;

    private bool ShouldShowAmount() => DecisionType is DecisionType.PointComparison && DamageDecisionComparisonObject is DamageDecisionObject.Amount;
    private bool ShouldShowColor() => DecisionType is DecisionType.Color && DamageDecisionComparisonObject is DamageDecisionObject.Amount;



    [ShowInInspector]
    public string Descrtiption => GetDescription();

    public string GetDescription()
    {
        string description = "This effect will happen when a ";

        string decisionObject = "";

        switch (DamageDecisionObject)
        {
            case DamageDecisionObject.TriggerHolder:
                break;
            case DamageDecisionObject.Reciever:
                decisionObject = "card is damaged";
                break;
            case DamageDecisionObject.Inflictor:
                break;
            case DamageDecisionObject.Amount:
                break;
        }

        string decisionType = "";

        switch (DecisionType)
        {
            case DecisionType.PointComparison:
                break;
            case DecisionType.Parity:
                break;
            case DecisionType.Color:
                decisionType = predeterminedColor.ToString() + " ";
                break;
            case DecisionType.Affinity:
                break;
        }

        description += decisionType + decisionObject;

        return description;
    }
}
