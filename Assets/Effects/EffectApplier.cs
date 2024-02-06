using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class EffectApplier : MonoBehaviour
{
    public Card parentCard;
    public RoomData data;

    protected EffectTrigger triggerType;
    public void BaseInitialize(Card parentCard, RoomData data, EffectTrigger triggerType)
    {
        this.parentCard = parentCard;
        this.data = data;
        this.triggerType = triggerType;
    }

    public IEnumerator Apply(Card target, int amount)
    {
        yield return ApplyEffect(target, amount);
        target.visualHandler.Animate("FlashOut");
    }

    public abstract IEnumerator ApplyEffect(Card target, int amount);
}
