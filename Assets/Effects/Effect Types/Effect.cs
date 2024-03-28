using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    //Base
    public Card ParentCard { get; private set; }
    public EffectTypeAsset EffectTypeAsset { get; private set; }
    public bool IsPersistent { get; private set; }
    public bool IsFrozenTillEndOfTurn { get; private set; }

    //Trigger
    public EffectTriggerAsset EffectTrigger { get; private set; }
    public IEventTriggerFilter TriggerFilter { get; private set; }

    //Target Acquiry 
    public EffectTargetPool EffectTargetPool { get; private set; }
    public EffectTargetStrategy EffectTargetStrategy { get; private set; }
    public int AmountOfTargets { get; private set; }
    public NormalEventFilter TargetFilter { get; private set; }

    //Providers
    protected EventRegistry _events;
    protected BattleRoomDataProvider _data;

    //Amount
    protected GetAmountStrategy _amountStrategy;
    protected int _defaultAmount;

    public Effect(EffectBlueprint blueprint, BattleRoomDataProvider data, Card parentCard)
    {
        _events = Locator.Events;

        _data = data;
        ParentCard = parentCard;

        if (blueprint != null)
        {
            EffectTypeAsset = blueprint.EffectTypeAsset;
            EffectTrigger = blueprint.Trigger;
            IsPersistent = blueprint.IsPersistent;
            EffectTargetPool = blueprint.TargetPool;
            EffectTargetStrategy = blueprint.TargetStrategy;
            AmountOfTargets = blueprint.AmountOfTargets;


            _amountStrategy = blueprint.amountStrategy;
            _defaultAmount = blueprint.amount;

            if (EffectTrigger != null)
            {
                switch (EffectTrigger.TriggerArchetype)
                {
                    case TriggerArchetype.LocalNormalEvents:
                        TriggerFilter = new NormalEventFilter(blueprint.NormalEventsFilter);
                        break;
                    case TriggerArchetype.GlobalNormalEvents:
                        TriggerFilter = new NormalEventFilter(blueprint.NormalEventsFilter);
                        break;
                    case TriggerArchetype.LocalAmountEvents:
                        TriggerFilter = new LocalAmountTriggerFilter(blueprint.LocalAmountEventsFilter);
                        break;
                    case TriggerArchetype.GlobalAmountEvents:
                        TriggerFilter = new GlobalAmountTriggerFilter(blueprint.GlobalAmountEventsFilter);
                        break;
                }
            }

            if (blueprint.ShouldFilterTargets)
            {
                TargetFilter = new NormalEventFilter(blueprint.TargetFilterBlueprint);
            }
        }
    }

    public virtual IEnumerator Trigger(List<Card> predefinedTargets = null, IEventData eventData = null)
    {
        if (EffectTrigger != null && EffectTrigger.IsOnePerTurn) this.IsFrozenTillEndOfTurn = true;

        if (predefinedTargets != null)
        {
            yield return ApplyEffectOnTargets(predefinedTargets);
        }

        else
        {
            List<Card> validTargetCards = _data.GetTargets(ParentCard, EffectTargetPool, TargetFilter,
                EffectTypeAsset.TargetOnlyAliveCards, EffectTargetStrategy, AmountOfTargets, eventData);

            if (validTargetCards.Count > 0)
            {
                if (EffectTrigger.AnimationSpritePrefab != null)
                {
                    Pooler.Spawn(EffectTrigger.AnimationSpritePrefab, ParentCard.transform.position, Quaternion.identity);
                    ParentCard.visualHandler.Animate("Jiggle");
                    ParentCard.visualHandler.Animate("FlashOut");
                }
                yield return Tools.GetWait(1f);
            }

            yield return ApplyEffectOnTargets(validTargetCards);
        }
    }

    public virtual void OnRemoveEffect(Card targetCard)
    {

    }

    private IEnumerator ApplyEffectOnTargets(List<Card> targetCards)
    {
        foreach (var targetCard in targetCards)
        {
            //Keep cards from applying support effects on themselves
            //if (EffectTargetPool is not EffectTargetPool.InitiatingCard)
            //{
            //    if (ParentCard == targetCard) continue;
            //}

            yield return ApplyEffect(targetCard);
            //targetCard.visualHandler.Animate("FlashOut");
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

    public void UnfreezeEffect()
    {
        this.IsFrozenTillEndOfTurn = false;
    }
}
