using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Codex
{
    public CodexBlueprint defaultCodex;
    public List<CardBlueprint> black = new();
    public List<CardBlueprint> red = new();

    public Codex (CodexBlueprint codexBlueprint)
    {
        defaultCodex = codexBlueprint;
        black?.Clear();
        red?.Clear();
        black = new(codexBlueprint.black);
        red = new(codexBlueprint.red);
    }

    public CardBlueprint GetCardOverride(CardArchetype archetype)
    {
        List<CardBlueprint> targetDeck = archetype.color is CardColor.Black ? black : red;
        return targetDeck[archetype.points];
    }

    public void PurgeOverrideToDefault(CardArchetype archetype)
    {
        List<CardBlueprint> targetDeck = archetype.color is CardColor.Black ? black : red;
        targetDeck[archetype.points] = defaultCodex.GetBlueprintByArchetype(archetype);
    }

    public void OverrideCard(CardBlueprint cardBlueprint)
    {
        List<CardBlueprint> targetDeck = cardBlueprint.Archetype.color is CardColor.Black ? black : red;
        int index = cardBlueprint.Archetype.points;
        targetDeck.Insert(index, cardBlueprint);
        targetDeck.RemoveAt(index + 1);
    }
}
