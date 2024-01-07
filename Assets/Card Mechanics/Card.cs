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
    public BattlePoint hurtPoints;

    [FoldoutGroup("Card Info")]
    public BattlePoint attackPoints;

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

    public List<Guardian> guardians = new();

    private ShapeshiftLock shapeshiftLock;

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

        attackPoints = new BattlePoint(points, BattlePointType.Attack);
        hurtPoints = new BattlePoint(0, BattlePointType.Hurt);
    }

    public IEnumerator CalcAttackPoints()
    {
        attackPoints.value = points;
        yield return StartCoroutine(CalcPoints(attackPoints, points));
    }

    public void ToggleDamageVisual(bool enable)
    {
        if (enable)
        {
            visualHandler.EnableDamageVisual(attackPoints.value);
        } 
        
        else
        {
            visualHandler.DisableDamageVisual();
        }

    }

    public IEnumerator CalcHurtPoints(int damagePoints)
    {
        hurtPoints.value = 0;
        yield return StartCoroutine(CalcPoints(hurtPoints, damagePoints));
    }

    public IEnumerator CalcPoints(BattlePoint battlePoint, int startingValue)
    {
        int calcValue = startingValue;

        List<BattlePointModifier> modifierList = battlePoint.type == BattlePointType.Attack ? attackPointsModifiers : hurtPointsModifiers;

        modifierList.Sort((a, b) => a.modifierType.CompareTo(b.modifierType));

        //string listName = battlePoint.type == BattlePointType.Attack ? "attackPointsModifiers" : "hurtPointsModifiers";
        //Debug.Log(gameObject.name + " has " + modifierList.Count.ToString() + " in " + listName);

        foreach (BattlePointModifier modifier in modifierList)
        {
            //Debug.Log("Was: " + calcValue);
            calcValue = modifier.Apply(calcValue);
            //Debug.Log("Now is: " + calcValue.ToString());
        }

        battlePoint.value = calcValue;
        yield return null;
    }


    public void TakeDamage()
    {
        if (guardians.Count == 0)
        {
            points -= hurtPoints.value;
            if (points < 0)
            {
                points = 0;
            }
        }

        else
        {
            DealWithGuardians();
        }

        visualHandler.UpdateNumberSprite();
    }

    private void DealWithGuardians()
    {
        guardians.Sort((a, b) => a.guardianType.CompareTo(b.guardianType));

        List<Guardian> guardiansToRemove = new();

        foreach (var guardian in guardians)
        {
            hurtPoints.value = (guardian.ApplyAndGetRestOfDamage(hurtPoints.value, points));
            points -= hurtPoints.value;
            if (guardian.applicationType != EffectApplicationType.Persistent)
            {
                guardiansToRemove.Add(guardian);
            }

            if (points <= 0)
            {
                // DEAL WITH DEATH
                break;
            }
        }

        foreach (var guardian in guardiansToRemove)
        {
            guardians.Remove(guardian);
        }
    }

    public void TakeDirectDamage(int damage)
    {
        points -= damage;
        if (points < 0) points = 0;
        visualHandler.UpdateNumberSprite();
        StartCoroutine(HandleShapeshift());
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

    public IEnumerator HandleShapeshift()
    {
        if (!ShouldShapeshift()) yield break;
        else yield return StartCoroutine(Shapeshift());
    }

    public IEnumerator Shapeshift()
    {
        CardBlueprint newForm = shapeshiftHelper.GetCardBlueprint(cardOwner, points, cardColor);
        currentArchetype = newForm;
        visualHandler.SetNewCardVisual(newForm);
        gameObject.name = newForm.name;
        yield return StartCoroutine(effects.RemoveCurrentEffects());
        ResetPointAlterations();
        effects.SpawnEffects(newForm);
        yield return new WaitForSeconds(1);
    }

    private void ResetPointAlterations()
    {
        attackPointsModifiers.Clear();
        hurtPointsModifiers.Clear();
    }

    public IEnumerator ForceShapeshift(CardBlueprint blueprint)
    {
        points = blueprint.defaultPoints;
        cardColor = blueprint.cardColor;
        visualHandler.UpdateNumberSprite();
        yield return Shapeshift();
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
    Battle,
    Choosable
}

public class ShapeshiftLock
{
    int threshold;
}