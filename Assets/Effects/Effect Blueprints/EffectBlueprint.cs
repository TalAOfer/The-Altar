using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class EffectBlueprint
{
    [Title("Protoype")]
    public EffectPrototype prototype;
    [ShowIf("prototype", EffectPrototype.Select)]
    public EventRegistry events;

    [ShowIf("prototype", EffectPrototype.Normal)]
    public EffectTarget target;

    [ShowIf("@ShouldShowAmountOfTargets()")]
    public int amountOfTargets = 1;

    public float predelay = 0f;
    public float postdelay = 0f;

    [Title("Applier")]
    public EffectApplicationType applicationType;
    public ApplierType applierType;

    [ShowIf("@ShouldShowAmountStrategy()")]
    public GetAmountStrategy amountStrategy;

    [ShowIf("@ShouldShowAmount()")]
    public int amount = 1;

    [ShowIf("applierType", ApplierType.AlterBattlePoints)]
    public BattlePointType battlePointType;

    [ShowIf("applierType", ApplierType.AlterBattlePoints)]
    public ModifierType modifierType;

    [ShowIf("applierType", ApplierType.SetColor)]
    public CardColor color;

    [ShowIf("applierType", ApplierType.SpawnCardToHand)]
    public CardArchetype cardArchetype;

    [ShowIf("applierType", ApplierType.AddGuardian)]
    public GuardianType guardianType;

    [ShowIf("applierType", ApplierType.AddGuardian)]
    public EffectApplicationType guardianApplicationType;

    [ShowIf("applierType", ApplierType.AddEffect)]
    public EffectBlueprintReference effectBlueprint;

    [ShowIf("applierType", ApplierType.AddEffect)]
    public EffectTrigger whenToTriggerAddedEffect;

    public bool isConditional;

    [ShowIf("isConditional")]
    public Decision decision;
    public void InstantiateEffect(EffectTrigger triggerType, Card parentCard, BattleRoomDataProvider data)
    {
        GameObject newEffectGO = new(triggerType.name + " : " + applierType.ToString());
        newEffectGO.transform.SetParent(parentCard.transform, false);
        newEffectGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (triggerType.TriggerType == TriggerType.StartOfBattle && isConditional) Debug.LogError("Conditional effects need to be in before attacking, not start of battle");

        EffectApplier applier = null;

        switch (applierType)
        {
            case ApplierType.DebugEffect:
                applier = newEffectGO.AddComponent<DebugApplier>();
                break;
            case ApplierType.AlterBattlePoints:
                applier = newEffectGO.AddComponent<AlterBattlePointsApplier>();
                AlterBattlePointsApplier battlePointsApplier = (AlterBattlePointsApplier)applier;
                battlePointsApplier.Initialize(modifierType, battlePointType);
                break;
            case ApplierType.SetColor:
                applier = newEffectGO.AddComponent<SetColorApplier>();
                SetColorApplier setColorApplier = (SetColorApplier)applier;
                setColorApplier.Initialize(color);
                break;
            case ApplierType.ToggleColor:
                applier = newEffectGO.AddComponent<ToggleColorApplier>();
                break;
            case ApplierType.GainPoints:
                applier = newEffectGO.AddComponent<GainPointsApplier>();
                break;
            case ApplierType.DrawCard:
                applier = newEffectGO.AddComponent<DrawCardApplier>();
                break;
            case ApplierType.SpawnCardToHand:
                applier = newEffectGO.AddComponent<SpawnCardToHandApplier>();
                SpawnCardToHandApplier spawnToHandApplier = (SpawnCardToHandApplier)applier;
                spawnToHandApplier.Initialize(cardArchetype);
                break;
            case ApplierType.AddEffect:
                applier = newEffectGO.AddComponent<AddEffectApplier>();
                AddEffectApplier addEffectApplier = (AddEffectApplier)applier;
                addEffectApplier.Initialize(effectBlueprint, whenToTriggerAddedEffect);
                break;
            case ApplierType.AddGuardian:
                applier = newEffectGO.AddComponent<AddGuardianApplier>();
                AddGuardianApplier addGuardianApplier = (AddGuardianApplier)applier;
                addGuardianApplier.Initialize(guardianType, guardianApplicationType);
                break;
        }

        switch (prototype)
        {
            case EffectPrototype.Normal:
                BaseInitializeEffect<Effect>(newEffectGO, applier, triggerType, parentCard, data);
                break;
            case EffectPrototype.Select:
                BaseInitializeEffect<SelectEffect>(newEffectGO, applier, triggerType, parentCard, data);
                break;
        }
    }

    public T BaseInitializeEffect<T>(GameObject newEffectGO, EffectApplier applier, EffectTrigger triggerType, Card parentCard, BattleRoomDataProvider data) where T : Effect
    {
        T effect = newEffectGO.AddComponent<T>();
        effect.BaseInitialize(data, applier, parentCard, this); // Assuming BaseInitialize is a method in T or its base class
        applier.BaseInitialize(data, parentCard, triggerType);

        parentCard.effects.AddEffectToDictionary(triggerType, effect);

        return effect;
    }

    private bool ShouldShowAmount()
    {
        return (applierType is
               ApplierType.GainPoints
            or ApplierType.SpawnCardToHand
            or ApplierType.DrawCard
            or ApplierType.AlterBattlePoints)
            && amountStrategy is GetAmountStrategy.Value;
    }

    private bool ShouldShowAmountStrategy()
    {
        return applierType is
               ApplierType.GainPoints
            or ApplierType.SpawnCardToHand
            or ApplierType.DrawCard
            or ApplierType.AlterBattlePoints;
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

public enum EffectPrototype
{
    Normal,
    Select
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

public enum ApplierType
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

