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

    public Card SpawnEnemyInIndexByBlueprint(int containerIndex, CardBlueprint cardBlueprint)
    {
        Card card = SpawnCard(cardBlueprint, GameConstants.ENEMY_CARD_LAYER, room.grid[containerIndex].transform);
        card.transform.localPosition = Vector3.zero;
        card.index = containerIndex;

        StartCoroutine(room.grid[containerIndex].SetSlotState(MapSlotState.Enemy));
        return card;
    }

    public Card SpawnEnemyInIndexByStrength(int containerIndex, int strength)
    {
        CardBlueprint cardBlueprint = DrawRandomEnemyByStrength(strength);

        Card card = SpawnCard(cardBlueprint, GameConstants.ENEMY_CARD_LAYER, room.grid[containerIndex].transform);
        card.transform.localPosition = Vector3.zero;
        card.index = containerIndex;

        StartCoroutine(room.grid[containerIndex].SetSlotState(MapSlotState.Enemy));
        return card;
    }

    public CardBlueprint DrawEnemyByArchetype(CardArchetype archetype)
    {
        return Codex.GetCardOverride(archetype);
    }


    private CardBlueprint DrawRandomEnemyByStrength(int strength)
    {
        CardColor randomColor = (CardColor)Random.Range(0, 2);
        return Codex.GetCardOverride(new CardArchetype(strength, randomColor));
    }

    private Card SpawnCard(CardBlueprint cardBlueprint, string sortingLayerName, Transform parent)
    {
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, parent);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(Codex, cardBlueprint, sortingLayerName, CardInteractionType.Playable);

        return card;
    }


}
