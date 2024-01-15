using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    public Transform spawnContainer;

    [SerializeField] private RunData runData;
    private BlueprintPoolInstance codex => runData.playerCodex;
    private DeckInstance deck => runData.playerDeck;

    public CardBlueprint DrawCard()
    {
        if (deck.cards.Count == 0)
        {
            deck.Reinitialize(true);
        }

        CardArchetype drawnArchetype = deck.cards[0];
        deck.cards.RemoveAt(0); // Remove the card from the deck
        CardBlueprint drawnBlueprint = codex.GetCardOverride(drawnArchetype);
        return drawnBlueprint;
    }

    public Card SpawnCard(CardBlueprint cardBlueprint, string sortingLayerName)
    {
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, spawnContainer);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(codex, cardBlueprint, sortingLayerName);

        return card;
    }
}
