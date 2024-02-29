using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Blueprints/Codex")] 
public class CodexBlueprint : ScriptableObject
{
    public List<CardBlueprint> black;
    public List<CardBlueprint> red;

    public CardBlueprint GetBlueprintByArchetype(CardArchetype archetype)
    {
        List<CardBlueprint> targetDeck = archetype.color is CardColor.Black ? black : red;
        return targetDeck[archetype.points - 1];
    }
}
