using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectHandler : SerializedMonoBehaviour
{
    [SerializeField] private Card _card;
    [SerializeField] private EffectTriggerRegistry _triggers;
    private EventRegistry _events;

    [ShowInInspector]
    public Dictionary<EffectTriggerAsset, List<Effect>> EffectsDict { get; private set; } = new();

    public void Init(CardBlueprint blueprint)
    {
        InstantiateDefaultCardEffects(blueprint);
        _events = Locator.Events;
    }

    public void InstantiateDefaultCardEffects(CardBlueprint blueprint)
    {
        foreach (var effectBlueprintRef in blueprint.Effects)
        {
            Effect effect = effectBlueprintRef.Value.InstantiateEffect(_card, _card.DataProvider);
            AddEffectToDictionary(effect);
        }
    }

    public void AddEffectToDictionary(Effect effect)
    {
        if (!EffectsDict.ContainsKey(effect.EffectTrigger))
        {
            EffectsDict[effect.EffectTrigger] = new List<Effect>();
        }

        EffectsDict[effect.EffectTrigger].Add(effect);
    }

    public IEnumerator RemoveAllEffects()
    {
        List<EffectTriggerAsset> keys = new(EffectsDict.Keys);

        foreach (var trigger in keys)
        {
            RemoveEffects(trigger);
        }

        yield return new WaitForFixedUpdate();
    }

    private void RemoveEffects(EffectTriggerAsset trigger)
    {
        List<Effect> effects = EffectsDict[trigger];

        List<Effect> toRemove = new(effects);
        foreach (Effect effect in toRemove)
        {
            if (effect.IsPersistent) continue;
            effect.OnRemoveEffect(_card);
            effects.Remove(effect);
        }

        if (effects.Count == 0)
        {
            EffectsDict.Remove(trigger);
        }
    }

    public IEnumerator ApplyEffects(TriggerType type, IEventData eventData)
    {
        EffectTriggerAsset triggerType = _triggers.GetTriggerAssetByEnum(type);

        /*Debugging
        //Debug.Log("Is there a dict: " + (_effectsDict != null).ToString());
        //Debug.Log("Is there a key: " + (triggerType != null).ToString());
        */

        if (!EffectsDict.ContainsKey(triggerType)) yield break;

        if (EffectsDict.TryGetValue(triggerType, out List<Effect> allEffects))
        {
            foreach (Effect effect in allEffects)
            {
                if (effect.IsFrozenTillEndOfTurn) continue;
                if (effect.TriggerFilter?.Decide(_card, eventData) == false) continue;

                yield return effect.Trigger(null, eventData);
            }
        }

        yield break;
    }

    public void TriggerEffects(TriggerType type, IEventData eventData)
    {
        EffectTriggerAsset triggerType = _triggers.GetTriggerAssetByEnum(type);

        if (!EffectsDict.ContainsKey(triggerType)) return;

        if (EffectsDict.TryGetValue(triggerType, out List<Effect> allEffects))
        {
            foreach (Effect effect in allEffects)
            {
                if (effect.TriggerFilter == null || effect.TriggerFilter.Decide(_card, eventData))
                {
                    EffectNode effectNode = new(effect, eventData, null);
                    _events.OnEffectTriggered.Raise(this, effectNode);
                }
            }
        }
    }

    public void ReenableOnetimeEffects()
    {
        foreach (var effectEntry in EffectsDict)
        {
            foreach (Effect effect in effectEntry.Value)
            {
                effect.UnfreezeEffect();
            }
        }
    }
}