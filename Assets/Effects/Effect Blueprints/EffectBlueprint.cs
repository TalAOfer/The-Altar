using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Blueprints/Effect")]
public class EffectBlueprint : ScriptableObject
{
    public RoomData data;

    [Title("Protoype")]
    public EffectPrototype prototype;
    [ShowIf("prototype", EffectPrototype.Select)]
    public AllEvents events;

    [ShowIf("prototype", EffectPrototype.Normal)]
    public EffectTarget target;

    [ShowIf("target", EffectTarget.RandomCardFromHand)]
    public int amountOfTargets;

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
    public EffectBlueprint effectBlueprint;

    [ShowIf("applierType", ApplierType.AddEffect)]
    public EffectTrigger whenToTriggerAddedEffect;

    public bool isConditional;

    [ShowIf("isConditional")]
    public Decision decision;
    public void SpawnEffect(EffectTrigger triggerType, Card parentCard)
    {
        GameObject newEffectGO = new GameObject(triggerType.ToString() + " : " + applierType.ToString());
        newEffectGO.transform.SetParent(parentCard.transform, false);
        newEffectGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (triggerType is EffectTrigger.StartOfBattle && isConditional) Debug.LogError("Conditional effects need to be in before attacking, not start of battle");

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
                BaseInitializeEffect<Effect>(newEffectGO, applier, triggerType, parentCard);
                break;
            case EffectPrototype.Select:
                BaseInitializeEffect<SelectEffect>(newEffectGO, applier, triggerType, parentCard);
                break;
        }
    }

    public T BaseInitializeEffect<T>(GameObject newEffectGO, EffectApplier applier, EffectTrigger triggerType, Card parentCard) where T : Effect
    {
        T effect = newEffectGO.AddComponent<T>();
        effect.BaseInitialize(applier, parentCard, this); // Assuming BaseInitialize is a method in T or its base class
        applier.BaseInitialize(parentCard, data, isConditional, decision, amountStrategy, amount);

        AddEffectToList(parentCard, triggerType, effect);

        return effect;
    }

    public void AddEffectToList(Card parentCard, EffectTrigger triggerType, Effect effect)
    {
        switch (triggerType)
        {
            case EffectTrigger.StartOfTurn:
                parentCard.effects.StartOfTurnEffects.Add(effect);
                break;
            case EffectTrigger.StartOfBattle:
                parentCard.effects.StartOfBattleEffects.Add(effect);
                break;
            case EffectTrigger.Support:
                parentCard.effects.SupportEffects.Add(effect);
                break;
            case EffectTrigger.BeforeAttacking:
                parentCard.effects.BeforeAttackingEffects.Add(effect);
                break;
            case EffectTrigger.OnDeath:
                parentCard.effects.OnDeathEffects.Add(effect);
                break;
            case EffectTrigger.OnGlobalDeath:
                parentCard.effects.OnGlobalDeathEffects.Add(effect);
                break;
            case EffectTrigger.OnSurvive:
                parentCard.effects.OnSurviveEffects.Add(effect);
                break;
            case EffectTrigger.Bloodthirst:
                parentCard.effects.BloodthirstEffects.Add(effect);
                break;
            case EffectTrigger.Meditate:
                parentCard.effects.MeditateEffects.Add(effect);
                break;
        }
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


public enum EffectTrigger
{
    StartOfTurn,

    StartOfBattle,
    Support,
    BeforeAttacking,
    OnDeath,
    OnGlobalDeath,
    OnSurvive,

    Bloodthirst,
    Meditate,
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
    RandomCardFromHand
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

