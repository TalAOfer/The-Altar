using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private BattleRoom room;
    
    private FloorData floorData;
    private Codex Codex => floorData.enemyCodex;

    private void Awake()
    {
        floorData = Locator.FloorData;
    }
    private Card SpawnCard(CardBlueprint cardBlueprint, string sortingLayerName, Transform parent)
    {
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, parent);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(Codex, cardBlueprint, sortingLayerName, CardInteractionType.Playable);

        return card;
    }

    public Card SpawnEnemyInIndexByBlueprint(int containerIndex, CardBlueprint cardBlueprint)
    {
        Card card = SpawnCard(cardBlueprint, GameConstants.ENEMY_CARD_LAYER, room.grid[containerIndex].transform);
        card.transform.localPosition = Vector3.zero;
        card.index = containerIndex;

        StartCoroutine(room.grid[containerIndex].SetSlotState(MapSlotState.Enemy));
        return card;
    }
}
