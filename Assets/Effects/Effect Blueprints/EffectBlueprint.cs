using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Effect")]
public class EffectBlueprint : ScriptableObject
{
    public AllEvents events;
    public EffectType effectType;
    public ResponseNeeded effectResponseType;

    public float predelay = 0f;
    public float postdelay = 1f;

    [ShowIf("effectType", EffectType.GetAdvantageOverColor)]
    public CardColor ThisCardIsStronerOn;
    [ShowIf("effectType", EffectType.GetAdvantageOverColor)]
    public int advantageAmount;
    [ShowIf("effectType", EffectType.GetAdvantageOverColor)]
    public int disadvantageAmount;

    [ShowIf("effectType", EffectType.GainPoints)]
    public int pointsToAdd;

    [ShowIf("effectType", EffectType.ChangeColor)]
    public CardColor changeTo;

    [ShowIf("effectType", EffectType.AlterBattlePoints)]
    public float mult;

    [ShowIf("effectType", EffectType.AlterBattlePoints)]
    public PointAlterationType alterationType;

    public void SpawnEffect(EffectTrigger triggerType, Card parentCard)
    {
        GameObject newEffectGO = new GameObject(triggerType.ToString() + " : " + effectType.ToString());
        newEffectGO.transform.SetParent(parentCard.transform, false);
        newEffectGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        switch (effectType)
        {
            case EffectType.GetAdvantageOverColor:
                var advantageEffect = newEffectGO.AddComponent<ColorAdvantageEffect>();
                advantageEffect.InitializeDelay(predelay, postdelay);
                AddEffectToList(parentCard, triggerType, advantageEffect);
                advantageEffect.Initialize(ThisCardIsStronerOn, advantageAmount, disadvantageAmount);
                break;
            case EffectType.ChangeColor:
                var changeColorEffect = newEffectGO.AddComponent<ChangeColorEffect>();
                changeColorEffect.InitializeDelay(predelay, postdelay);
                changeColorEffect.Initialize(changeTo);
                AddEffectToList(parentCard, triggerType, changeColorEffect);
                break;
            case EffectType.GainPoints:
                if (effectResponseType == ResponseNeeded.None)
                {
                    var gainPointsEffect = newEffectGO.AddComponent<GainPointsEffect>();
                    gainPointsEffect.InitializeDelay(predelay, postdelay);
                    gainPointsEffect.Initialize(pointsToAdd);
                    AddEffectToList(parentCard, triggerType, gainPointsEffect);
                } 
                
                else
                {
                    var givePointsEffect = newEffectGO.AddComponent<GivePointsToCardEffect>();
                    givePointsEffect.InitializeDelay(predelay, postdelay);
                    givePointsEffect.BaseInitialize(effectResponseType, events);
                    givePointsEffect.Initialize(pointsToAdd);
                    AddEffectToList(parentCard, triggerType, givePointsEffect);
                }
                break;
            case EffectType.Shapeshift:
                var shapeShiftEffect = newEffectGO.AddComponent<ShapeshiftEffect>();
                shapeShiftEffect.InitializeDelay(predelay, postdelay);
                AddEffectToList(parentCard, triggerType, shapeShiftEffect);
                break;
            case EffectType.AlterBattlePoints:
                var hurtPointsAlterationEffect = newEffectGO.AddComponent<BattlePointAlterationEffect>();
                hurtPointsAlterationEffect.InitializeDelay(predelay, postdelay);
                hurtPointsAlterationEffect.Initialize(mult, alterationType);
                AddEffectToList(parentCard, triggerType, hurtPointsAlterationEffect);
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
        }
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
}

public enum ResponseNeeded
{
    None,
    Choice,
    RandomFromHand,
    RandomFromMap
}

public enum EffectType
{
    GetAdvantageOverColor,
    ChangeColor,
    GainPoints,
    Shapeshift,
    AlterBattlePoints,
}

public enum AlterationType
{
    Addition,
    Mult
}
