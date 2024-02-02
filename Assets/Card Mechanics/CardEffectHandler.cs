using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectHandler : MonoBehaviour
{
    [SerializeField] private Card card;

    public List<Effect> StartOfTurnEffects = new();

    public List<Effect> StartOfBattleEffects = new();
    public List<Effect> SupportEffects = new();
    public List<Effect> BeforeAttackingEffects = new();
    public List<Effect> OnDeathEffects = new();
    public List<Effect> OnGlobalDeathEffects = new();
    public List<Effect> OnSurviveEffects = new();

    public List<Effect> BloodthirstEffects = new();
    public List<Effect> MeditateEffects = new();

    public void Init(CardBlueprint blueprint)
    {
        SpawnEffects(blueprint);
    }

    public void SpawnEffects(CardBlueprint blueprint)
    {
        foreach (EffectBlueprint effect in blueprint.StartOfBattle)
        {
            effect.SpawnEffect(EffectTrigger.StartOfBattle, card);
        }

        foreach (EffectBlueprint effect in blueprint.BeforeAttacking)
        {
            effect.SpawnEffect(EffectTrigger.BeforeAttacking, card);
        }

        foreach (EffectBlueprint effect in blueprint.OnDeath)
        {
            effect.SpawnEffect(EffectTrigger.OnDeath, card);
        }

        foreach (EffectBlueprint effect in blueprint.OnSurvive)
        {
            effect.SpawnEffect(EffectTrigger.OnSurvive, card);
        }

        foreach (EffectBlueprint effect in blueprint.OnGlobalDeath)
        {
            effect.SpawnEffect(EffectTrigger.OnGlobalDeath, card);
        }

        foreach (EffectBlueprint effect in blueprint.Support)
        {
            effect.SpawnEffect(EffectTrigger.Support, card);
        }

        foreach (EffectBlueprint effect in blueprint.StartOfTurn)
        {
            effect.SpawnEffect(EffectTrigger.StartOfTurn, card);
        }

        foreach (EffectBlueprint effect in blueprint.Meditate)
        {
            effect.SpawnEffect(EffectTrigger.Meditate, card);
        }

        foreach (EffectBlueprint effect in blueprint.Bloodthirst)
        {
            effect.SpawnEffect(EffectTrigger.Bloodthirst, card);
        }
    }

    public IEnumerator RemoveCurrentEffects()
    {
        RemoveEffects(StartOfBattleEffects);
        RemoveEffects(BeforeAttackingEffects);
        RemoveEffects(OnSurviveEffects);
        RemoveEffects(OnDeathEffects);
        RemoveEffects(OnGlobalDeathEffects);
        RemoveEffects(SupportEffects);
        RemoveEffects(StartOfTurnEffects);
        RemoveEffects(MeditateEffects);
        RemoveEffects(BloodthirstEffects);

        yield return new WaitForFixedUpdate();
    }

    private void RemoveEffects(List<Effect> effects)
    {
        if (effects.Count <= 0) return;

        List<Effect> toRemove = new List<Effect>(effects);
        foreach (Effect effect in toRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            effects.Remove(effect);
            Destroy(effect.gameObject);
        }
    }

    // Apply methods for each effect type
    public IEnumerator ApplyStartOfBattleEffects(Card otherCard)
    {
        yield return ApplyEffects(StartOfBattleEffects);
    }

    public IEnumerator ApplyBeforeAttackingEffects(Card otherCard)
    {
        yield return ApplyEffects(BeforeAttackingEffects);
    }

    public IEnumerator ApplyOnSurviveEffects(Card otherCard)
    {
        yield return ApplyEffects(OnSurviveEffects);
    }

    public IEnumerator ApplyOnDeathEffects(Card killingCard)
    {
        yield return ApplyEffects(OnDeathEffects);
    }

    public IEnumerator ApplyOnGlobalDeathEffects()
    {
        yield return ApplyEffects(OnGlobalDeathEffects);
    }

    public IEnumerator ApplySupportEffects(Card otherCard)
    {
        yield return ApplyEffects(SupportEffects);
    }

    public IEnumerator ApplyStartOfTurnEffects()
    {
        yield return ApplyEffects(StartOfTurnEffects);
    }

    public IEnumerator ApplyMeditateEffects()
    {
        yield return ApplyEffects(MeditateEffects);
        RemoveEffects(MeditateEffects);
    }

    public IEnumerator ApplyBloodthirstEffects()
    {
        yield return ApplyEffects(BloodthirstEffects);
    }


    private IEnumerator ApplyEffects(List<Effect> effects)
    {
        foreach (Effect effect in effects)
        {
            yield return StartCoroutine(effect.Trigger());
        }
    }
}