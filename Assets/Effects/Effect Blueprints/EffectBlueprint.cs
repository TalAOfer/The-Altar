using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Effect")]
public class EffectBlueprint : ScriptableObject
{
    public AllEvents events;
    public EffectType effectType;
    public EffectApplicationType applicationType;

    public float predelay = 0f;
    public float postdelay = 1f;

    [ShowIf("effectType", EffectType.AlterBattlePoints)]
    public BattlePointType battlePointType;

    [ShowIf("effectType", EffectType.AlterBattlePoints)]
    public ModifierType modifierType;

    [ShowIf("effectType", EffectType.AlterBattlePoints)]
    public float alterationAmount;

    [ShowIf("effectType", EffectType.AlterBattlePoints)]
    public bool isConditional;

    [ShowIf("@ShouldShowDecision()")]
    public Decision decision;

    [ShowIf("@ShouldShowPointsToAdd()")]
    public int pointsToAdd;

    [ShowIf("@ShouldShowCardBlueprint()")]
    public CardBlueprint cardBlueprint;

    [ShowIf("effectType", EffectType.AddGuardian)]
    public GuardianType guardianType;

    [ShowIf("effectType", EffectType.AddGuardian)]
    public EffectApplicationType guardianApplicationType;

    [ShowIf("@ShouldShowWhoToChange()")]
    public WhoToChange whoToChange;

    public void SpawnEffect(EffectTrigger triggerType, Card parentCard)
    {
        GameObject newEffectGO = new GameObject(triggerType.ToString() + " : " + effectType.ToString());
        newEffectGO.transform.SetParent(parentCard.transform, false);
        newEffectGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (triggerType is EffectTrigger.StartOfBattle && isConditional) Debug.LogError("Conditional effects need to be in before attacking, not start of battle");

        switch (effectType)
        {
            case EffectType.DummyEffect:
                BaseInitializeEffect<DummyEffect>(newEffectGO, triggerType, parentCard);
                break;
            case EffectType.ChangeColor:
                var changeColorEffect = BaseInitializeEffect<ChangeColorEffect>(newEffectGO, triggerType, parentCard);
                changeColorEffect.Initialize(whoToChange);
                break;
            case EffectType.GainPoints:
                var gainPointsEffect = BaseInitializeEffect<GainPointsEffect>(newEffectGO, triggerType, parentCard);
                gainPointsEffect.Initialize(pointsToAdd);
                break;
            case EffectType.AlterBattlePoints:
                var hurtPointsAlterationEffect = BaseInitializeEffect<ModifyBattlePointsEffect>(newEffectGO, triggerType, parentCard);
                hurtPointsAlterationEffect.Initialize(whoToChange, alterationAmount, modifierType, battlePointType, isConditional, decision);
                break;
            case EffectType.SummonCard:
                var summonCardEffect = BaseInitializeEffect<SummonCardEffect>(newEffectGO, triggerType, parentCard);
                summonCardEffect.Initialize(cardBlueprint);
                break;
            case EffectType.DrawCard:
                BaseInitializeEffect<DrawCardEffect>(newEffectGO, triggerType, parentCard);
                break;
            case EffectType.GivePointsToRandomPlayerCard:
                var givePointsEffect = BaseInitializeEffect<GivePointsToRandomHandCardEffect>(newEffectGO, triggerType, parentCard);
                givePointsEffect.Initialize(pointsToAdd);
                break;
            case EffectType.ForceShapeshift:
                var forcedShapeshiftEffect = BaseInitializeEffect<ForcedShapeshiftEffect>(newEffectGO, triggerType, parentCard);
                forcedShapeshiftEffect.Initialize(cardBlueprint);
                break;
            case EffectType.AddGuardian:
                var addGuardianEffect = BaseInitializeEffect<AddGuardianEffect>(newEffectGO, triggerType, parentCard);
                addGuardianEffect.Initialize(guardianType, guardianApplicationType);
                break;
            case EffectType.AddBattlePointsPerX:
                BaseInitializeEffect<AddBattlePointsAccordingToXEffect>(newEffectGO, triggerType, parentCard);
                break;
            case EffectType.AddBattlePointsAccordingToOtherRevealedEnemyCard:
                BaseInitializeEffect<AddBattlePointsAccordingToOtherRevealedCardEffect>(newEffectGO, triggerType, parentCard);
                break;
            case EffectType.GivePointsToOtherRevealedEnemyCard:
                BaseInitializeEffect<GivePointsToOtherRevealedEnemyCardEffect>(newEffectGO, triggerType, parentCard);
                break;
            case EffectType.ChangeColorToAllRevealedEnemies:
                BaseInitializeEffect<ChangeColorToAllRevealedEnemies>(newEffectGO, triggerType, parentCard);
                break;
        }
    }

    public T BaseInitializeEffect<T>(GameObject newEffectGO, EffectTrigger triggerType, Card parentCard) where T : Effect
    {
        T effect = newEffectGO.AddComponent<T>();
        effect.BaseInitialize(this, parentCard); // Assuming BaseInitialize is a method in T or its base class

        AddEffectToList(parentCard, triggerType, effect);

        return effect;
    }

public void AddEffectToList(Card parentCard, EffectTrigger triggerType, Effect effect)
    {
        switch (triggerType)
        {
            case EffectTrigger.Support:
                parentCard.effects.SupportEffects.Add(effect); 
                break;
            case EffectTrigger.StartOfBattle:
                parentCard.effects.StartOfBattleEffects.Add(effect);
                break;
            case EffectTrigger.BeforeAttacking:
                parentCard.effects.BeforeAttackingEffects.Add(effect);
                break;
            case EffectTrigger.OnSurvive:
                parentCard.effects.OnSurviveEffects.Add(effect);
                break;
            case EffectTrigger.OnDeath:
                parentCard.effects.OnDeathEffects.Add(effect);
                break;

            case EffectTrigger.OnObtain:
                parentCard.effects.OnObtainEffects.Add(effect);
                break;
            case EffectTrigger.StartOfTurn:
                parentCard.effects.StartOfTurnEffects.Add(effect);
                break;
            case EffectTrigger.OnGainPoints:
                parentCard.effects.OnGainPointsEffects.Add(effect);
                break;
            case EffectTrigger.OnGlobalDeath:
                parentCard.effects.OnGlobalDeathEffects.Add(effect);
                break;
            case EffectTrigger.OnActionTaken:
                parentCard.effects.OnActionTakenEffects.Add(effect);
                break;
            case EffectTrigger.EndOfTurn:
                parentCard.effects.EndOfTurnEffects.Add(effect);
                break;
        }
    }

    private bool ShouldShowPointsToAdd()
    {
        return effectType is EffectType.GainPoints or EffectType.GivePointsToRandomPlayerCard;
    }

    private bool ShouldShowCardBlueprint()
    {
        return effectType is EffectType.SummonCard or EffectType.ForceShapeshift;
    }

    private bool ShouldShowDecision()
    {
        return effectType is EffectType.AlterBattlePoints && isConditional;
    }

    private bool ShouldShowWhoToChange()
    {
        return effectType is EffectType.ChangeColor or EffectType.AlterBattlePoints;
    }
}


public enum EffectTrigger
{
    OnObtain,
    StartOfTurn,

    StartOfBattle,
    Support,
    BeforeAttacking,
    OnSurvive,
    OnDeath,
    OnGlobalDeath,

    OnSacrifice,
    OnGainPoints,
    EndOfTurn,
    OnActionTaken,
}

public enum EffectApplicationType
{
    Base,
    Bond,
    Persistent
}

public enum EffectType
{
    DummyEffect,
    AlterBattlePoints,
    ChangeColor,
    GainPoints,
    GivePointsToRandomPlayerCard,
    SummonCard,
    DrawCard,
    ForceShapeshift,
    AddGuardian,
    AddBattlePointsPerX,
    AddBattlePointsAccordingToOtherRevealedEnemyCard,
    GivePointsToOtherRevealedEnemyCard,
    ChangeColorToAllRevealedEnemies,
}

