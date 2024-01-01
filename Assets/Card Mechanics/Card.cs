using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [FoldoutGroup("Card Info")]
    public int points;

    [FoldoutGroup("Card Info")]
    public int hurtPoints;

    [FoldoutGroup("Card Info")]
    public int attackPoints;

    [FoldoutGroup("Card Info")]
    public int index;

    [FoldoutGroup("Card Info")]
    public CardColor cardColor;

    [FoldoutGroup("Card Info")]
    public CardOwner cardOwner;

    [FoldoutGroup("Card Info")]
    public CardState cardState;


    public CardEffectHandler effects;
    public CardVisualHandler visualHandler;
    public CardInteractionHandler interactionHandler;

    public AllEvents events;
    [SerializeField] private ShapeshiftHelper shapeshiftHelper;
    public bool IsDead
    {
        get { return points <= 0; }
    }


    public void Init(CardBlueprint blueprint, int index, string startingSortingLayer)
    {
        cardOwner = blueprint.cardOwner;
        this.index = index;

        points = blueprint.defaultPoints;
        effects.Init(blueprint);
        interactionHandler.Initialize();
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

    public IEnumerator GainPoints(int pointsToGain, bool initiateHandleShapeshift)
    {
        points += pointsToGain;
        if (points < 0) points = 0;
        else if (points > 10) points = 10;
        visualHandler.UpdateNumberSprite();

        yield return StartCoroutine(effects.ApplyOnGainPointsEffects());

        if (initiateHandleShapeshift)
        {
            yield return StartCoroutine(HandleShapeshift());
        }
    }

    public IEnumerator HandleShapeshift()
    {
        if (IsDead) yield return StartCoroutine(TurnToBones());
        else yield return StartCoroutine(Shapeshift());
    }

    public IEnumerator TurnToBones()
    {
        CardBlueprint newForm = shapeshiftHelper.GetCardBlueprint(cardOwner, points, cardColor);
        visualHandler.SetNewCardVisual(newForm);
        gameObject.name = newForm.name;
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(effects.ApplyOnDeathEffects());
    }

    public IEnumerator Shapeshift()
    {
        CardBlueprint newForm = shapeshiftHelper.GetCardBlueprint(cardOwner,points, cardColor);
        visualHandler.SetNewCardVisual(newForm);
        gameObject.name = newForm.name;
        yield return StartCoroutine(effects.RemoveCurrentEffects());
        effects.SpawnEffects(newForm);
        yield return new WaitForSeconds(1);
    }

    public IEnumerator ForceShapeshift(CardBlueprint blueprint)
    {
        visualHandler.SetNewCardVisual(blueprint);
        points = blueprint.defaultPoints;
        gameObject.name = blueprint.name;
        yield return StartCoroutine(effects.RemoveCurrentEffects());
        effects.SpawnEffects(blueprint);
    }

    public void Die()
    {
        events.OnGlobalCardDeath.Raise(this, this);
        gameObject.SetActive(false);
    }

    public IEnumerator ChangeCardState(CardState newState)
    {
        if (newState == cardState)
        {
            Debug.LogWarning("Trying to switch to same state");
            yield break;
        } 
        
        else
        {
            cardState = newState;
        }

        switch (newState)
        {
            case CardState.Default:
                if (cardOwner == CardOwner.Player)
                {
                    visualHandler.SetSortingLayer(GameConstants.HAND_LAYER);
                }
                else
                {
                    visualHandler.SetSortingLayer(GameConstants.TOP_MAP_LAYER);
                }
                break;
            case CardState.Battle:
                if (cardOwner == CardOwner.Player)
                {
                    visualHandler.SetSortingLayer(GameConstants.TOP_BATTLE_LAYER);
                }
                else
                {
                    visualHandler.SetSortingLayer(GameConstants.BOTTOM_BATTLE_LAYER);
                }
                break;
        }

        yield return null;
    }
}

public enum CardState
{
    Default,
    Battle
}
