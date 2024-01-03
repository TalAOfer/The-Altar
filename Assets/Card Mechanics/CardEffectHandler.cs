using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectHandler : MonoBehaviour
{
    [SerializeField] private Card card;

    public List<Effect> BeforeBattleEffects = new();
    public List<Effect> AttackPointsAlterationEffects = new();
    public List<Effect> HurtPointsAlterationEffects = new();
    public List<Effect> OnSurviveEffects = new();
    public List<Effect> OnDeathEffects = new();

    public List<Effect> OnGainPointsEffects = new();

    public List<Effect> OnGlobalDeathEffects = new();
    public List<Effect> SupportEffects = new();
    
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

        foreach(EffectBlueprint effect in blueprint.OnSurvive)
        {
            effect.SpawnEffect(EffectTrigger.OnSurvive, card);  
        }

        foreach(EffectBlueprint effect in blueprint.OnGlobalDeath)
        {
            effect.SpawnEffect(EffectTrigger.OnGlobalDeath, card);  
        }

        foreach(EffectBlueprint effect in blueprint.Support)
        {
            effect.SpawnEffect(EffectTrigger.Support, card);    
        }
    }

    public IEnumerator RemoveCurrentEffects(ShapeshiftType shapeshiftType)
    {
        bool isPrebattle = shapeshiftType == ShapeshiftType.Prebattle;
        // Create lists to hold the effects to be removed
        List<Effect> beforeBattleToRemove = new(BeforeBattleEffects);
        List<Effect> onGainPointsToRemove = new(OnGainPointsEffects);
        List<Effect> onDeathToRemove = new(OnDeathEffects);
        List<Effect> hurtPointsToRemove = new(HurtPointsAlterationEffects);
        List<Effect> attackPointsToRemove = new(AttackPointsAlterationEffects);
        List<Effect> onSurviveToRemove = new(OnSurviveEffects);
        List<Effect> onGlobalDeathToRemove = new(OnGlobalDeathEffects);
        List<Effect> supportToRemove = new(SupportEffects);

        // Remove and destroy the before battle effects
        foreach (Effect effect in beforeBattleToRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            if (isPrebattle && effect.effectApplicationType == EffectApplicationType.ForBattle) continue;
            BeforeBattleEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        // Remove and destroy the on gain points effects
        foreach (Effect effect in onGainPointsToRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            if (isPrebattle && effect.effectApplicationType == EffectApplicationType.ForBattle) continue;
            OnGainPointsEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        // Remove and destroy the on death effects
        foreach (Effect effect in onDeathToRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            if (isPrebattle && effect.effectApplicationType == EffectApplicationType.ForBattle) continue;
            OnDeathEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        foreach (Effect effect in hurtPointsToRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            if (isPrebattle && effect.effectApplicationType == EffectApplicationType.ForBattle) continue;
            HurtPointsAlterationEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        foreach (Effect effect in attackPointsToRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            if (isPrebattle && effect.effectApplicationType == EffectApplicationType.ForBattle) continue;
            AttackPointsAlterationEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        foreach(Effect effect in onSurviveToRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            if (isPrebattle && effect.effectApplicationType == EffectApplicationType.ForBattle) continue;
            OnSurviveEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        foreach(Effect effect in onGlobalDeathToRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            if (isPrebattle && effect.effectApplicationType == EffectApplicationType.ForBattle) continue;
            OnGlobalDeathEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        foreach(Effect effect in supportToRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            if (isPrebattle && effect.effectApplicationType == EffectApplicationType.ForBattle) continue;
            SupportEffects.Remove(effect);
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

        List<Effect> onGainPointsToRemove = new(OnGainPointsEffects);

        foreach (Effect effect in onGainPointsToRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            OnGainPointsEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

    }

    public IEnumerator ApplyOnDeathEffects()
    {
        foreach (Effect effect in OnDeathEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(card, null, EffectTrigger.OnDeath)));


            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
        }

        List<Effect> onDeathToRemove = new(OnDeathEffects);

        // Remove and destroy the on death effects
        foreach (Effect effect in onDeathToRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            OnDeathEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        yield return null;
    }

    public IEnumerator ApplyBeforeBattleEffects(Card enemyCard)
    {
        foreach (Effect effect in BeforeBattleEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(card, enemyCard, EffectTrigger.BeforeBattle)));
        }

        List<Effect> beforeBattleToRemove = new(BeforeBattleEffects);

        foreach (Effect effect in beforeBattleToRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            BeforeBattleEffects.Remove(effect);
            Destroy(effect.gameObject);
        }

        yield return null;
    }

    public IEnumerator ApplyOnSurviveEffects(Card enemyCard)
    {
        foreach (Effect effect in OnSurviveEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(card, enemyCard, EffectTrigger.OnSurvive)));
        }

        List<Effect> onSurviveToRemove = new(OnSurviveEffects);

        foreach (Effect effect in onSurviveToRemove)
        {
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            OnSurviveEffects.Remove(effect);
            Destroy(effect.gameObject);
        }
    } 

    public void TriggerOnGlobalDeathEffects()
    {
        StartCoroutine(ApplyOnGlobalDeathEffects());
    }
    private IEnumerator ApplyOnGlobalDeathEffects()
    {
        foreach (Effect effect in OnGlobalDeathEffects)
        {
            //Apply
            yield return StartCoroutine(effect.Apply(new EffectContext(card, null, EffectTrigger.OnGlobalDeath)));
            
            //Remove upon application
            if (effect.effectApplicationType == EffectApplicationType.Persistent) continue;
            OnGlobalDeathEffects.Remove(effect);
            Destroy(effect.gameObject);
        }
    }

    public IEnumerator ApplySupportEffects(Card battlingCard)
    {
        foreach (Effect effect in SupportEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(card, battlingCard, EffectTrigger.Support)));
        }
    }
}
