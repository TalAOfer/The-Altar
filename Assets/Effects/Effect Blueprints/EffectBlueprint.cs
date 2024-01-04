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

    [ShowIf("effectType", EffectType.ForceShapeshift)]
    public ShapeshiftType shapeshiftType;

    public WhoToChange whoToChange;

    public void SpawnEffect(EffectTrigger triggerType, Card parentCard)
    {
        GameObject newEffectGO = new GameObject(triggerType.ToString() + " : " + effectType.ToString());
        newEffectGO.transform.SetParent(parentCard.transform, false);
        newEffectGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        switch (effectType)
        {
            case EffectType.ChangeColor:
                var changeColorEffect = newEffectGO.AddComponent<ChangeColorEffect>();
                changeColorEffect.BaseInitialize(this);
                changeColorEffect.Initialize(whoToChange);
                AddEffectToList(parentCard, triggerType, changeColorEffect);
                break;
            case EffectType.GainPoints:
                var gainPointsEffect = newEffectGO.AddComponent<GainPointsEffect>();
                gainPointsEffect.BaseInitialize(this);
                gainPointsEffect.Initialize(pointsToAdd);
                AddEffectToList(parentCard, triggerType, gainPointsEffect);
                break;
            case EffectType.AlterBattlePoints:
                var hurtPointsAlterationEffect = newEffectGO.AddComponent<ModifyBattlePointsEffect>();
                hurtPointsAlterationEffect.BaseInitialize(this);
                hurtPointsAlterationEffect.Initialize(whoToChange, alterationAmount, modifierType, battlePointType, isConditional, decision);
                AddEffectToList(parentCard, triggerType, hurtPointsAlterationEffect);
                break;
            case EffectType.SummonCard:
                var summonCardEffect = newEffectGO.AddComponent<SummonCardEffect>();
                summonCardEffect.BaseInitialize(this);
                summonCardEffect.Initialize(cardBlueprint);
                AddEffectToList(parentCard, triggerType, summonCardEffect);
                break;
            case EffectType.DrawCard:
                var drawCardEffect = newEffectGO.AddComponent<DrawCardEffect>();
                drawCardEffect.BaseInitialize(this);
                AddEffectToList(parentCard, triggerType, drawCardEffect);
                break;
            case EffectType.GivePoints:
                var givePointsEffect = newEffectGO.AddComponent<GivePointsToRandomHandCardEffect>();
                givePointsEffect.BaseInitialize(this);
                givePointsEffect.Initialize(pointsToAdd);
                AddEffectToList(parentCard, triggerType, givePointsEffect);
                break;
            case EffectType.ForceShapeshift:
                var forcedShapeshiftEffect = newEffectGO.AddComponent<ForcedShapeshiftEffect>();
                forcedShapeshiftEffect.BaseInitialize(this);
                forcedShapeshiftEffect.Initialize(cardBlueprint, shapeshiftType);
                AddEffectToList(parentCard, triggerType, forcedShapeshiftEffect);
                break;
        }
    }

    public void AddEffectToList(Card parentCard, EffectTrigger triggerType, Effect effect)
    {
        switch (triggerType)
        {
            case EffectTrigger.Support:
                parentCard.effects.SupportEffects.Add(effect); 
                break;   
            case EffectTrigger.BeforeBattle:
                parentCard.effects.BeforeBattleEffects.Add(effect);
                break;
              case EffectTrigger.OnSurvive:
                parentCard.effects.OnSurviveEffects.Add(effect);
                break;
            case EffectTrigger.OnDeath:
                parentCard.effects.OnDeathEffects.Add(effect);
                break;

            case EffectTrigger.OnReveal:
                break;
            case EffectTrigger.OnGainPoints:
                parentCard.effects.OnGainPointsEffects.Add(effect);
                break;
            case EffectTrigger.OnGlobalDeath:
                parentCard.effects.OnGlobalDeathEffects.Add(effect);
                break;
        }
    }

    private bool ShouldShowPointsToAdd()
    {
        return effectType is EffectType.GainPoints or EffectType.GivePoints;
    }

    private bool ShouldShowCardBlueprint()
    {
        return effectType is EffectType.SummonCard or EffectType.ForceShapeshift;
    }

    private bool ShouldShowDecision()
    {
        return effectType is EffectType.AlterBattlePoints && isConditional;
    }
}


public enum EffectTrigger
{
    OnReveal,
    Support,
    BeforeBattle,
    OnGainPoints,
    OnDeath,
    OnSurvive,
    OnGlobalDeath
}

public enum EffectApplicationType
{
    Base,
    Persistent
}

public enum EffectType
{
    AlterBattlePoints,
    ChangeColor,
    GainPoints,
    GivePoints,
    SummonCard,
    DrawCard,
    ForceShapeshift,
}

public enum ModifierType
{
    Addition,
    Mult,
    Replace
}
