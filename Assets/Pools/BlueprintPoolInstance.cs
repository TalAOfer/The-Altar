using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlueprintPoolInstance
{
    public List<CardBlueprint> black;
    public List<CardBlueprint> red;

    public void Initialize(BlueprintPoolBlueprint poolBlueprint)
    {
        black?.Clear();
        red?.Clear();
        black = new(poolBlueprint.black);
        red = new(poolBlueprint.red);
    }

    public void ShuffleLists()
    {
        Tools.ShuffleList(black);
        Tools.ShuffleList(red);
    }

    public CardBlueprint GetCardOverride(CardArchetype archetype)
    {
        List<CardBlueprint> targetDeck = archetype.color is CardColor.Black ? black : red;
        return targetDeck[archetype.points];
    }

    public void OverrideCard(CardBlueprint cardBlueprint)
    {
        List<CardBlueprint> targetDeck = cardBlueprint.cardColor is CardColor.Black ? black : red;
        int index = cardBlueprint.defaultPoints;
        targetDeck.Insert(index, cardBlueprint);
        targetDeck.RemoveAt(index + 1);
    }
}
