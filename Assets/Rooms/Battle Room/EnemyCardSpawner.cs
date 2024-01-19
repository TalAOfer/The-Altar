using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private RunData runData;
    [SerializeField] private BattleRoom room;
    private BlueprintPoolInstance codex => runData.enemyCodex;

    public Card SpawnEnemyInIndexByBlueprint(int containerIndex, CardBlueprint cardBlueprint)
    {
        Card card = SpawnCard(cardBlueprint, GameConstants.TOP_MAP_LAYER, room.grid[containerIndex].transform);
        card.transform.localPosition = Vector3.zero;
        card.index = containerIndex;

        StartCoroutine(room.grid[containerIndex].SetSlotState(MapSlotState.Occupied));
        card.interactionHandler.SetNewDefaultLocation(card.transform.position, card.transform.localScale, card.transform.eulerAngles);
        return card;
    }

    public Card SpawnEnemyInIndexByStrength(int containerIndex, int strength)
    {
        CardBlueprint cardBlueprint = DrawRandomEnemyByStrength(strength);

        Card card = SpawnCard(cardBlueprint, GameConstants.TOP_MAP_LAYER, room.grid[containerIndex].transform);
        card.transform.localPosition = Vector3.zero;
        card.index = containerIndex;

        StartCoroutine(room.grid[containerIndex].SetSlotState(MapSlotState.Occupied));
        card.interactionHandler.SetNewDefaultLocation(card.transform.position, card.transform.localScale, card.transform.eulerAngles);
        return card;
    }

    private CardBlueprint DrawEnemyByArchetype(CardArchetype archetype)
    {
        return codex.GetCardOverride(archetype);
    }


    private CardBlueprint DrawRandomEnemyByStrength(int strength)
    {
        CardColor randomColor = (CardColor)Random.Range(0, 2);
        return codex.GetCardOverride(new CardArchetype(strength, randomColor));
    }

    private Card SpawnCard(CardBlueprint cardBlueprint, string sortingLayerName, Transform parent)
    {
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, parent);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(codex, cardBlueprint, sortingLayerName);

        return card;
    }


}