using System;
using System.Collections.Generic;

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

    public Codex (CodexBlueprint defaultCodexBlueprint, CodexBlueprint fullCodexBlueprint, List<CardArchetype> progressionMap)
    {
        black?.Clear();
        red?.Clear();
        black = new(defaultCodexBlueprint.black);
        red = new(defaultCodexBlueprint.red);

        foreach (CardArchetype archetype in progressionMap)
        {
            CardBlueprint cardBlueprint = fullCodexBlueprint.GetBlueprintByArchetype(archetype);
            OverrideCard(cardBlueprint);
        }
    }

    public CardBlueprint GetCardOverride(CardArchetype archetype)
    {
        List<CardBlueprint> targetDeck = archetype.color is CardColor.Black ? black : red;
        return targetDeck[archetype.points - 1];
    }

    public void PurgeOverrideToDefault(CardArchetype archetype)
    {
        List<CardBlueprint> targetDeck = archetype.color is CardColor.Black ? black : red;
        targetDeck[archetype.points - 1] = defaultCodex.GetBlueprintByArchetype(archetype);
    }

    public void OverrideCard(CardBlueprint cardBlueprint)
    {
        List<CardBlueprint> targetDeck = cardBlueprint.Archetype.color is CardColor.Black ? black : red;
        int index = cardBlueprint.Archetype.points;
        targetDeck[index - 1] = cardBlueprint;
        //targetDeck.Insert(index, cardBlueprint);
        //targetDeck.RemoveAt(index + 1);
    }
}
