using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Card")]
public class CardBlueprint : ScriptableObject
{
    [PreviewField(150)]
    public Sprite cardSprite;
    public bool isDefault;

    public string cardName;
    public CardArchetype Archetype;
    public Affinity Affinity;
    [TextArea()]
    public string Description;
    public SpecialEffects SpecialEffects;

    public List<EffectBlueprintReference> Effects;
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

//[Serializable]
//public class CardEffectBlueprint
//{
//    [BoxGroup("Trigger")]
//    public EffectTrigger Trigger;
//    //[BoxGroup("Trigger")]
//    //[ShowIf("@Trigger.IsModifiable")]
//    //public AmountEventFilterBlueprint Filter;
//    [BoxGroup("Effect")]
//    public EffectBlueprintReference Effect;

//    //private bool ShouldShowFilter => Trigger.IsModifiable;
//}

