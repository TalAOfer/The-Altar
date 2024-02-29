using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class EffectApplier : MonoBehaviour
{
    public Card parentCard;
    protected DataProvider data;
    protected PlayerActionProvider actions; 
    protected EventRegistry events;

    protected EffectTrigger triggerType;
    public void BaseInitialize(Card parentCard, EffectTrigger triggerType)
    {
        data = Locator.DataProvider;
        events = Locator.Events;

        this.parentCard = parentCard;
        this.triggerType = triggerType;
    }

    public IEnumerator Apply(Card target, int amount)
    {
        yield return ApplyEffect(target, amount);
        target.visualHandler.Animate("FlashOut");
    }

    public abstract IEnumerator ApplyEffect(Card target, int amount);

    public void RaiseEffectAppliedEvent(Card target, int amount)
    {
        events.OnEffectApplied.Raise(this, new EffectIndication(GetEffectIndicationString(target, amount), target));
    }

    public abstract string GetEffectIndicationString(Card target, int amount);
}
