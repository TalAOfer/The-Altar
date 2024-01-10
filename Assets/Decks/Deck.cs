using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private AllEvents events;
    
    [SerializeField] private Transform mapMasterContainer;
    [SerializeField] private GameObject cardPrefab;

    [SerializeField] protected bool shouldShuffleDeck;
    [SerializeField] protected DeckBlueprint deckBlueprint;
    protected List<CardBlueprint> deck;
    private void Awake()
    {
        deck = new List<CardBlueprint>(deckBlueprint.cards);
        if (shouldShuffleDeck) Tools.ShuffleList(deck);
    }
    protected CardBlueprint DrawCard(CardOwner cardOwner)
    {
        if (deck.Count == 0)
        {
            throw new System.InvalidOperationException("No cards left in the deck.");
        }

        CardBlueprint drawnCard = deck[0];
        deck.RemoveAt(0); // Remove the card from the deck
        return drawnCard;
    }

    protected Card SpawnCard(CardBlueprint cardBlueprintDrawn, int index, string sortingLayerName)
    {
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, mapMasterContainer);
        cardGO.transform.localScale = Vector3.one * GameConstants.MapScale;
        cardGO.name = cardBlueprintDrawn.name;

        Card card = cardGO.GetComponent<Card>();
        card.Init(cardBlueprintDrawn, index, sortingLayerName);

        return card;
    }

}
