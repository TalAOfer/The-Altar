using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [FoldoutGroup("Dependencies")]
    [SerializeField] private Transform mapMasterContainer;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private GameObject cardPrefab;
    
    [FoldoutGroup("Deck")]
    public BlueprintPoolInstance blueprintPoolInstance;
    [FoldoutGroup("Deck")]
    [SerializeField] protected bool shouldShuffleDeck;

    [FoldoutGroup("Deck")]
    [SerializeField] private CardOwner cardOwner;

    public List<CardArchetype> deck;

    protected CardBlueprint DrawCard()
    {
        if (deck.Count == 0)
        {
            throw new System.InvalidOperationException("No cards left in the deck.");
        }

        CardArchetype drawnArchetype = deck[0];
        deck.RemoveAt(0); // Remove the card from the deck
        CardBlueprint drawnBlueprint = blueprintPoolInstance.GetCardOverride(drawnArchetype);
        return drawnBlueprint;
    }


    protected Card SpawnCard(CardBlueprint cardBlueprint, int index, string sortingLayerName)
    {
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, mapMasterContainer);
        cardGO.transform.localScale = Vector3.one * GameConstants.MapScale;
        cardGO.name = cardBlueprint.name;

        Card card = cardGO.GetComponent<Card>();
        //card.Init(blueprintPoolInstance, cardBlueprint, index, sortingLayerName);

        return card;
    }

}
