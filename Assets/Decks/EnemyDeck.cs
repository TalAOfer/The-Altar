using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeck : Deck
{
    [SerializeField] private MapGridArranger grid;
    [SerializeField] private EnemyManager enemyManager;

    public void DrawEnemyCardToMapIndex(Component sender, object data)
    {
        int index = (int)data;
        SpawnEnemyCard(index);
    }
    public void SpawnEnemyCard(int containerIndex)
    {
        Card card = SpawnCard(DrawCard(CardOwner.Enemy), containerIndex, GameConstants.TOP_MAP_LAYER);
        card.transform.position = grid.MapSlots[containerIndex].transform.position;
        enemyManager.AddEnemyToManager(card);
        //StartCoroutine(card.interactionHandler.MoveCardToPositionOverTime(grid.MapSlots[containerIndex].transform.position, 1f));
        StartCoroutine(grid.MapSlots[containerIndex].SetSlotState(MapSlotState.Occupied));
        card.interactionHandler.SetNewDefaultLocation(card.transform.position, card.transform.localScale, card.transform.eulerAngles);
    }
}
