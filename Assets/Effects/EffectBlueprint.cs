using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Effect")]
public class EffectBlueprint : ScriptableObject
{
    public EffectType effectType;

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
    public void SpawnEffect(EffectTrigger triggerType, Card parentCard)
    {
        GameObject newEffectGO = new GameObject(triggerType.ToString() + " : " + effectType.ToString());

        // Set the new GameObject's parent
        newEffectGO.transform.SetParent(parentCard.transform, false);

        // Optionally, reset local position and rotation if needed
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
                var gainPointsEffect = newEffectGO.AddComponent<GainPointsEffect>();
                gainPointsEffect.InitializeDelay(predelay, postdelay);
                gainPointsEffect.Initialize(pointsToAdd);
                AddEffectToList(parentCard, triggerType, gainPointsEffect);
                break;
            case EffectType.Shapeshift:
                var shapeShiftEffect = newEffectGO.AddComponent<ShapeshiftEffect>();
                shapeShiftEffect.InitializeDelay(predelay, postdelay);
                AddEffectToList(parentCard , triggerType, shapeShiftEffect);   
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
                parentCard.BeforeBattleEffects.Add(effect);
                break;
            case EffectTrigger.OnGainPoints:
                parentCard.OnGainPointsEffects.Add(effect);
                break;
            case EffectTrigger.OnDeath:
                parentCard.OnDeathEffects.Add(effect);
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
    AttackPointsAlteration
}

public enum EffectType
{
    GetAdvantageOverColor,
    ChangeColor,
    GainPoints,
    Shapeshift
}
