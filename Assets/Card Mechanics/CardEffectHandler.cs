using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectHandler : MonoBehaviour
{
    [SerializeField] private Card card;

    public List<Effect> StartOfBattleEffects = new();
    public List<Effect> BeforeAttackingEffects = new();
    public List<Effect> OnSurviveEffects = new();
    public List<Effect> OnDeathEffects = new();

    public List<Effect> OnGainPointsEffects = new();
    public List<Effect> OnGlobalDeathEffects = new();
    public List<Effect> SupportEffects = new();
    public List<Effect> OnActionTakenEffects = new();
    public List<Effect> OnObtainEffects = new();
    public List<Effect> OnSacrificeEffects = new();
    public List<Effect> StartOfTurnEffects = new();
    public List<Effect> EndOfTurnEffects = new();

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

        foreach (EffectBlueprint effect in blueprint.OnGainPoints)
        {
            effect.SpawnEffect(EffectTrigger.OnGainPoints, card);
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

        foreach (EffectBlueprint effect in blueprint.OnActionTaken)
        {
            effect.SpawnEffect(EffectTrigger.OnActionTaken, card);
        }

        foreach (EffectBlueprint effect in blueprint.OnObtain)
        {
            effect.SpawnEffect(EffectTrigger.OnObtain, card);
        }

        foreach (EffectBlueprint effect in blueprint.OnSacrifice)
        {
            effect.SpawnEffect(EffectTrigger.OnSacrifice, card);
        }

        foreach (EffectBlueprint effect in blueprint.StartOfTurn)
        {
            effect.SpawnEffect(EffectTrigger.StartOfTurn, card);
        }

        foreach (EffectBlueprint effect in blueprint.EndOfTurn)
        {
            effect.SpawnEffect(EffectTrigger.EndOfTurn, card);
        }

    }

    public IEnumerator RemoveCurrentEffects()
    {
        RemoveEffects(StartOfBattleEffects);
        RemoveEffects(BeforeAttackingEffects);
        RemoveEffects(OnSurviveEffects);
        RemoveEffects(OnDeathEffects);
        RemoveEffects(OnGainPointsEffects);
        RemoveEffects(OnGlobalDeathEffects);
        RemoveEffects(SupportEffects);
        RemoveEffects(OnActionTakenEffects);
        RemoveEffects(OnObtainEffects);
        RemoveEffects(OnSacrificeEffects);
        RemoveEffects(StartOfTurnEffects);
        RemoveEffects(EndOfTurnEffects);

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
        yield return ApplyEffects(StartOfBattleEffects, otherCard, EffectTrigger.StartOfBattle);
    }

    public IEnumerator ApplyBeforeAttackingEffects(Card otherCard)
    {
        yield return ApplyEffects(BeforeAttackingEffects, otherCard, EffectTrigger.BeforeAttacking);
    }

    public IEnumerator ApplyOnSurviveEffects(Card otherCard)
    {
        yield return ApplyEffects(OnSurviveEffects, otherCard, EffectTrigger.OnSurvive);
    }

    public IEnumerator ApplyOnDeathEffects(Card killingCard)
    {
        yield return ApplyEffects(OnDeathEffects, killingCard, EffectTrigger.OnDeath);
    }

    public IEnumerator ApplyOnGainPointsEffects()
    {
        yield return ApplyEffects(OnGainPointsEffects, null, EffectTrigger.OnGainPoints);
    }

    public IEnumerator ApplyOnGlobalDeathEffects()
    {
        yield return ApplyEffects(OnGlobalDeathEffects, null, EffectTrigger.OnGlobalDeath);
    }

    public IEnumerator ApplySupportEffects(Card otherCard)
    {
        yield return ApplyEffects(SupportEffects, otherCard, EffectTrigger.Support);
    }

    public IEnumerator ApplyOnActionTakenEffects()
    {
        yield return ApplyEffects(OnActionTakenEffects, null, EffectTrigger.OnActionTaken);
    }

    public IEnumerator ApplyOnObtainEffects()
    {
        yield return ApplyEffects(OnObtainEffects, null, EffectTrigger.OnObtain);
        RemoveEffects(OnObtainEffects);
    }

    public IEnumerator ApplyOnSacrificeEffects()
    {
        yield return ApplyEffects(OnSacrificeEffects, null, EffectTrigger.OnSacrifice);
    }

    public IEnumerator ApplyStartOfTurnEffects()
    {
        yield return ApplyEffects(StartOfTurnEffects, null, EffectTrigger.StartOfTurn);
        RemoveEffects(StartOfTurnEffects);
    }

    public IEnumerator ApplyEndOfTurnEffects()
    {
        yield return ApplyEffects(EndOfTurnEffects, null, EffectTrigger.StartOfTurn);
        RemoveEffects(StartOfTurnEffects);
    }

    private IEnumerator ApplyEffects(List<Effect> effects, Card otherCard, EffectTrigger trigger)
    {
        foreach (Effect effect in effects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(card, otherCard)));
        }
    }
}