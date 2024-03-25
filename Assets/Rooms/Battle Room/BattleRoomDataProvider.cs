using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleRoomDataProvider
{
    private readonly BattleRoomStateMachine _sm;
    public BattleRoomDataProvider(BattleRoomStateMachine SM)
    {
        _sm = SM;
    }

    #region Action Providers

    public void ReorderCards()
    {
        _sm.PlayerCardManager.Hand.ReorderPlaceholders(true);
        _sm.EnemyCardManager.ReorderPlaceholders(true);
    }
    public void RemoveCard(Card card)
    {
        if (card.Affinity is Affinity.Player) _sm.PlayerCardManager.RemoveCardFromManager(card);
        else _sm.EnemyCardManager.RemoveEnemyFromManager(card);
    }

    public void DrawCardsToHand(int amount)
    {
        CoroutineRunner.Instance.StartCoroutine(_sm.PlayerCardManager.DrawCardsToHand(amount));
    }

    public void SpawnCardToHandByArchetype(CardArchetype archetype)
    {
        _sm.PlayerCardManager.SpawnCardToHandByArchetype(archetype);
    }

    public IEnumerator SpawnEnemiesByArchetype(CardArchetype archetype, int amount)
    {
        yield return _sm.EnemyCardManager.SpawnEnemiesByArchetype(archetype, amount);
    }

    #endregion

    #region Target Providers

    public List<Card> GetTargets(Card parentCard, EffectTargetPool targetPool, NormalEventFilter filter, EffectTargetStrategy targetStrategy, int amountOfTargets, IEventData eventData)
    {
        List<Card> targets = new();

        switch (targetPool)
        {
            case EffectTargetPool.InitiatingCard:
                targets.Add(parentCard);
                break;
            case EffectTargetPool.Oppnent:
                targets.Add(GetOpponent(parentCard));
                break;
            case EffectTargetPool.AllCards:
                targets.AddRange(GetAllActivePlayerCards());
                targets.AddRange(GetAllActiveEnemyCards());
                break;
            case EffectTargetPool.SelectedCards:
                break;
            case EffectTargetPool.TriggerCard:
                Card triggerCard = null;
                switch (eventData)
                {
                    case AmountEventData amountData:
                        triggerCard = amountData.Reciever;
                        break;
                    case NormalEventData normalData:
                        triggerCard = normalData.Emitter;
                        break;
                }
                targets.Add(triggerCard);
                break;
        }

        if (targetPool is EffectTargetPool.InitiatingCard or EffectTargetPool.Oppnent or EffectTargetPool.TriggerCard) return targets;

        List<Card> validTargets = new(targets);

        if (filter != null)
        {
            foreach (Card card in targets)
            {
                if (!filter.Decide(parentCard, new NormalEventData(card)))
                    validTargets.Remove(card);
            }
        }

        switch (targetStrategy)
        {
            case EffectTargetStrategy.All:
                return validTargets;
            case EffectTargetStrategy.Random:
                return GetRandomCards(validTargets, amountOfTargets);
            case EffectTargetStrategy.Highest:
                return GetHighestCards(validTargets, amountOfTargets);
            case EffectTargetStrategy.Lowest:
                return GetLowestCards(validTargets, amountOfTargets);
            default:
                break;
        }

        return targets;
    }

    public Card GetOpponent(Card card)
    {
        return card.Affinity == Affinity.Player ? _sm.Ctx.BattlingEnemyCard : _sm.Ctx.BattlingPlayerCard;
    }

    public List<Card> GetAllActiveCards()
    {
        return GetAllActivePlayerCards().Concat(GetAllActiveEnemyCards()).ToList();
    }

    public List<Card> GetAllActiveEnemyCards()
    {
        return new(_sm.EnemyCardManager.ActiveEnemies);
    }

    public List<Card> GetAllActivePlayerCards()
    {
        return new(_sm.PlayerCardManager.ActiveCards);
    }

    public List<Card> GetRandomCards(List<Card> sourceList, int amount, Card excludeThis = null)
    {
        List<Card> randomCards = new List<Card>();

        if (sourceList == null || sourceList.Count == 0 || amount <= 0)
        {
            return randomCards;
        }

        // Create a copy of the source list to avoid modifying the original
        List<Card> tempList = new(sourceList);

        // Exclude the specified card if it's not null
        if (excludeThis != null)
        {
            tempList.Remove(excludeThis);
        }

        // Adjust the number of cards to draw if necessary, after excluding
        int drawCount = Mathf.Min(amount, tempList.Count);

        // Randomly pick cards
        for (int i = 0; i < drawCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempList.Count);
            randomCards.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        return randomCards;
    }

    public List<Card> GetLowestCards(List<Card> cardList, int amount, Card excludeThis = null)
    {
        if (cardList == null || cardList.Count == 0 || amount <= 0) return new List<Card>();

        List<Card> filteredList = excludeThis != null
            ? cardList.Where(card => card != excludeThis).ToList()
            : new List<Card>(cardList);

        // Sort the list by points in ascending order
        List<Card> sortedList = filteredList.OrderBy(card => card.points).ToList();

        // Take the first 'amount' cards from the sorted list
        return sortedList.Take(amount).ToList();
    }

    public List<Card> GetHighestCards(List<Card> cardList, int amount, Card excludeThis = null)
    {
        if (cardList == null || cardList.Count == 0 || amount <= 0) return new List<Card>();

        List<Card> filteredList = excludeThis != null
            ? cardList.Where(card => card != excludeThis).ToList()
            : new List<Card>(cardList);

        // Sort the list by points in descending order
        List<Card> sortedList = filteredList.OrderByDescending(card => card.points).ToList();

        // Take the first 'amount' cards from the sorted list
        return sortedList.Take(amount).ToList();
    }
    #endregion

    #region Amount Providers
    public int GetAmount(Card parentCard, GetAmountStrategy amountStrategy)
    {
        int amount = -10;

        switch (amountStrategy)
        {
            case GetAmountStrategy.EnemiesOnMap:
                amount = GetAmountOfEnemies();
                break;
            case GetAmountStrategy.CardsInHand:
                amount = GetAmountOfPlayerCards();
                break;
            case GetAmountStrategy.RoomCount:
                amount = GetRoomIndex();
                break;
            case GetAmountStrategy.NotImplementedDeadEnemiesOnMap:
                break;
            case GetAmountStrategy.LowestValueEnemyCard:
                amount = GetLowestEnemyCardValue(parentCard);
                break;
        }

        return amount;
    }
    public int GetAmountOfPlayerCards()
    {
        return _sm.PlayerCardManager.ActiveCards.Count;
    }

    public int GetAmountOfEnemies()
    {
        return _sm.EnemyCardManager.ActiveEnemies.Count;
    }

    public int GetRoomIndex()
    {
        return _sm.FloorCtx.CurrentRoomIndex;
    }

    public int GetLowestEnemyCardValue(Card excludeThis)
    {
        int amount = 100;

        foreach (Card card in _sm.EnemyCardManager.ActiveEnemies)
        {
            if (card == excludeThis) continue;

            if (card.points < amount)
            {
                amount = card.points;
            }
        }

        return amount;
    }

    #endregion
}


