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
    public bool higherBeing;

    [Title("Effects")]
    public List<EffectBlueprint> StartOfTurn;
    public List<EffectBlueprint> OnObtain;
    public List<EffectBlueprint> OnGainPoints;
    public List<EffectBlueprint> OnSacrifice;

    public List<EffectBlueprint> StartOfBattle;
    public List<EffectBlueprint> BeforeAttacking;
    public List<EffectBlueprint> Support;
    public List<EffectBlueprint> OnSurvive;
    public List<EffectBlueprint> OnDeath;
    public List<EffectBlueprint> OnGlobalDeath;
    public List<EffectBlueprint> OnActionTaken;
    public List<EffectBlueprint> EndOfTurn;
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


