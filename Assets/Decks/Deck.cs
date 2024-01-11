using Sirenix.OdinInspector;
using System;
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
    public List<CardArchetype> newDeck;
    [SerializeField] BlueprintPool blueprintPool;

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

    protected CardBlueprint DrawNewCard()
    {
        if (deck.Count == 0)
        {
            throw new System.InvalidOperationException("No cards left in the deck.");
        }

        CardArchetype drawnArchetype = newDeck[0];
        newDeck.RemoveAt(0); // Remove the card from the deck
        CardBlueprint drawnBlueprint = blueprintPool.GetCardOverride(drawnArchetype);
        return drawnBlueprint;
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


    [Button]
    public void InitializeDeck()
    {
        newDeck = new();
        for (int i = 0; i < blueprintPool.maxDrawableBlack; i++)
        {
            newDeck.Add(new CardArchetype(i+1, CardColor.Black));
        }

        for (int i = 0; i <blueprintPool.maxDrawableRed;  i++)
        {
            newDeck.Add(new CardArchetype(i+1, CardColor.Red));
        }

        if (shouldShuffleDeck) Tools.ShuffleList(newDeck);
    }

    [Button]
    public void AddToBlueprintPool(CardBlueprint blueprint)
    {
        blueprintPool.OverrideCard(blueprint);
    }

}

[Serializable]
public class CardArchetype
{
    public int number;
    public CardColor color;

    public CardArchetype(int number, CardColor color)
    {
        this.number = number;
        this.color = color;
    }
}
