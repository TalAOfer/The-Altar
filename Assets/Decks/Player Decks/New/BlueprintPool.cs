using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprint Pool")]
public class BlueprintPool : ScriptableObject
{
    public List<CardBlueprint> black;
    public int maxDrawableBlack;
    public List<CardBlueprint> red;
    public int maxDrawableRed;

    public void Initialize(DeckBlueprint defaultBlackDeck, DeckBlueprint defaultRedDeck)
    {
        black.Clear();
        red.Clear();

        black = new List<CardBlueprint>(defaultBlackDeck.cards);
        red = new List<CardBlueprint>(defaultRedDeck.cards);
    }

    public CardBlueprint GetCardOverride(CardArchetype archetype)
    {
        List<CardBlueprint> targetDeck = archetype.color is CardColor.Black ? black : red;
        return targetDeck[archetype.number];
    }

    public void OverrideCard(CardBlueprint cardBlueprint)
    {
        List<CardBlueprint> targetDeck = cardBlueprint.cardColor is CardColor.Black ? black : red;
        int index = cardBlueprint.defaultPoints;
        targetDeck.Insert(index, cardBlueprint); 
        targetDeck.RemoveAt(index + 1);
    }

    public void IncreaseMaximumDrawability(CardColor cardColor, int amount)
    {
        if (cardColor is CardColor.Black)
        {
            maxDrawableBlack += amount;
            maxDrawableBlack = Mathf.Min(10, maxDrawableBlack);
        } else
        {
            maxDrawableRed += amount;
            maxDrawableRed = Mathf.Min(10, maxDrawableRed);
        }
    }
}
