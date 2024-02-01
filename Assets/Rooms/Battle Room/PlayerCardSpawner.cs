using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    public Transform spawnContainer;
    public Transform spawnPos;

    [SerializeField] private RunData runData;
    public BlueprintPoolInstance Codex => runData.playerCodex;
    private DeckInstance Deck => runData.playerDeck;

    public CardBlueprint DrawCard()
    {
        if (Deck.cards.Count == 0)
        {
            Deck.Reinitialize(true);
        }

        CardArchetype drawnArchetype = Deck.cards[0];
        Deck.cards.RemoveAt(0); // Remove the card from the deck
        CardBlueprint drawnBlueprint = Codex.GetCardOverride(drawnArchetype);
        return drawnBlueprint;
    }

    public Card SpawnCard(CardBlueprint cardBlueprint, string sortingLayerName)
    {
        GameObject cardGO = Instantiate(cardPrefab, spawnPos.position, Quaternion.identity, spawnContainer);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(Codex, cardBlueprint, sortingLayerName, CardInteractionType.Playable);

        return card;
    }
}
