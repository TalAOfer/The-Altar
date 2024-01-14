using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeck : Deck
{
    [SerializeField] private MapGridArranger grid;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private BattleRoom levelManager;

    public void DrawEnemyCardToMapIndex(Component sender, object data)
    {
        int index = (int)data;
        SpawnEnemyCard(index);
    }

    public Card SpawnEnemyInIndexByStrength(int containerIndex, int strength)
    {
        CardColor randomColor = (CardColor) Random.Range(0, 2);
        CardBlueprint cardBlueprint = blueprintPoolInstance.GetCardOverride(new CardArchetype(strength, randomColor));

        Card card = SpawnCard(cardBlueprint, containerIndex, GameConstants.TOP_MAP_LAYER);
        card.transform.parent = grid.MapSlots[containerIndex].transform;
        card.transform.localPosition = Vector3.zero;

        StartCoroutine(grid.MapSlots[containerIndex].SetSlotState(MapSlotState.Occupied));
        card.interactionHandler.SetNewDefaultLocation(card.transform.position, card.transform.localScale, card.transform.eulerAngles);
        return card;
    }

    public void SpawnEnemyCard(int containerIndex)
    {
        Card card = SpawnCard(DrawCard(), containerIndex, GameConstants.TOP_MAP_LAYER);
        card.transform.position = grid.MapSlots[containerIndex].transform.position;
        enemyManager.AddEnemyToManager(card);
        //StartCoroutine(card.interactionHandler.MoveCardToPositionOverTime(grid.MapSlots[containerIndex].transform.position, 1f));
        StartCoroutine(grid.MapSlots[containerIndex].SetSlotState(MapSlotState.Occupied));
        card.interactionHandler.SetNewDefaultLocation(card.transform.position, card.transform.localScale, card.transform.eulerAngles);
    }
}
