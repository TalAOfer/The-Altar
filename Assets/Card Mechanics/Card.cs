using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public bool isDead { get; private set; }

    public int points { get; set; }
    public int hurtPoints { get; set; }
    public int attackPoints { get; set; }
    public int index { get; set; }

    public CardColor cardColor { get; set; }
    public CardState cardState { get; set; }

    public CardEffectHandler effects;
    public CardVisualHandler visualHandler;
    public CardInteractionHandler interactionHandler;

    
    [SerializeField] private ShapeshiftHelper shapeshiftHelper;


    public void Init(CardBlueprint blueprint, CardState cardState, int index, string startingSortingLayer)
    {
        this.cardState = cardState;
        this.index = index;
        points = blueprint.defaultPoints;

        effects.Init(blueprint);
        visualHandler.Init(blueprint, startingSortingLayer);
    }

    public IEnumerator CalcAttackPoints()
    {
        attackPoints = points;

        foreach (Effect effect in effects.AttackPointsAlterationEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(this, null, EffectTrigger.AttackPointsAlteration)));
        }

        yield return null;
    }

    public IEnumerator CalcHurtPoints(int damagePoints)
    {
        hurtPoints = damagePoints;

        foreach (Effect effect in effects.HurtPointsAlterationEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(this, null, EffectTrigger.HurtPointsAlteration)));
        }

        yield return null;
    }

    public void TakeDamage()
    {
        points -= hurtPoints;
        if (points < 0) points = 0;
        visualHandler.UpdateNumberSprite();
    }

    public IEnumerator GainPoints(int pointsToGain)
    {
        points += pointsToGain;
        if (points < 0) points = 0;
        else if (points > 10) points = 10;
        visualHandler.UpdateNumberSprite();

        yield return StartCoroutine(effects.ApplyOnGainPointsEffects());
    }

    public IEnumerator HandleShapeshift()
    {
        if (points == 0) yield return StartCoroutine(TurnToBones());
        else yield return StartCoroutine(Shapeshift());
    }

    public IEnumerator TurnToBones()
    {
        CardBlueprint newForm = shapeshiftHelper.GetCardBlueprint(points, cardColor);
        visualHandler.SetNewCardVisual(newForm);
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(effects.ApplyOnDeathEffects());
    }

    public IEnumerator Shapeshift()
    {
        CardBlueprint newForm = shapeshiftHelper.GetCardBlueprint(points, cardColor);
        visualHandler.SetNewCardVisual(newForm);
        yield return StartCoroutine(effects.RemoveCurrentEffects());
        effects.SpawnEffects(newForm);
        yield return new WaitForSeconds(1);
    }

    public void SetIsDead(bool isDead) => this.isDead = isDead;
}

public enum CardState
{
    Reward,
    Enemy,
    Hand
}
