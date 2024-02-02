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
    public CardColor cardColor;
    public CardOwner cardOwner;
    public int defaultPoints;
    public string description;
    public SpecialEffects specialEffects;

    [Title("Effects")]
    public List<EffectBlueprint> StartOfTurn;
    
    public List<EffectBlueprint> StartOfBattle;
    public List<EffectBlueprint> Support;
    public List<EffectBlueprint> BeforeAttacking;
    public List<EffectBlueprint> OnDeath;
    public List<EffectBlueprint> OnGlobalDeath;
    public List<EffectBlueprint> OnSurvive;

    public List<EffectBlueprint> Bloodthirst;
    public List<EffectBlueprint> Meditate;
}

public enum CardColor
{
    Black, Red
}

public enum CardOwner
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


