using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class EffectApplier : MonoBehaviour
{
    public Card parentCard;
    public RoomData data;
    bool isConditional;
    Decision decision;
    GetAmountStrategy getAmountStrategy;
    public int defaultAmount;
    protected EffectTrigger triggerType;
    public void BaseInitialize(Card parentCard, RoomData data, bool isConditional, Decision decision, GetAmountStrategy getAmountStrategy, int defaultAmount, EffectTrigger triggerType)
    {
        this.parentCard = parentCard;
        this.data = data;
        this.isConditional = isConditional;
        this.decision = decision;
        this.getAmountStrategy = getAmountStrategy;
        this.defaultAmount = defaultAmount;
        this.triggerType = triggerType;
    }

    public IEnumerator Apply(Card target, RoomData data)
    {
        if (isConditional && !decision.Decide(target, data.GetOpponent(target))) yield break;
        parentCard.visualHandler.Animate("Jiggle");

        yield return new WaitForSeconds(1f);
        yield return ApplyEffect(target);
        target.visualHandler.Animate("FlashOut");
    }

    public abstract IEnumerator ApplyEffect(Card target);

    public int GetAmount()
    {
        int amount = 0;

        switch (getAmountStrategy)
        {
            case GetAmountStrategy.Value:
                amount = defaultAmount;
                break;
            case GetAmountStrategy.EmptySpacesOnMap:
                amount = data.GetEmptySpacesAmount();
                break;
            case GetAmountStrategy.EnemiesOnMap:
                amount = data.GetAmountOfEnemies();
                break;
            case GetAmountStrategy.CardsInHand:
                amount = data.PlayerManager.activeCards.Count;
                break;
            case GetAmountStrategy.RoomCount:
                amount = data.GetRoomIndex();
                break;

            case GetAmountStrategy.NotImplementedDeadEnemiesOnMap:
                break;
            case GetAmountStrategy.LowestValueEnemyCard:
                amount = data.GetLowestEnemyCardValue();
                break;
        }

        return amount;
    }
}
