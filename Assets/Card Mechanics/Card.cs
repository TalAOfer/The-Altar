using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [FoldoutGroup("Dependencies")]
    public AllEvents events;

    private BlueprintPoolInstance pool;

    [FoldoutGroup("Child Components")]
    public CardEffectHandler effects;
    [FoldoutGroup("Child Components")]
    public CardVisualHandler visualHandler;
    [FoldoutGroup("Child Components")]
    public CardInteractionHandler interactionHandler;

    [FoldoutGroup("Card Info")]
    public int points;

    [FoldoutGroup("Card Info")]
    public int index;

    [FoldoutGroup("Card Info")]
    public CardColor cardColor;

    [FoldoutGroup("Card Info")]
    public CardOwner cardOwner;

    [FoldoutGroup("Card Info")]
    public CardState cardState;

    [FoldoutGroup("Card Info")]
    public CardBlueprint currentOverride;

    [FoldoutGroup("Battle Points")]
    public BattlePoint attackPoints;

    [FoldoutGroup("Battle Points")]
    public BattlePoint hurtPoints;

    [FoldoutGroup("Battle Points")]
    public List<BattlePointModifier> attackPointsModifiers = new();

    [FoldoutGroup("Battle Points")]
    public List<BattlePointModifier> hurtPointsModifiers = new();

    [FoldoutGroup("Battle Points")]
    public List<Guardian> guardians = new();

    private HigherBeing higherBeing;

    public bool IsDead
    {
        get { return points <= 0; }
    }

    public void Init(BlueprintPoolInstance pool, CardBlueprint blueprint, string startingSortingLayer)
    {
        this.pool = pool;

        currentOverride = blueprint;
        SetCardColor(blueprint.cardColor);
        cardOwner = blueprint.cardOwner;
        points = blueprint.defaultPoints;

        higherBeing = new HigherBeing(blueprint.higherBeing, 0);

        attackPoints = new BattlePoint(points, BattlePointType.Attack);
        hurtPoints = new BattlePoint(0, BattlePointType.Hurt);

        effects.Init(blueprint);

        interactionHandler.Initialize();
        visualHandler.Init(blueprint, startingSortingLayer);

    }

    public CardBlueprint GetCurrentOverride()
    {
        if (higherBeing.isLocked && !IsDead) return currentOverride;
        return pool.GetCardOverride(new CardArchetype(points, cardColor));
    }

    public void SetCardColor(CardColor newColor)
    {
        cardColor = newColor;
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

        string log = currentOverride.cardName + "'s " + battlePoint.type.ToString().ToLower() + "points are " + calcValue.ToString();
        events.AddLogEntry.Raise(this, log);

        foreach (BattlePointModifier modifier in modifierList)
        {
            calcValue = modifier.Apply(calcValue);
            log = currentOverride.cardName + "'s " + battlePoint.type.ToString().ToLower() + "points are " + calcValue.ToString();
        }

        battlePoint.value = calcValue;
        yield return null;
    }


    public void TakeDamage(Component sender)
    {

        if (guardians.Count == 0)
        {
            points -= hurtPoints.value;
            visualHandler.SpawnFallingDamage(hurtPoints.value);
            if (points < 0)
            {
                points = 0;
            }
        }

        else
        {
            DealWithGuardians();
        }

        visualHandler.InitiateParticleSplash();
        visualHandler.SetNumberSprites();
    }

    private void DealWithGuardians()
    {
        guardians.Sort((a, b) => a.guardianType.CompareTo(b.guardianType));

        List<Guardian> guardiansToRemove = new();

        int damageLeft = hurtPoints.value;

        foreach (var guardian in guardians)
        {
            damageLeft = (guardian.ApplyAndGetRestOfDamage(hurtPoints.value, points));
            if (guardian.applicationType != EffectApplicationType.Persistent)
            {
                guardiansToRemove.Add(guardian);
            }
        }

        points -= damageLeft;
        visualHandler.SpawnFallingDamage(damageLeft);

        foreach (var guardian in guardiansToRemove)
        {
            guardians.Remove(guardian);
        }
    }

    public void TakeDirectDamage(int damage)
    {
        points -= damage;
        if (points < 0) points = 0;
        visualHandler.SetNumberSprites();
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
        return currentOverride != GetCurrentOverride();
    }

    public IEnumerator HandleShapeshift()
    {
        if (!ShouldShapeshift()) yield break;
        else yield return StartCoroutine(Shapeshift());
    }

    public IEnumerator Shapeshift()
    {
        CardBlueprint newForm = GetCurrentOverride();

        if (currentOverride != newForm)
        {
            currentOverride = newForm;
        }

        if (IsDead)
        {
            yield return StartCoroutine(visualHandler.ToggleOverallVanish(true));
            yield break;
        }


        yield return visualHandler.ToggleSpritesVanish(true);
        visualHandler.SetNewCardVisual();
        gameObject.name = newForm.name;
        higherBeing.isLocked = newForm.higherBeing;
        yield return StartCoroutine(effects.RemoveCurrentEffects());
        ResetPointAlterations();
        effects.SpawnEffects(newForm);
        yield return visualHandler.ToggleSpritesVanish(false);

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
        yield return Shapeshift();
    }

    public void ChangeCardState(CardState newState)
    {
        if (newState == cardState)
        {
            Debug.LogWarning("Trying to switch to same state");
            return;
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
            case CardState.Selectable:
                visualHandler.SetSortingLayer(GameConstants.BOTTOM_BATTLE_LAYER);
                break;
            case CardState.Selected:
                visualHandler.SetSortingLayer(GameConstants.TOP_BATTLE_LAYER);
                break;
            case CardState.Selecting:
                visualHandler.SetSortingLayer(GameConstants.TOP_BATTLE_LAYER);
                break;
        }
    }
}

public enum CardState
{
    Default,
    Battle,
    Selectable,
    Selected,
    Selecting
}

public class HigherBeing
{
    public bool isLocked;
    public int threshold;

    public HigherBeing(bool isLocked, int threshold)
    {
        this.isLocked = isLocked;
        this.threshold = threshold;
    }
}