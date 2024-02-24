using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public AllEvents events;
    public DataProvider data;

    public Card parentCard;
    public EffectApplier applier;
    public EffectApplicationType effectApplicationType;
    public EffectTarget target;
    protected float predelay;
    protected float postdelay;
    protected int amountOfTargets;
    protected bool isConditional;
    protected Decision decision;
    GetAmountStrategy amountStrategy;
    public int defaultAmount;
    public void BaseInitialize(EffectApplier applier, Card parentCard, EffectBlueprint blueprint)
    {
        data = Locator.DataProvider;

        this.applier = applier;
        this.parentCard = parentCard;

        effectApplicationType = blueprint.applicationType;
        predelay = blueprint.predelay;
        postdelay = blueprint.postdelay;
        events = blueprint.events;
        target = blueprint.target;
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
        List<Card> targetCards = GetTarget();
        List<Card> validTargetCards = new(targetCards);
        List<Coroutine> applicationCoroutines = new();


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
            if (target is not EffectTarget.InitiatingCard)
            {
                if (parentCard == targetCard) continue;
                if (targetCard.IsDead) continue;
            }
            
            int amount = GetAmount(targetCard);
            //Catch trying to get non-existing data
            //Like: if card needs the higest value of a card on map but he is the only card on map
            if (amount == -10) continue;

            Coroutine coroutine = StartCoroutine(applier.Apply(targetCard, amount));
            applicationCoroutines.Add(coroutine);
        }


        // Wait for all coroutines to finish
        foreach (var coroutine in applicationCoroutines)
        {
            yield return coroutine;
        }
    }

    public List<Card> GetTarget()
    {
        List<Card> targets = new();

        switch (target)
        {
            case EffectTarget.InitiatingCard:
                targets.Add(parentCard);
                break;
            case EffectTarget.Oppnent:
                targets.Add(data.GetOpponent(parentCard));
                break;
            case EffectTarget.AllPlayerCards:
                targets = data.GetAllActivePlayerCards();
                break;
            case EffectTarget.AllEnemyCards:
                targets = data.GetAllActiveEnemies();
                break;
            case EffectTarget.AllCardsOnMap:
                targets = data.GetAllActiveEnemiesOnMap();
                break;
            case EffectTarget.AllCardsInHand:
                targets = data.GetAllCardsInHand();
                break;
            case EffectTarget.RandomCardOnMap:
                List<Card> randomEnemyCards = data.GetRandomEnemyCards(amountOfTargets, parentCard);
                if (randomEnemyCards != null) targets = randomEnemyCards;
                break;
            case EffectTarget.RandomCardFromHand:
                List<Card> randomPlayerCards = data.GetRandomPlayerCards(amountOfTargets, parentCard);
                if (randomPlayerCards != null) targets = randomPlayerCards;
                break;
            case EffectTarget.PlayerCardBattling:
                targets.Add(data.BattlingPlayerCard);
                break;
            case EffectTarget.EnemyCardBattling:
                targets.Add(data.BattlingEnemyCard);
                break;
            case EffectTarget.LowestPlayerCard:
                Card lowestPlayerCard = data.GetLowestPlayerCard(parentCard);
                if (lowestPlayerCard != null) targets.Add(lowestPlayerCard);
                break;
        }

        return targets;
    }

    public int GetAmount(Card targetCard)
    {
        int amount = 0;

        switch (amountStrategy)
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
                amount = data.GetAmountOfPlayerCards();
                break;
            case GetAmountStrategy.RoomCount:
                amount = data.GetRoomIndex();
                break;

            case GetAmountStrategy.NotImplementedDeadEnemiesOnMap:
                break;
            case GetAmountStrategy.LowestValueEnemyCard:
                amount = data.GetLowestEnemyCardValue(targetCard);
                break;
        }

        return amount;
    }
}
