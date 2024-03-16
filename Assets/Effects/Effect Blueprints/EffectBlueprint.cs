using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class EffectBlueprint
{
    public EffectType EffectType;

    public EffectTarget target;

    [ShowIf("@ShouldShowAmountOfTargets()")]
    public int amountOfTargets = 1;

    public bool shouldAnimate = true;

    public EffectApplicationType applicationType;

    [ShowIf("@ShouldShowAmountStrategy()")]
    public GetAmountStrategy amountStrategy;

    [ShowIf("@ShouldShowAmount()")]
    public int amount = 1;

    [ShowIf("@ShouldShowCardArchetype()")]
    public CardArchetype cardArchetype;

    #region Battle Modifier

    [ShowIf("EffectType", EffectType.AlterBattlePoints)]
    public BattlePointType battlePointType;

    [ShowIf("EffectType", EffectType.AlterBattlePoints)]
    public ModifierType modifierType;

    [ShowIf("EffectType", EffectType.AlterBattlePoints)]
    public bool filterBattleModifier;

    [ShowIf("EffectType", EffectType.AlterBattlePoints)]
    public BattleModifierFilterBlueprint battleModifierfilter;

    #endregion

    #region Set Color

    [ShowIf("EffectType", EffectType.SetColor)]
    public CardColor color;

    #endregion

    #region Guardians

    [ShowIf("EffectType", EffectType.AddGuardian)]
    public GuardianType guardianType;

    [ShowIf("EffectType", EffectType.AddGuardian)]
    public EffectApplicationType guardianApplicationType;

    #endregion

    #region Add Effect

    [ShowIf("EffectType", EffectType.AddEffect)]
    public EffectBlueprintAsset effectBlueprint;

    [ShowIf("EffectType", EffectType.AddEffect)]
    public EffectTrigger whenToTriggerAddedEffect;
    
    #endregion


    public Effect InstantiateEffect(EffectTrigger triggerType, Card parentCard, BattleRoomDataProvider data)
    {
        switch (EffectType)
        {
            case EffectType.DebugEffect:
                return new DebugEffect(this, data, triggerType, parentCard);
            case EffectType.AlterBattlePoints:
                return new AlterBattlePointsEffect(this, data, triggerType, parentCard, modifierType, battlePointType, battleModifierfilter);
            case EffectType.SetColor:
                return new SetColorEffect(this, data, triggerType, parentCard, color);
            case EffectType.ToggleColor:
                return new ToggleColorEffect(this, data, triggerType, parentCard);
            case EffectType.GainPoints:
                return new GainPointsEffect(this, data, triggerType, parentCard);
            case EffectType.DrawCard:
                return new DrawCardEffect(this, data, triggerType, parentCard);
            case EffectType.SpawnCardToHand:
                return new SpawnCardToHandEffect(this, data, triggerType, parentCard, cardArchetype);
            case EffectType.AddEffect:
                return new AddEffectEffect(this, data, triggerType, parentCard, effectBlueprint.blueprint, whenToTriggerAddedEffect);
            case EffectType.AddGuardian:
                return new AddGuardianEffect(this, data, triggerType, parentCard, guardianType, applicationType);
            case EffectType.SpawnEnemy:
                return new SpawnEnemiesEffect(this, data, triggerType, parentCard, cardArchetype);
            case EffectType.GainArmor:
                return new GainArmorEffect(this, data, triggerType, parentCard);
            case EffectType.TakeDamage:
                return new TakeDamageEffect(this, data, triggerType, parentCard);
            default:
                Debug.LogError("Effect wasn't found");
                return null;
        }
    }


    private bool ShouldShowAmount()
    {
        return (EffectType is
               EffectType.GainPoints
            or EffectType.SpawnCardToHand
            or EffectType.DrawCard
            or EffectType.AlterBattlePoints
            or EffectType.SpawnEnemy
            or EffectType.GainArmor
            or EffectType.TakeDamage)
            && amountStrategy is GetAmountStrategy.Value;
    }

    private bool ShouldShowCardArchetype()
    {
        return EffectType is EffectType.SpawnCardToHand or EffectType.SpawnEnemy;
    }
    private bool ShouldShowAmountStrategy()
    {
        return EffectType is
               EffectType.GainPoints
            or EffectType.SpawnCardToHand
            or EffectType.DrawCard
            or EffectType.AlterBattlePoints
            or EffectType.SpawnEnemy
            or EffectType.GainArmor
            or EffectType.TakeDamage;
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
    SelectedCards,
    HighestPlayerCard,
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
    SpawnEnemy,
    GainArmor,
    TakeDamage
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

