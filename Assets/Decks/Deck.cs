using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [FoldoutGroup("Dependencies")]
    [SerializeField] private AllEvents events;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private Transform mapMasterContainer;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private GameObject cardPrefab;

    [FoldoutGroup("Deck")]
    [SerializeField] protected BlueprintPool blueprintPool;

    [FoldoutGroup("Deck")]
    [SerializeField] protected bool shouldShuffleDeck;

    [FoldoutGroup("Deck")]
    CardOwner cardOwner;

    public List<CardArchetype> deck;

    private void Awake()
    {
        InitializeDeck();
    }
    protected CardBlueprint DrawCard()
    {
        if (deck.Count == 0)
        {
            throw new System.InvalidOperationException("No cards left in the deck.");
        }

        CardArchetype drawnArchetype = deck[0];
        deck.RemoveAt(0); // Remove the card from the deck
        CardBlueprint drawnBlueprint = blueprintPool.GetCardOverride(drawnArchetype);
        return drawnBlueprint;
    }


    protected Card SpawnCard(CardBlueprint cardBlueprint, int index, string sortingLayerName)
    {
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, mapMasterContainer);
        cardGO.transform.localScale = Vector3.one * GameConstants.MapScale;
        cardGO.name = cardBlueprint.name;

        Card card = cardGO.GetComponent<Card>();
        card.Init(blueprintPool, cardBlueprint, cardOwner, index, sortingLayerName);

        return card;
    }


    [Button]
    public void InitializeDeck()
    {
        deck = new();
        for (int i = 0; i < blueprintPool.maxDrawableBlack; i++)
        {
            deck.Add(new CardArchetype(i+1, CardColor.Black));
        }

        for (int i = 0; i <blueprintPool.maxDrawableRed;  i++)
        {
            deck.Add(new CardArchetype(i+1, CardColor.Red));
        }

        if (shouldShuffleDeck) Tools.ShuffleList(deck);
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
    public int points;
    public CardColor color;

    public CardArchetype(int points, CardColor color)
    {
        this.points = points;
        this.color = color;
    }
}
