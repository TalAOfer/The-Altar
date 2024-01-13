using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    public Transform spawnContainer;
    public MapGridArranger grid;

    public BlueprintPoolInstance blueprintPoolInstance;

    public Card SpawnEnemyInIndexByStrength(int containerIndex, int strength)
    {
        CardBlueprint cardBlueprint = DrawRandomEnemyByStrength(strength);

        Card card = SpawnCard(cardBlueprint, GameConstants.TOP_MAP_LAYER, grid.MapSlots[containerIndex].transform);
        card.transform.localPosition = Vector3.zero;

        StartCoroutine(grid.MapSlots[containerIndex].SetSlotState(MapSlotState.Occupied));
        card.interactionHandler.SetNewDefaultLocation(card.transform.position, card.transform.localScale, card.transform.eulerAngles);
        return card;
    }



    private CardBlueprint DrawEnemyByArchetype(CardArchetype archetype)
    {
        return blueprintPoolInstance.GetCardOverride(archetype);
    }


    private CardBlueprint DrawRandomEnemyByStrength(int strength)
    {
        CardColor randomColor = (CardColor)Random.Range(0, 2);
        return blueprintPoolInstance.GetCardOverride(new CardArchetype(strength, randomColor));
    }

    private Card SpawnCard(CardBlueprint cardBlueprint, string sortingLayerName, Transform parent)
    {
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, parent);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(blueprintPoolInstance, cardBlueprint, sortingLayerName);

        return card;
    }


}
