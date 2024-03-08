using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private Codex codex;
    public BattleRoomDataProvider DataProvider {  get; private set; }
    public CardBase cardBase {  get; private set; }

    [FoldoutGroup("Child Components")]
    public CardEffectHandler effects;
    [FoldoutGroup("Child Components")]
    public CardVisualHandler visualHandler;
    [FoldoutGroup("Child Components")]
    public CardMovementHandler movement;
    [FoldoutGroup("Child Components")]
    public InteractionEventEmitter interactionHandler;

    [FoldoutGroup("Card Info")]
    public int points;

    [FoldoutGroup("Card Info")]
    public int index;

    [FoldoutGroup("Card Info")]
    public CardColor cardColor;

    [FoldoutGroup("Card Info")]
    public Affinity Affinity { get; private set; }

    [FoldoutGroup("Card Info")]
    public CardState cardState;

    [FoldoutGroup("Card Info")]
    public CardInteractionType cardInteractionType;

    [FoldoutGroup("Card Info")]
    public CardBlueprint Mask;

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

    private HigherBeing higherBeing = new(false, 0);

    public bool IsDead
    {
        get { return points <= 0; }
    }

    public void Init(Codex codex, CardBlueprint blueprint, string startingSortingLayer, CardInteractionType cardInteractionType, BattleRoomDataProvider dataProvider)
    {
        this.codex = codex;
        this.cardInteractionType = cardInteractionType;
        DataProvider = dataProvider;

        Mask = blueprint;
        SetCardColor(blueprint.Archetype.color);
        Affinity = blueprint.Affinity;
        points = blueprint.Archetype.points;

        higherBeing = new HigherBeing(blueprint.SpecialEffects.HasFlag(SpecialEffects.HigherBeing), 0);

        attackPoints = new BattlePoint(points, BattlePointType.Attack);
        hurtPoints = new BattlePoint(0, BattlePointType.Hurt);

        effects.Init(blueprint);

        visualHandler.Init(blueprint, startingSortingLayer);

    }

    public CardBlueprint GetCurrentMask()
    {
        if (higherBeing.isLocked && !IsDead) return Mask;
        return codex.GetCardOverride(new CardArchetype(points, cardColor));
    }

    public void SetCardColor(CardColor newColor)
    {
        if (higherBeing.isLocked) return;
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

        string log = Mask.cardName + "'s " + battlePoint.type.ToString().ToLower() + "points are " + calcValue.ToString();
        Locator.Events.AddLogEntry.Raise(this, log);

        foreach (BattlePointModifier modifier in modifierList)
        {
            calcValue = modifier.Apply(calcValue);
            log = Mask.cardName + "'s " + battlePoint.type.ToString().ToLower() + "points are " + calcValue.ToString();
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

    public void GainPoints(int pointsToGain)
    {
        points += pointsToGain;
        if (points < 0) points = 0;
        else if (points > 10) points = 10;
        visualHandler.SetNumberSprites();
    }

    public bool ShouldShapeshift()
    {
        return !IsDead && Mask != GetCurrentMask();
    }

    public IEnumerator HandleShapeshift()
    {
        if (!ShouldShapeshift()) yield break;
        else yield return StartCoroutine(Shapeshift());
    }

    public IEnumerator Shapeshift()
    {
        if (IsDead)
        {
            yield return StartCoroutine(visualHandler.ToggleOverallVanish(true));
            yield break;
        }

        CardBlueprint newMask = GetCurrentMask();

        if (Mask != newMask)
        {
            Mask = newMask;
        }

        Tools.PlaySound("Shapeshift", transform);
        yield return visualHandler.ToggleSpritesVanish(true);
        visualHandler.SetNewCardVisual();
        gameObject.name = newMask.name;
        higherBeing.isLocked = newMask.SpecialEffects.HasFlag(SpecialEffects.HigherBeing);
        yield return StartCoroutine(effects.RemoveAllEffects());
        ResetPointAlterations();
        effects.InstantiateDefaultCardEffects(newMask);
        yield return visualHandler.ToggleSpritesVanish(false);

    }

    private void ResetPointAlterations()
    {
        attackPointsModifiers.Clear();
        hurtPointsModifiers.Clear();
    }

    public IEnumerator ForceShapeshift(CardBlueprint blueprint)
    {
        points = blueprint.Archetype.points;
        cardColor = blueprint.Archetype.color;
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
    }
}

public enum CardState
{
    Default,
    Selected, 
    Battle,
    Draw,
}

public enum CardInteractionType
{
    Playable,
    Selection,
    Codex
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