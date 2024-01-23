using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
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

    public Card GetRandomPlayerCard(Card excludeThis)
    {
        List<Card> cardsToPickFrom = GetAllActivePlayerCards();

        // Ensure the excludeThis card is properly removed
        if (excludeThis != null)
        {
            cardsToPickFrom.RemoveAll(card => card.Equals(excludeThis));
        }

        cardsToPickFrom.Remove(BattlingPlayerCard);

        // Add a safety check in case all cards are removed or list is empty
        if (cardsToPickFrom.Count == 0)
        {
            return null; // Or handle this scenario appropriately
        }

        int rand = Random.Range(0, cardsToPickFrom.Count);
        return cardsToPickFrom[rand];
    }
    public Card GetRandomEnemyCard(Card excludeThis)
    {
        List<Card> cardsToPickFrom = GetAllActiveEnemies();

        // Ensure the excludeThis card is properly removed
        if (excludeThis != null)
        {
            cardsToPickFrom.RemoveAll(card => card.Equals(excludeThis));
        }

        cardsToPickFrom.Remove(BattlingEnemyCard);

        // Add a safety check in case all cards are removed or list is empty
        if (cardsToPickFrom.Count == 0)
        {
            return null; // Or handle this scenario appropriately
        }

        int rand = Random.Range(0, cardsToPickFrom.Count);
        return cardsToPickFrom[rand];
    }

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
