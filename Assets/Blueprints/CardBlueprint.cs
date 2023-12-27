using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Card")]
public class CardBlueprint : ScriptableObject
{
    [PreviewField(150)]
    public Sprite cardSprite;

    public Symbol symbol;
    public string cardName;
    public CardColor cardColor;
    public int defaultPoints;

    [Title("Effects")]
    public List<EffectBlueprint> OnReveal;
    public List<EffectBlueprint> BeforeBattle;
    public List<EffectBlueprint> OnGainPoints;
    public List<EffectBlueprint> OnDeath;
    public List<EffectBlueprint> OnSacrifice;
    public List<EffectBlueprint> OnActionTaken;
}

public enum CardColor
{
    Black, Red
}

public enum Symbol
{
    Spades, Clubs, Hearts, Diamonds 
}

