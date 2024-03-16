using System.Collections;
using System.Collections.Generic;


public abstract class Effect
{
    public EffectApplicationType EffectApplicationType { get; private set; }
    public EffectTarget TargetStrategy { get; private set; }
    public int AmountOfTargets { get; private set; }
    public EffectTrigger EffectTrigger { get; private set; }
    public Card ParentCard { get; private set; }

    protected EventRegistry _events;
    protected BattleRoomDataProvider _data;
    protected bool _isConditional;
    //protected Decision _decision;
    protected GetAmountStrategy _amountStrategy;
    protected int _defaultAmount;

    public Effect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard)
    {
        _events = Locator.Events;

        _data = data;
        ParentCard = parentCard;
        EffectTrigger = trigger;

        if (blueprint != null)
        {
            EffectApplicationType = blueprint.applicationType;
            TargetStrategy = blueprint.target;
            AmountOfTargets = blueprint.amountOfTargets;

            _amountStrategy = blueprint.amountStrategy;
            _defaultAmount = blueprint.amount;
            //_isConditional = blueprint.isConditional;
            //_decision = blueprint.decision;
        }
    }

    public virtual IEnumerator Trigger()
    {
        List<Card> targetCardsBeforeDecisions = _data.GetTargets(ParentCard, TargetStrategy, AmountOfTargets);

        List<Card> validTargetCards = GetTargetsThatMeetConditions(targetCardsBeforeDecisions);

        if (validTargetCards.Count > 0)
        {
            ParentCard.visualHandler.Animate("Jiggle");
            yield return Tools.GetWait(1f);
        }

        yield return ApplyEffectOnTargets(validTargetCards);
    }

    public virtual void OnRemoveEffect(Card targetCard)
    {

    }

    private List<Card> GetTargetsThatMeetConditions(List<Card> targetCards)
    {
        List<Card> validTargetCards = new(targetCards);
        if (this is AlterBattlePointsEffect) return validTargetCards;

        //Remove cards that don't fulfill the conditions of the effect
        foreach (Card targetCard in targetCards)
        {
            //if (_isConditional && !_decision.Decide(targetCard, _data.GetOpponent(targetCard)))
            //{
            //    validTargetCards.Remove(targetCard);
            //}
        }

        return validTargetCards;
    }

    private IEnumerator ApplyEffectOnTargets(List<Card> targetCards)
    {
        foreach (var targetCard in targetCards)
        {
            //Keep cards from applying support effects on themselves
            if (TargetStrategy is not EffectTarget.InitiatingCard)
            {
                if (ParentCard == targetCard) continue;
                if (targetCard.IsDead) continue;
            }

            yield return ApplyEffect(targetCard);
            targetCard.visualHandler.Animate("FlashOut");
        }
    }

    public abstract IEnumerator ApplyEffect(Card target);

    public int GetAmount(Card target)
    {
        return _amountStrategy is GetAmountStrategy.Value ? _defaultAmount
                : _data.GetAmount(target, _amountStrategy);
    }


    public void RaiseEffectAppliedEvent(Card target, int amount)
    {
        _events.OnEffectApplied.Raise(ParentCard, new EffectIndication(GetEffectIndicationString(target, amount), target));
    }

    public virtual string GetEffectIndicationString(Card target, int amount)
    {
        return "";
    }
}
