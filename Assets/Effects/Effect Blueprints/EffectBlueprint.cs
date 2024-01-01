using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Effect")]
public class EffectBlueprint : ScriptableObject
{
    public AllEvents events;
    public EffectType effectType;

    public float predelay = 0f;
    public float postdelay = 1f;

    [ShowIf("effectType", EffectType.GetAdvantageOverColor)]
    public CardColor ThisCardIsStronerOn;
    [ShowIf("@ShouldShowAdvantagePoints()")]
    public int advantageAmount;
    [ShowIf("effectType", EffectType.GetAdvantageOverColor)]
    public int disadvantageAmount;

    [ShowIf("@ShouldShowPointsToAdd()")]
    public int pointsToAdd;

    [ShowIf("effectType", EffectType.ChangeColor)]
    public CardColor changeTo;

    [ShowIf("effectType", EffectType.AlterBattlePoints)]
    public float mult;

    [ShowIf("effectType", EffectType.AlterBattlePoints)]
    public PointAlterationType alterationType;

    [ShowIf("@ShouldShowCardBlueprint()")]
    public CardBlueprint cardBlueprint;

    public void SpawnEffect(EffectTrigger triggerType, Card parentCard)
    {
        GameObject newEffectGO = new GameObject(triggerType.ToString() + " : " + effectType.ToString());
        newEffectGO.transform.SetParent(parentCard.transform, false);
        newEffectGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        switch (effectType)
        {
            case EffectType.GetAdvantageOverColor:
                var advantageEffect = newEffectGO.AddComponent<ColorAdvantageEffect>();
                advantageEffect.BaseInitialize(predelay, postdelay,events);
                advantageEffect.Initialize(ThisCardIsStronerOn, advantageAmount, disadvantageAmount);
                AddEffectToList(parentCard, triggerType, advantageEffect);
                break;
            case EffectType.ChangeColor:
                var changeColorEffect = newEffectGO.AddComponent<ChangeColorEffect>();
                changeColorEffect.BaseInitialize(predelay, postdelay, events);
                changeColorEffect.Initialize(changeTo);
                AddEffectToList(parentCard, triggerType, changeColorEffect);
                break;
            case EffectType.GainPoints:
                var gainPointsEffect = newEffectGO.AddComponent<GainPointsEffect>();
                gainPointsEffect.BaseInitialize(predelay, postdelay, events);
                gainPointsEffect.Initialize(pointsToAdd);
                AddEffectToList(parentCard, triggerType, gainPointsEffect);
                break;
            case EffectType.Shapeshift:
                var shapeShiftEffect = newEffectGO.AddComponent<ShapeshiftEffect>();
                shapeShiftEffect.BaseInitialize(predelay, postdelay, events);
                AddEffectToList(parentCard, triggerType, shapeShiftEffect);
                break;
            case EffectType.AlterBattlePoints:
                var hurtPointsAlterationEffect = newEffectGO.AddComponent<BattlePointAlterationEffect>();
                hurtPointsAlterationEffect.BaseInitialize(predelay, postdelay, events);
                hurtPointsAlterationEffect.Initialize(mult, alterationType);
                AddEffectToList(parentCard, triggerType, hurtPointsAlterationEffect);
                break;
            case EffectType.SummonCard:
                var summonCardEffect = newEffectGO.AddComponent<SummonCardEffect>();
                summonCardEffect.BaseInitialize(predelay, postdelay, events);
                summonCardEffect.Initialize(cardBlueprint);
                AddEffectToList(parentCard, triggerType, summonCardEffect);
                break;
            case EffectType.DrawCard:
                var drawCardEffect = newEffectGO.AddComponent<DrawCardEffect>();
                drawCardEffect.BaseInitialize(predelay, postdelay, events);
                AddEffectToList(parentCard, triggerType, drawCardEffect);
                break;
            case EffectType.GivePoints:
                var givePointsEffect = newEffectGO.AddComponent<GivePointsToCardEffect>();
                givePointsEffect.BaseInitialize(predelay, postdelay, events);
                givePointsEffect.Initialize(pointsToAdd);
                AddEffectToList(parentCard, triggerType, givePointsEffect);
                break;
            case EffectType.ForceShapeshift:
                var forcedShapeshiftEffect = newEffectGO.AddComponent<ForcedShapeshiftEffect>();
                forcedShapeshiftEffect.BaseInitialize(predelay, postdelay, events);
                forcedShapeshiftEffect.Initialize(cardBlueprint);
                AddEffectToList(parentCard, triggerType, forcedShapeshiftEffect);
                break;
            case EffectType.GetAdvantageOverStrongerCards:
                var getAdvantageOverStrongerCardsEffect = newEffectGO.AddComponent<AdvantageOverStrongerCardsEffect>();
                getAdvantageOverStrongerCardsEffect.BaseInitialize(predelay, postdelay, events);
                getAdvantageOverStrongerCardsEffect.Initialize(advantageAmount);
                AddEffectToList(parentCard, triggerType, getAdvantageOverStrongerCardsEffect);
                break;
        }
    }

    public void AddEffectToList(Card parentCard, EffectTrigger triggerType, Effect effect)
    {
        switch (triggerType)
        {
            case EffectTrigger.OnReveal:
                break;
            case EffectTrigger.BeforeBattle:
                parentCard.effects.BeforeBattleEffects.Add(effect);
                break;
            case EffectTrigger.OnGainPoints:
                parentCard.effects.OnGainPointsEffects.Add(effect);
                break;
            case EffectTrigger.OnDeath:
                parentCard.effects.OnDeathEffects.Add(effect);
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
            case EffectTrigger.OnGlobalDeath:
                parentCard.effects.OnGlobalDeathEffects.Add(effect);
                break;
        }
    }

    private bool ShouldShowPointsToAdd()
    {
        return effectType is EffectType.GainPoints or EffectType.GivePoints;
    }

    private bool ShouldShowAdvantagePoints()
    {
        return effectType is EffectType.GetAdvantageOverColor or EffectType.GetAdvantageOverStrongerCards;
    }

    private bool ShouldShowCardBlueprint()
    {
        return effectType is EffectType.SummonCard or EffectType.ForceShapeshift;
    }
}


public enum EffectTrigger
{
    OnReveal,
    BeforeBattle,
    OnGainPoints,
    OnDeath,
    HurtPointsAlteration,
    AttackPointsAlteration,
    OnSurvive,
    OnGlobalDeath
}



public enum EffectType
{
    GetAdvantageOverColor,
    ChangeColor,
    GainPoints,
    GivePoints,
    Shapeshift,
    AlterBattlePoints,
    SummonCard,
    DrawCard,
    ForceShapeshift,
    GetAdvantageOverStrongerCards,
}

public enum AlterationType
{
    Addition,
    Mult
}
