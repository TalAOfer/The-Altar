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

    private List<Effect> GetListByEnum(TriggerType type) => _effectsDict[_triggers.GetTriggerByEnum(type)];


    public void Init(CardBlueprint blueprint)
    {
        InstantiateEffects(blueprint);
    }

    public void InstantiateEffects(CardBlueprint blueprint)
    {
        foreach (var trigger in _triggers.triggers)
        {
            var effectsForTrigger = blueprint.GetEffectsForTrigger(trigger);
            foreach (var effectBlueprint in effectsForTrigger)
            {
                effectBlueprint.Value.InstantiateEffect(trigger, _card); // Ensure SpawnEffect is adapted to use EffectTriggerSO
            }
        }
    }

    public void AddEffectToDictionary(EffectTrigger trigger, Effect effect)
    {
        if (!_effectsDict.ContainsKey(trigger))
        {
            _effectsDict[trigger] = new List<Effect>();
        }
        _effectsDict[trigger].Add(effect);
    }

    public IEnumerator RemoveCurrentEffects()
    {
        foreach (var trigger in _effectsDict.Keys)
        {
            RemoveEffects(trigger);
        }

        yield return new WaitForFixedUpdate();
    }

    private void RemoveEffects(EffectTrigger trigger)
    {
        List<Effect> effects = _effectsDict[trigger];

        List<Effect> toRemove = new List<Effect>(effects);
        foreach (Effect effect in toRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            effects.Remove(effect);
            Destroy(effect.gameObject);
        }

        if (effects.Count == 0)
        {
            _effectsDict.Remove(trigger);
        }

    }

    public IEnumerator ApplyEffects(TriggerType type)
    {
        EffectTrigger triggerType = _triggers.GetTriggerByEnum(type);

        /*Debugging
        //Debug.Log("Is there a dict: " + (_effectsDict != null).ToString());
        //Debug.Log("Is there a key: " + (triggerType != null).ToString());
        */

        if (!_effectsDict.ContainsKey(triggerType)) yield break;

        if (_effectsDict.TryGetValue(triggerType, out List<Effect> allEffects))
        {
            foreach (Effect effect in allEffects)
            {
                yield return StartCoroutine(effect.Trigger());
            }
        }

        if (type is TriggerType.Meditate)
        {
            RemoveEffects(triggerType);
        }

        yield return null;
    }

}