using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardEffectHandler : SerializedMonoBehaviour
{
    [SerializeField] private Card _card;
    [SerializeField] private EffectTriggerRegistry _triggers;

    [ShowInInspector]
    private Dictionary<EffectTrigger, List<Effect>> _effectsDict = new();

    public void Init(CardBlueprint blueprint)
    {
        InstantiateDefaultCardEffects(blueprint);
    }

    public void InstantiateDefaultCardEffects(CardBlueprint blueprint)
    {
        foreach (var trigger in _triggers.triggers)
        {
            var effectsInTrigger = blueprint.GetEffectsInTrigger(trigger);
            foreach (var effectBlueprint in effectsInTrigger)
            {
                Effect effect = effectBlueprint.Value.InstantiateEffect(trigger, _card, _card.DataProvider);
                AddEffectToDictionary(effect);
            }
        }
    }

    public void AddEffectToDictionary(Effect effect)
    {
        if (!_effectsDict.ContainsKey(effect.trigger))
        {
            _effectsDict[effect.trigger] = new List<Effect>();
        }

        _effectsDict[effect.trigger].Add(effect);
    }

    public IEnumerator RemoveAllEffects()
    {
        List<EffectTrigger> keys = new(_effectsDict.Keys);

        foreach (var trigger in keys)
        {
            RemoveEffects(trigger);
        }

        yield return new WaitForFixedUpdate();
    }

    private void RemoveEffects(EffectTrigger trigger)
    {
        List<Effect> effects = _effectsDict[trigger];

        List<Effect> toRemove = new(effects);
        foreach (Effect effect in toRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            effects.Remove(effect);
        }

        if (effects.Count == 0)
        {
            _effectsDict.Remove(trigger);
        }
    }

    public IEnumerator ApplyEffects(TriggerType type)
    {
        EffectTrigger triggerType = _triggers.GetTriggerAssetByEnum(type);

        /*Debugging
        //Debug.Log("Is there a dict: " + (_effectsDict != null).ToString());
        //Debug.Log("Is there a key: " + (triggerType != null).ToString());
        */

        if (!_effectsDict.ContainsKey(triggerType)) yield break;

        if (_effectsDict.TryGetValue(triggerType, out List<Effect> allEffects))
        {
            foreach (Effect effect in allEffects)
            {
                yield return effect.Trigger();
            }
        }

        if (type is TriggerType.Meditate)
        {
            RemoveEffects(triggerType);
        }

        yield return null;
    }

}