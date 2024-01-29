using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[CreateAssetMenu(menuName = "Room Data")]
public class RoomData : ScriptableObject
{
    public RoomType RoomType;

    public FloorManager floorManager;

    [ShowIf("RoomType", RoomType.Battle)]
    public BattleRoomState BattleRoomState;
    [ShowIf("RoomType", RoomType.Battle)]
    public PlayerManager PlayerManager;
    [ShowIf("RoomType", RoomType.Battle)]
    public BattleRoom EnemyManager;
    [ShowIf("RoomType", RoomType.Battle)]
    public Card BattlingEnemyCard;
    [ShowIf("RoomType", RoomType.Battle)]
    public Card BattlingPlayerCard;

    #region Target Providers

    public Card GetOpponent(Card card)
    {
        if (BattleRoomState != BattleRoomState.Battle) Debug.Log("When not in battle this will return null");
        return card.cardOwner == CardOwner.Player ? BattlingEnemyCard : BattlingPlayerCard;
    }

    public List<Card> GetAllActiveEnemies()
    {
        return new(EnemyManager.activeEnemies);
    }

    public List<Card> GetAllActiveEnemiesOnMap()
    {
        if (BattleRoomState != BattleRoomState.Battle) Debug.Log("When not in battle, use GetAllActiveEnemies() instead");

        List<Card> activeEnemies = new(EnemyManager.activeEnemies);
        if (BattlingEnemyCard != null) activeEnemies.Remove(BattlingEnemyCard);
        return activeEnemies;
    }

    public List<Card> GetAllActivePlayerCards()
    {
        return new(PlayerManager.activeCards);
    }

    public List<Card> GetAllCardsInHand()
    {
        if (BattleRoomState != BattleRoomState.Battle) Debug.Log("When not in battle, use GetAllActivePlayerCards() instead");

        List<Card> activeEnemies = new(PlayerManager.activeCards);
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

    #endregion

    #region Amount Providers
    public int GetEmptySpacesAmount()
    {
        int amount = 0;
        foreach(MapSlot slot in EnemyManager.grid)
        {
            if (slot.slotState != MapSlotState.Occupied) amount++;
        }

        return amount;
    }

    public int GetAmountOfEnemies()
    {
        return EnemyManager.activeEnemies.Count;
    }

    public int GetRoomIndex()
    {
        return floorManager.currentRoomIndex;
    }

    public int GetLowestEnemyCardValue()
    {
        int amount = 100;

        foreach(Card card in EnemyManager.activeEnemies) 
        {
            if (card.points < amount)
            {
                amount = card.points;
            }
        }

        return amount;
    }

    #endregion
}

public enum BattleRoomState
{
    Setup,
    Idle,
    BattleFormation,
    Battle
}

public enum SelectionRoomState
{

}
