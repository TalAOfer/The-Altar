using System;

[Serializable]
public class DamageEventDecision
{
    public DecisionType decisionType;

    private readonly DamageDecisionObject _damageDecisionObject;
    private readonly DamageDecisionObject _damageDecisionComparisonObject;

    private Decision decision;

    public DamageEventDecision(DamageEventDecisionBlueprint blueprint)
    {
        _damageDecisionObject = blueprint.DamageDecisionComparisonObject;
        _damageDecisionComparisonObject = blueprint.DamageDecisionComparisonObject;

        Compare compare = _damageDecisionObject is not DamageDecisionObject.Amount ? Compare.Object : Compare.Value;
        CompareTo compareTo = _damageDecisionComparisonObject is DamageDecisionObject.Amount ? CompareTo.Value : CompareTo.AnotherObject;

        decision = new Decision(compare, compareTo);
    }

    public bool Decide(Card triggerHolder, DamageEventData DamageEventData)
    {
        Card baseCard = null;
        Card comparisonCard = null;
        int amount = DamageEventData.Amount;

        switch (_damageDecisionObject)
        {
            case DamageDecisionObject.TriggerHolder:
                baseCard = triggerHolder;
                break;
            case DamageDecisionObject.Reciever:
                baseCard = DamageEventData.Reciever;
                break;
            case DamageDecisionObject.Inflictor:
                baseCard = DamageEventData.Inflictor;
                break;
            case DamageDecisionObject.Amount:
                amount = DamageEventData.Amount;
                break;
        }

        switch (_damageDecisionComparisonObject)
        {
            case DamageDecisionObject.TriggerHolder:
                comparisonCard = triggerHolder;
                break;
            case DamageDecisionObject.Reciever:
                comparisonCard = DamageEventData.Reciever;
                break;
            case DamageDecisionObject.Inflictor:
                comparisonCard= DamageEventData.Inflictor;
                break;
        }

        return decision.Decide(baseCard, comparisonCard, amount);
    }
}
public enum DamageDecisionObject
{
    TriggerHolder,
    Reciever,
    Inflictor,
    Amount
}