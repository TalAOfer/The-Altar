using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    public EventRegistry events;
    public BattleRoomDataProvider data;

    public Card parentCard;
    public EffectApplicationType effectApplicationType;
    public EffectTarget targetStrategy;
    protected float predelay;
    protected float postdelay;
    protected int amountOfTargets;
    protected bool isConditional;
    protected Decision decision;
    protected GetAmountStrategy amountStrategy;
    public int defaultAmount;
    public EffectTrigger trigger;
    public Effect(EffectBlueprint blueprint, BattleRoomDataProvider data, EffectTrigger trigger, Card parentCard)
    {
        this.data = data;
        this.parentCard = parentCard;
        this.trigger = trigger;
        events = Locator.Events;

        effectApplicationType = blueprint.applicationType;
        predelay = blueprint.predelay;
        postdelay = blueprint.postdelay;
        targetStrategy = blueprint.target;
        amountOfTargets = blueprint.amountOfTargets;
        amountStrategy = blueprint.amountStrategy;
        defaultAmount = blueprint.amount;
        isConditional = blueprint.isConditional;
        decision = blueprint.decision;
    }

    public IEnumerator Trigger()
    {
        yield return Tools.GetWait(predelay);

        yield return Apply();

        yield return Tools.GetWait(postdelay);
    }

    public virtual IEnumerator Apply()
    {
        List<Card> targetCards = data.GetTargets(parentCard, targetStrategy, amountOfTargets);
        List<Card> validTargetCards = new(targetCards);

        //Remove cards that don't fulfill the conditions of the effect
        foreach (Card targetCard in targetCards)
        {
            if (isConditional && !decision.Decide(targetCard, data.GetOpponent(targetCard)))
            {
                validTargetCards.Remove(targetCard);
            }
        }

        if (validTargetCards.Count > 0)
        {
            parentCard.visualHandler.Animate("Jiggle");
            yield return Tools.GetWait(1f);
        }

        foreach (var targetCard in validTargetCards)
        {
            //Keep cards from applying support effects on themselves
            if (targetStrategy is not EffectTarget.InitiatingCard)
            {
                if (parentCard == targetCard) continue;
                if (targetCard.IsDead) continue;
            }
            
            int amount = amountStrategy is GetAmountStrategy.Value ? defaultAmount 
                : data.GetAmount(targetCard, amountStrategy);

            //Catch trying to get non-existing data
            //Like: if card needs the higest value of a card on map but he is the only card on map
            if (amount == -10) continue;

            yield return ApplyEffect(targetCard, amount);
            targetCard.visualHandler.Animate("FlashOut");
        }
    }

    public abstract IEnumerator ApplyEffect(Card target, int amount);

    public void RaiseEffectAppliedEvent(Card target, int amount)
    {
        events.OnEffectApplied.Raise(parentCard, new EffectIndication(GetEffectIndicationString(target, amount), target));
    }

    public abstract string GetEffectIndicationString(Card target, int amount);
}
