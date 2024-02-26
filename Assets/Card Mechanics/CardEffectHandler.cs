using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectHandler : SerializedMonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private EffectTriggerRegistry triggers;

    [ShowInInspector]
    private Dictionary<EffectTrigger, List<Effect>> effects = new();

    public void Init(CardBlueprint blueprint)
    {
        SpawnEffects(blueprint);
    }

    public void SpawnEffects(CardBlueprint blueprint)
    {
        foreach (var trigger in triggers.triggers)
        {
            var effectsForTrigger = blueprint.GetEffectsForTrigger(trigger);
            foreach (var effectBlueprint in effectsForTrigger)
            {
                effectBlueprint.Value.SpawnEffect(trigger, card); // Ensure SpawnEffect is adapted to use EffectTriggerSO
            }
        }
    }

    public void AddEffectToDictionary(EffectTrigger trigger, Effect effect)
    {
        if (!effects.ContainsKey(trigger))
        {
            effects[trigger] = new List<Effect>();
        }
        effects[trigger].Add(effect);
    }

    public IEnumerator RemoveCurrentEffects()
    {
        foreach (var effectList in effects.Values)
        {
            RemoveEffects(effectList);
        }

        yield return new WaitForFixedUpdate();
    }

    private void RemoveEffects(List<Effect> effects)
    {
        List<Effect> toRemove = new List<Effect>(effects);
        foreach (Effect effect in toRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            effects.Remove(effect);
            Destroy(effect.gameObject);
        }
    }

    public IEnumerator ApplyEffects(TriggerType type)
    {
        EffectTrigger triggerType = triggers.GetTriggerByEnum(type);

        if (effects.TryGetValue(triggerType, out List<Effect> allEffects))
        {
            foreach (Effect effect in allEffects)
            {
                yield return StartCoroutine(effect.Trigger());
            }
        }

        yield return null;
    }
}