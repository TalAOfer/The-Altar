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

    public CardBlueprint currentArchetype;
    public CardEffectHandler effects;
    public CardVisualHandler visualHandler;
    public CardInteractionHandler interactionHandler;

    public List<BattlePointModifier> attackPointsModifiers = new();
    public List<BattlePointModifier> hurtPointsModifiers = new();

    public AllEvents events;
    [SerializeField] private ShapeshiftHelper shapeshiftHelper;
    public bool IsDead
    {
        get { return points <= 0; }
    }

    public void Init(CardBlueprint blueprint, int index, string startingSortingLayer)
    {
        currentArchetype = blueprint;

        cardOwner = blueprint.cardOwner;
        this.index = index;

        points = blueprint.defaultPoints;
        effects.Init(blueprint);
        interactionHandler.Initialize();
        visualHandler.Init(blueprint, startingSortingLayer);
    }

    public IEnumerator CalcAttackPoints(Card otherCard)
    {
        attackPoints = points;

        foreach (Effect effect in effects.AttackPointsAlterationEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(this, otherCard, EffectTrigger.AttackPointsAlteration)));
        }

        yield return null;
    }

    public IEnumerator CalcPoints(int startingValue, List<BattlePointModifier> modifierList)
    {
        int value = startingValue;
        modifierList.Sort((a, b) => a.modifierType.CompareTo(b.modifierType));

        foreach (BattlePointModifier modifier in attackPointsModifiers)
        {
            value = modifier.Apply(value);
        }

        attackPoints = value;
        yield return null;
    }

    public IEnumerator CalcHurtPoints(Card otherCard, int damagePoints)
    {
        hurtPoints = damagePoints;

        foreach (Effect effect in effects.HurtPointsAlterationEffects)
        {
            yield return StartCoroutine(effect.Apply(new EffectContext(this, otherCard, EffectTrigger.HurtPointsAlteration)));
        }

        yield return null;
    }


    public void TakeDamage()
    {
        points -= hurtPoints;
        if (points < 0) points = 0;
        visualHandler.UpdateNumberSprite();
        
        //Reset attackpoints after battle
        attackPoints = 0;
        hurtPoints = 0;
    }

    public IEnumerator GainPoints(int pointsToGain)
    {
        points += pointsToGain;
        if (points < 0) points = 0;
        else if (points > 10) points = 10;

        yield return StartCoroutine(effects.ApplyOnGainPointsEffects());
    }

    public bool ShouldShapeshift()
    {
        return currentArchetype != shapeshiftHelper.GetCardBlueprint(cardOwner, points, cardColor);
    }

    public IEnumerator HandleShapeshift(ShapeshiftType shapeshiftType)
    {
        if (!ShouldShapeshift()) yield break;
        else yield return StartCoroutine(Shapeshift(shapeshiftType));
    }

    public IEnumerator Shapeshift(ShapeshiftType shapeshiftType)
    {
        CardBlueprint newForm = shapeshiftHelper.GetCardBlueprint(cardOwner,points, cardColor);
        currentArchetype = newForm;
        visualHandler.SetNewCardVisual(newForm);
        gameObject.name = newForm.name;
        yield return StartCoroutine(effects.RemoveCurrentEffects(shapeshiftType));
        effects.SpawnEffects(newForm);
        yield return new WaitForSeconds(1);
    }

    public IEnumerator ForceShapeshift(CardBlueprint blueprint, ShapeshiftType shapeshiftType)
    {
        points = blueprint.defaultPoints;
        cardColor = blueprint.cardColor;
        visualHandler.UpdateNumberSprite();
        yield return Shapeshift(shapeshiftType);
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

public enum ShapeshiftType
{
    OutOfBattle,
    Prebattle,
    Postbattle
}
