using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[Serializable]
public class BlueprintPoolInstance
{
    public List<CardBlueprint> black = new();
    public List<CardBlueprint> red = new();

    public void InitializeAsPool(BlueprintPoolInstance pool)
    {
        black?.Clear();
        red?.Clear();
        black = new(pool.black);
        red = new(pool.red);
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
        List<CardBlueprint> targetDeck = cardBlueprint.archetype.color is CardColor.Black ? black : red;
        int index = cardBlueprint.archetype.points;
        targetDeck.Insert(index, cardBlueprint);
        targetDeck.RemoveAt(index + 1);
    }
}
