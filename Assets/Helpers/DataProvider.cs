using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataProvider : MonoBehaviour
{
    private RunData runData;
    private RoomData roomData;
    private FloorData floorData;

    public Card BattlingEnemyCard;
    public Card BattlingPlayerCard;
    private void Awake()
    {
        runData = Locator.RunData;
        roomData = Locator.RoomData;
        floorData = Locator.FloorData;
    }

    #region Target Providers

    public Card GetOpponent(Card card)
    {
        if (roomData.BattleRoomState != BattleRoomState.Battle) Debug.Log("When not in battle this will return null");
        return card.cardOwner == CardOwner.Player ? BattlingEnemyCard : BattlingPlayerCard;
    }

    public List<Card> GetAllActiveEnemies()
    {
        return new(roomData.EnemyManager.activeEnemies);
    }

    public List<Card> GetAllActiveEnemiesOnMap()
    {
        if (roomData.BattleRoomState != BattleRoomState.Battle) Debug.Log("When not in battle, use GetAllActiveEnemies() instead");

        List<Card> activeEnemies = new(roomData.EnemyManager.activeEnemies);
        if (BattlingEnemyCard != null) activeEnemies.Remove(BattlingEnemyCard);
        return activeEnemies;
    }

    public List<Card> GetAllActivePlayerCards()
    {
        return new(runData.playerManager.ActiveCards);
    }

    public List<Card> GetAllCardsInHand()
    {
        if (roomData.BattleRoomState != BattleRoomState.Battle) Debug.Log("When not in battle, use GetAllActivePlayerCards() instead");

        List<Card> activeEnemies = new(runData.playerManager.ActiveCards);
        if (BattlingPlayerCard != null) activeEnemies.Remove(BattlingPlayerCard);
        return activeEnemies;
    }

    public List<Card> GetRandomPlayerCards(int numberOfCards, Card excludeThis)
    {
        List<Card> cardsToPickFrom = GetAllActivePlayerCards();

        if (excludeThis != null)
        {
            cardsToPickFrom.RemoveAll(card => card.Equals(excludeThis));
        }

        cardsToPickFrom.Remove(BattlingPlayerCard);

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

        cardsToPickFrom.Remove(BattlingEnemyCard);

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
        List<Card> availableCards = new(runData.playerManager.ActiveCards);
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

    public int GetAmountOfPlayerCards()
    {
        return runData.playerManager.ActiveCards.Count;
    }

    public int GetEmptySpacesAmount()
    {
        int amount = 0;
        foreach (MapSlot slot in roomData.EnemyManager.grid)
        {
            if (slot.slotState == MapSlotState.Free) amount++;
        }

        return amount;
    }

    public int GetAmountOfEnemies()
    {
        return roomData.EnemyManager.activeEnemies.Count;
    }

    public int GetRoomIndex()
    {
        return floorData.currentRoomIndex;
    }

    public int GetLowestEnemyCardValue(Card excludeThis)
    {
        int amount = 100;

        foreach (Card card in roomData.EnemyManager.activeEnemies)
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


