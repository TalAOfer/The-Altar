using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Card")]
public class CardBlueprint : SerializedScriptableObject
{
    [PreviewField(150)]
    public Sprite cardSprite;
    public bool isDefault;

    public string cardName;
    public CardArchetype Archetype;
    public Affinity Affinity;
    public string Description;
    public SpecialEffects SpecialEffects;

    public Dictionary<EffectTrigger, List<EffectBlueprintReference>> Effects = new();
    public List<EffectBlueprintReference> GetEffectsForTrigger(EffectTrigger trigger)
    {
        if (Effects.TryGetValue(trigger, out List<EffectBlueprintReference> effectBlueprints))
        {
            return effectBlueprints;
        }
        return new List<EffectBlueprintReference>(); // Return an empty list if no effects are found for the trigger
    }
}

public enum CardColor
{
    Black, Red
}

public enum Affinity
{
    Player,
    Enemy,
}

[Flags]
public enum SpecialEffects
{
    HigherBeing = 1,
    Meditate = 2,
    Bloodthirst = 4,
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


