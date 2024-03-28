using Mono.Cecil;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Type")]
public class EffectTypeAsset : ScriptableObject
{
    public EffectType Type;
    public bool NeedsTarget;
    [ShowIf("NeedsTarget")]
    public bool TargetOnlyAliveCards;
    public bool NeedsAmount;
    public bool NeedsArchetype;
    public string BaseEffectText;
    public EffectTypeArchetype TypeArchetype;

    [ShowIf("TypeArchetype", EffectTypeArchetype.Grantable)]
    public string GrantVerb = "grant";
    [ShowIf("TypeArchetype", EffectTypeArchetype.Grantable)]
    public string GainVerb = "gain";

    public Effect InstantiateEffect(EffectBlueprint blueprint, Card parentCard, BattleRoomDataProvider data)
    {
        return Type switch
        {
            EffectType.DebugEffect => new DebugEffect(blueprint, data, parentCard),
            EffectType.AddBattlePointModifier => new AddBattlePointModifier(blueprint, data, parentCard),
            EffectType.SetColor => new SetColorEffect(blueprint, data, parentCard),
            EffectType.ToggleColor => new ToggleColorEffect(blueprint, data, parentCard),
            EffectType.GainPoints => new GainPointsEffect(blueprint, data, parentCard),
            EffectType.DrawCard => new DrawCardEffect(blueprint, data, parentCard),
            EffectType.SpawnCardToHand => new SpawnCardToHandEffect(blueprint, data, parentCard),
            EffectType.AddEffect => new AddEffectEffect(blueprint, data, parentCard),
            EffectType.AddGuardian => new AddGuardianEffect(blueprint, data, parentCard),
            EffectType.SpawnEnemy => new SpawnEnemiesEffect(blueprint, data, parentCard),
            EffectType.GainBuff => new GainBuffEffect(blueprint, data, parentCard),
            EffectType.TakeDamage => new TakeDamageEffect(blueprint, data, parentCard),
            _ => null,
        };
    }
}

public enum EffectTypeArchetype
{
    Grantable, Nontargetable, Starter
}