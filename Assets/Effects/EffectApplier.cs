using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class EffectApplier : MonoBehaviour
{
    public Card parentCard;
    public RoomData data;
    bool isConditional;
    Decision decision;
    ApplierModifierType amountModifier;
    public void BaseInitialize(Card parentCard, RoomData data, bool isConditional, Decision decision, ApplierModifierType amountModifier)
    {
        this.parentCard = parentCard;
        this.data = data;
        this.amountModifier = amountModifier;
        this.isConditional = isConditional;
        this.decision = decision;
    }

    public IEnumerator Apply(Card target, RoomData data)
    {
        if (isConditional && !decision.Decide(target, data.GetOpponent(target))) yield break;

        yield return ApplyEffect(target);
    }

    public abstract IEnumerator ApplyEffect(Card target);

    public int AmountGetter()
    {
        int amount = 0;

        switch (amountModifier)
        {
            case ApplierModifierType.EmptySpacesOnMap:
                
                break;
            case ApplierModifierType.EnemiesOnMap:
                break;
            case ApplierModifierType.CardsInHand:
                break;
            case ApplierModifierType.RoomCount:
                break;
        }

        return amount;
    }
}
