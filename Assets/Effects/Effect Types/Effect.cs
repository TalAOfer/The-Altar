using System.Collections;
using System.Collections.Generic;

public abstract class Effect
{
    public bool IsPersistent { get; private set; }
    public EffectTargetPool EffectTargetPool { get; private set; }

    public EffectTargetStrategy EffectTargetStrategy { get; private set; }
    public int AmountOfTargets { get; private set; }
    public EffectTrigger EffectTrigger { get; private set; }
    public Card ParentCard { get; private set; }

    public IEventTriggerFilter Filter { get; private set; }

    protected EventRegistry _events;
    protected BattleRoomDataProvider _data;

    protected GetAmountStrategy _amountStrategy;
    protected int _defaultAmount;

    public Effect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard)
    {
        _events = Locator.Events;

        _data = data;
        ParentCard = parentCard;
        
        if (blueprint != null)
        {
            EffectTrigger = blueprint.Trigger;
            IsPersistent = blueprint.IsPersistent;
            EffectTargetPool = blueprint.TargetPool;
            EffectTargetStrategy = blueprint.TargetStrategy;
            AmountOfTargets = blueprint.AmountOfTargets;

            _amountStrategy = blueprint.amountStrategy;
            _defaultAmount = blueprint.amount;
        }
    }

    public virtual IEnumerator Trigger(List<Card> predefinedTargets = null)
    {
        if (predefinedTargets != null)
        {
            yield return ApplyEffectOnTargets(predefinedTargets);
        }

        else
        {
            List<Card> targetCardsBeforeDecisions = _data.GetTargets(ParentCard, EffectTargetPool, EffectTargetStrategy, AmountOfTargets);

            List<Card> validTargetCards = GetTargetsThatMeetConditions(targetCardsBeforeDecisions);

            if (validTargetCards.Count > 0)
            {
                ParentCard.visualHandler.Animate("Jiggle");
                yield return Tools.GetWait(1f);
            }

            yield return ApplyEffectOnTargets(validTargetCards);
        }
    }

    public virtual void OnRemoveEffect(Card targetCard)
    {

    }

    private List<Card> GetTargetsThatMeetConditions(List<Card> targetCards)
    {
        List<Card> validTargetCards = new(targetCards);
        if (this is AddBattlePointModifier) return validTargetCards;

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
            if (EffectTargetPool is not EffectTargetPool.InitiatingCard)
            {
                if (ParentCard == targetCard) continue;
            }

            yield return ApplyEffect(targetCard);
            targetCard.visualHandler.Animate("FlashOut");
        }
    }

    protected abstract IEnumerator ApplyEffect(Card target);

    protected int GetAmount(Card target)
    {
        return _amountStrategy is GetAmountStrategy.Value ? _defaultAmount
                : _data.GetAmount(target, _amountStrategy);
    }


    protected void RaiseEffectAppliedEvent(Card target, int amount)
    {
        _events.OnEffectApplied.Raise(ParentCard, new EffectIndication(GetEffectIndicationString(target, amount), target));
    }

    protected virtual string GetEffectIndicationString(Card target, int amount)
    {
        return "";
    }
}
