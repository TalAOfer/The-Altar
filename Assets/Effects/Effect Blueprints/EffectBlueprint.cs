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

    [ShowIf("effectType", EffectType.Advantage)]
    public Decision decision;
    [ShowIf("effectType", EffectType.Advantage)]
    public int advantageAmount;
    [ShowIf("effectType", EffectType.Advantage)]
    public int disadvantageAmount;

    [ShowIf("@ShouldShowPointsToAdd()")]
    public int pointsToAdd;

    [ShowIf("effectType", EffectType.AlterBattlePoints)]
    public float alterationAmount;

    [ShowIf("effectType", EffectType.AlterBattlePoints)]
    public PointAlterationType alterationType;

    [ShowIf("@ShouldShowCardBlueprint()")]
    public CardBlueprint cardBlueprint;

    [ShowIf("effectType", EffectType.ForceShapeshift)]
    public ShapeshiftType shapeshiftType;

    [ShowIf("effectType", EffectType.AlterBattlePoints)]
    public ModifierType modifierType;

    public WhoToChange whoToChange;

    public void SpawnEffect(EffectTrigger triggerType, Card parentCard)
    {
        GameObject newEffectGO = new GameObject(triggerType.ToString() + " : " + effectType.ToString());
        newEffectGO.transform.SetParent(parentCard.transform, false);
        newEffectGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        switch (effectType)
        {
            case EffectType.Advantage:
                var advantagesEffect = newEffectGO.AddComponent<AdvantageEffect>();
                advantagesEffect.BaseInitialize(this);
                advantagesEffect.Initialize(whoToChange, decision, advantageAmount, disadvantageAmount);
                AddEffectToList(parentCard, triggerType, advantagesEffect);
                break;
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
                var hurtPointsAlterationEffect = newEffectGO.AddComponent<BattlePointAlterationEffect>();
                hurtPointsAlterationEffect.BaseInitialize(this);
                hurtPointsAlterationEffect.Initialize(alterationAmount, modifierType, alterationType);
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
                var givePointsEffect = newEffectGO.AddComponent<GivePointsToCardEffect>();
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
            case EffectTrigger.HurtPointsAlteration:
                parentCard.effects.HurtPointsAlterationEffects.Add(effect);
                break;
            case EffectTrigger.AttackPointsAlteration:
                parentCard.effects.AttackPointsAlterationEffects.Add(effect);
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
}


public enum EffectTrigger
{
    OnReveal,
    Support,
    BeforeBattle,
    OnGainPoints,
    OnDeath,
    HurtPointsAlteration,
    AttackPointsAlteration,
    OnSurvive,
    OnGlobalDeath
}

public enum EffectApplicationType
{
    Base,
    ForBattle,
    Persistent
}

public enum EffectType
{
    Advantage,
    ChangeColor,
    GainPoints,
    GivePoints,
    AlterBattlePoints,
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
