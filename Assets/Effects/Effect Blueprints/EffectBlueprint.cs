using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class EffectBlueprint
{
    public EffectTarget target;

    [ShowIf("@ShouldShowAmountOfTargets()")]
    public int amountOfTargets = 1;

    public float predelay = 0f;
    public float postdelay = 0f;

    public EffectApplicationType applicationType;
    public EffectType EffectType;

    [ShowIf("@ShouldShowAmountStrategy()")]
    public GetAmountStrategy amountStrategy;

    [ShowIf("@ShouldShowAmount()")]
    public int amount = 1;

    [ShowIf("EffectType", EffectType.AlterBattlePoints)]
    public BattlePointType battlePointType;

    [ShowIf("EffectType", EffectType.AlterBattlePoints)]
    public ModifierType modifierType;

    [ShowIf("EffectType", EffectType.SetColor)]
    public CardColor color;

    [ShowIf("EffectType", EffectType.SpawnCardToHand)]
    public CardArchetype cardArchetype;

    [ShowIf("EffectType", EffectType.AddGuardian)]
    public GuardianType guardianType;

    [ShowIf("EffectType", EffectType.AddGuardian)]
    public EffectApplicationType guardianApplicationType;

    [ShowIf("EffectType", EffectType.AddEffect)]
    public EffectBlueprintAsset effectBlueprint;

    [ShowIf("EffectType", EffectType.AddEffect)]
    public EffectTrigger whenToTriggerAddedEffect;

    public bool isConditional;

    [ShowIf("isConditional")]
    public Decision decision;
    public Effect InstantiateEffect(EffectTrigger triggerType, Card parentCard, BattleRoomDataProvider data)
    {
        if (triggerType.TriggerType == TriggerType.StartOfBattle && isConditional) Debug.LogError("Conditional effects need to be in before attacking, not start of battle");

        Effect effect = null;

        switch (EffectType)
        {
            case EffectType.DebugEffect:
                effect = new DebugEffect(this, data, triggerType, parentCard);
                break;
            case EffectType.AlterBattlePoints:
                effect = new AlterBattlePointsEffect(this, data, triggerType, parentCard, modifierType, battlePointType);
                break;
            case EffectType.SetColor:
                effect = new SetColorEffect(this, data, triggerType, parentCard, color);
                break;
            case EffectType.ToggleColor:
                effect = new ToggleColorEffect(this, data, triggerType, parentCard);
                break;
            case EffectType.GainPoints:
                effect = new GainPointsEffect(this, data, triggerType, parentCard);
                break;
            case EffectType.DrawCard:
                effect = new DrawCardEffect(this, data, triggerType, parentCard);
                break;
            case EffectType.SpawnCardToHand:
                effect = new SpawnCardToHandEffect(this, data, triggerType, parentCard, cardArchetype);
                break;
            case EffectType.AddEffect:
                effect = new AddEffectEffect(this, data, triggerType, parentCard, effectBlueprint.blueprint, whenToTriggerAddedEffect);
                break;
            case EffectType.AddGuardian:
                effect = new AddGuardianEffect(this, data, triggerType, parentCard, guardianType, applicationType);
                break;
        }

        return effect;
    }

    private bool ShouldShowAmount()
    {
        return (EffectType is
               EffectType.GainPoints
            or EffectType.SpawnCardToHand
            or EffectType.DrawCard
            or EffectType.AlterBattlePoints)
            && amountStrategy is GetAmountStrategy.Value;
    }

    private bool ShouldShowAmountStrategy()
    {
        return EffectType is
               EffectType.GainPoints
            or EffectType.SpawnCardToHand
            or EffectType.DrawCard
            or EffectType.AlterBattlePoints;
    }

    private bool ShouldShowDecision()
    {
        return isConditional;
    }

    private bool ShouldShowAmountOfTargets()
    {
        return target is EffectTarget.RandomCardOnMap or EffectTarget.RandomCardFromHand;
    }
}

public enum EffectApplicationType
{
    Base,
    Persistent,
    //Bond,
}

public enum EffectTarget
{
    InitiatingCard,
    Oppnent,
    PlayerCardBattling,
    EnemyCardBattling,
    AllPlayerCards,
    AllEnemyCards,
    AllCardsOnMap,
    AllCardsInHand,
    RandomCardOnMap,
    RandomCardFromHand,
    LowestPlayerCard,
}

public enum EffectTargetStrategy
{
    All,
    Random,
    Highest,
    Lowest
}

public enum EffectType
{
    DebugEffect,
    AlterBattlePoints,
    SetColor,
    ToggleColor,
    GainPoints,
    DrawCard,
    SpawnCardToHand,
    AddEffect,
    AddGuardian,
}

public enum GetAmountStrategy
{
    Value,
    EmptySpacesOnMap,
    EnemiesOnMap,
    NotImplementedDeadEnemiesOnMap,
    CardsInHand,
    RoomCount,
    LowestValueEnemyCard,
}

