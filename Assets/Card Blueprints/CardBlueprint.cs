using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Card")]
public class CardBlueprint : ScriptableObject
{
    [PreviewField(150)]
    public Sprite cardSprite;

    public string cardName;
    public CardColor cardColor;
    public int defaultPoints;
    public string description;

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

