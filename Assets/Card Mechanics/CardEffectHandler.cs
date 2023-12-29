using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectHandler : MonoBehaviour
{
    [SerializeField] private Card card;

    public List<Effect> BeforeBattleEffects = new();
    public List<Effect> OnGainPointsEffects = new();
    public List<Effect> OnDeathEffects = new();

    public List<Effect> HurtPointsAlterationEffects = new();
    public List<Effect> AttackPointsAlterationEffects = new();

    public void Init(CardBlueprint blueprint)
    {
        SpawnEffects(blueprint);
    } 

    public void SpawnEffects(CardBlueprint blueprint)
    {
        foreach (EffectBlueprint effect in blueprint.BeforeBattle)
        {
            effect.SpawnEffect(EffectTrigger.BeforeBattle, card);
        }

        foreach (EffectBlueprint effect in blueprint.OnGainPoints)
        {
            effect.SpawnEffect(EffectTrigger.OnGainPoints, card);
        }

        foreach (EffectBlueprint effect in blueprint.OnDeath)
        {
            effect.SpawnEffect(EffectTrigger.OnDeath, card);
        }

        foreach (EffectBlueprint effect in blueprint.HurtPointsAlteration)
        {
            effect.SpawnEffect(EffectTrigger.HurtPointsAlteration, card);
        }

        foreach (EffectBlueprint effect in blueprint.AttackPointsAlteration)
        {
            effect.SpawnEffect(EffectTrigger.AttackPointsAlteration, card);
        }
    }

    public IEnumerator RemoveCurrentEffects()
    {
        // Create lists to hold the effects to be removed
        List<Effect> beforeBattleToRemove = new(BeforeBattleEffects);
        List<Effect> onGainPointsToRemove = new(OnGainPointsEffects);
        List<Effect> onDeathToRemove = new(OnDeathEffects);
        List<Effect> hurtPointsToRemove = new(HurtPointsAlterationEffects);
        List<Effect> attackPointsToRemove = new(AttackPointsAlterationEffects);
        // Remove and destroy the before battle effects
        foreach (Effect effect in beforeBattleToRemove)
        {
            BeforeBattleEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        // Remove and destroy the on gain points effects
        foreach (Effect effect in onGainPointsToRemove)
        {
            OnGainPointsEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        // Remove and destroy the on death effects
        foreach (Effect effect in onDeathToRemove)
        {
            OnDeathEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        foreach (Effect effect in hurtPointsToRemove)
        {
            HurtPointsAlterationEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        foreach (Effect effect in attackPointsToRemove)
        {
            AttackPointsAlterationEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        // Wait for the next fixed frame update to ensure all Destroy calls have been processed
        yield return new WaitForFixedUpdate();
    }

    public IEnumerator ApplyOnGainPointsEffects()
    {
        foreach (Effect effect in OnGainPointsEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(card, null, EffectTrigger.OnGainPoints)));
        }
    }

    public IEnumerator ApplyOnDeathEffects()
    {
        foreach (Effect effect in OnDeathEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(card, null, EffectTrigger.OnDeath)));
        }

        if (card.points != 0) StartCoroutine(card.Shapeshift());
        card.SetIsDead(card.points == 0);
        yield return null;
    }

    public IEnumerator ApplyBeforeBattleEffects(Card enemyCard)
    {
        foreach (Effect effect in BeforeBattleEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(card, enemyCard, EffectTrigger.BeforeBattle)));
        }
    }
}