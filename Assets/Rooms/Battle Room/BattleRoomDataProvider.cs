using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleRoomDataProvider
{
    private readonly BattleStateMachine _ctx;
    public BattleRoomDataProvider(BattleStateMachine ctx)
    {
        _ctx = ctx;
    }

    #region Target Providers

    public List<Card> GetTargets(Card parentCard, EffectTarget targetType, int amountOfTargets)
    {
        List<Card> targets = new();

        switch (targetType)
        {
            case EffectTarget.InitiatingCard:
                targets.Add(parentCard);
                break;
            case EffectTarget.Oppnent:
                targets.Add(GetOpponent(parentCard));
                break;
            case EffectTarget.AllPlayerCards:
                targets = GetAllActivePlayerCards();
                break;
            case EffectTarget.AllEnemyCards:
                targets = GetAllActiveEnemies();
                break;
            case EffectTarget.AllCardsOnMap:
                targets = GetAllActiveEnemiesOnMap();
                break;
            case EffectTarget.AllCardsInHand:
                targets = GetAllCardsInHand();
                break;
            case EffectTarget.RandomCardOnMap:
                List<Card> randomEnemyCards = GetRandomEnemyCards(amountOfTargets, parentCard);
                if (randomEnemyCards != null) targets = randomEnemyCards;
                break;
            case EffectTarget.RandomCardFromHand:
                List<Card> randomPlayerCards = GetRandomPlayerCards(amountOfTargets, parentCard);
                if (randomPlayerCards != null) targets = randomPlayerCards;
                break;
            case EffectTarget.PlayerCardBattling:
                targets.Add(_ctx.Ctx.BattlingPlayerCard);
                break;
            case EffectTarget.EnemyCardBattling:
                targets.Add(_ctx.Ctx.BattlingEnemyCard);
                break;
            case EffectTarget.LowestPlayerCard:
                Card lowestPlayerCard = GetLowestPlayerCard(parentCard);
                if (lowestPlayerCard != null) targets.Add(lowestPlayerCard);
                break;
        }

        return targets;
    }

    public Card GetOpponent(Card card)
    {
        return card.Affinity == Affinity.Player ? _ctx.Ctx.BattlingEnemyCard : _ctx.Ctx.BattlingPlayerCard;
    }

    public List<Card> GetAllActiveEnemies()
    {
        return new(_ctx.EnemyCardManager.ActiveEnemies);
    }

    public List<Card> GetAllActiveEnemiesOnMap()
    {
        List<Card> activeEnemies = new(_ctx.EnemyCardManager.ActiveEnemies);
        if (_ctx.Ctx.BattlingEnemyCard != null) activeEnemies.Remove(_ctx.Ctx.BattlingEnemyCard);
        return activeEnemies;
    }

    public List<Card> GetAllActivePlayerCards()
    {
        return new(_ctx.PlayerCardManager.ActiveCards);
    }

    public List<Card> GetAllCardsInHand()
    {
        List<Card> activeEnemies = new(_ctx.PlayerCardManager.ActiveCards);
        if (_ctx.Ctx.BattlingPlayerCard != null) activeEnemies.Remove(_ctx.Ctx.BattlingPlayerCard);
        return activeEnemies;
    }

    public List<Card> GetRandomPlayerCards(int numberOfCards, Card excludeThis)
    {
        List<Card> cardsToPickFrom = GetAllActivePlayerCards();

        if (excludeThis != null)
        {
            cardsToPickFrom.RemoveAll(card => card.Equals(excludeThis));
        }

        cardsToPickFrom.Remove(_ctx.Ctx.BattlingPlayerCard);

        // Adjust the number of cards to draw if necessary
        int drawCount = Mathf.Min(numberOfCards, cardsToPickFrom.Count);

        if (drawCount == 0)
        {
            return new List<Card>(); // Return an empty list instead of null
        }

        List<int> randomIndices = Tools.GetXUniqueRandoms(drawCount, 0, cardsToPickFrom.Count);
        List<Card> randomCards = randomIndices.Select(index => cardsToPickFrom[index]).ToList();

        return randomCards;
    }
    public List<Card> GetRandomEnemyCards(int numberOfCards, Card excludeThis)
    {
        List<Card> cardsToPickFrom = GetAllActiveEnemies();

        if (excludeThis != null)
        {
            cardsToPickFrom.RemoveAll(card => card.Equals(excludeThis));
        }

        cardsToPickFrom.Remove(_ctx.Ctx.BattlingEnemyCard);

        // Adjust the number of cards to draw if necessary
        int drawCount = Mathf.Min(numberOfCards, cardsToPickFrom.Count);

        if (drawCount == 0)
        {
            return new List<Card>(); // Return an empty list instead of null
        }

        List<int> randomIndices = Tools.GetXUniqueRandoms(drawCount, 0, cardsToPickFrom.Count);
        List<Card> randomCards = randomIndices.Select(index => cardsToPickFrom[index]).ToList();

        return randomCards;
    }

    public Card GetLowestPlayerCard(Card excludeThis)
    {
        List<Card> availableCards = new(_ctx.PlayerCardManager.ActiveCards);
        availableCards.Remove(excludeThis);
        if (availableCards.Count == 0) return null;

        int min = 100;
        List<Card> lowestCards = new();

        foreach (Card card in availableCards)
        {
            if (card.points < min)
            {
                min = card.points;
                lowestCards.Clear();
                lowestCards.Add(card);
            }
            else if (card.points == min)
            {
                lowestCards.Add(card);
            }
        }

        int rand = Random.Range(0, lowestCards.Count);
        Card chosenCard = lowestCards[rand];

        return chosenCard;
    }

    #endregion

    #region Amount Providers
    public int GetAmount(Card parentCard, GetAmountStrategy amountStrategy)
    {
        int amount = 0;

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
        return _ctx.PlayerCardManager.ActiveCards.Count;
    }

    public int GetAmountOfEnemies()
    {
        return _ctx.EnemyCardManager.ActiveEnemies.Count;
    }

    public int GetRoomIndex()
    {
        return _ctx.FloorCtx.CurrentRoomIndex;
    }

    public int GetLowestEnemyCardValue(Card excludeThis)
    {
        int amount = 100;

        foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies)
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


