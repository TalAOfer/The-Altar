using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    public Transform spawnContainer;
    
    public BlueprintPoolInstance blueprintPoolInstance;
    public DeckInstance deck;

    private CardBlueprint DrawCard()
    {
        if (deck.cards.Count == 0)
        {
            deck.Reinitialize();
        }

        CardArchetype drawnArchetype = deck.cards[0];
        deck.cards.RemoveAt(0); // Remove the card from the deck
        CardBlueprint drawnBlueprint = blueprintPoolInstance.GetCardOverride(drawnArchetype);
        return drawnBlueprint;
    }

    private Card SpawnCard(CardBlueprint cardBlueprint, string sortingLayerName)
    {
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, spawnContainer);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(blueprintPoolInstance, cardBlueprint, sortingLayerName);

        return card;
    }
}
