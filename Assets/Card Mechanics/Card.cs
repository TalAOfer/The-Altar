using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private Codex codex;
    public int SpawnIndex {  get; private set; }
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
    public List<BattleAmountModifier> attackPointsModifiers = new();

    [FoldoutGroup("Battle Points")]
    public List<BattleAmountModifier> hurtPointsModifiers = new();

    [FoldoutGroup("Battle Points")]
    public List<Guardian> guardians = new();

    private HigherBeing higherBeing = new(false, 0);

    public int Armor { get; private set; } = 0;

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

        effects.Init(blueprint);

        visualHandler.Init(blueprint, startingSortingLayer);

        //StartCoroutine(effects.ApplyEffects(TriggerType.OnChange));
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

    public void ToggleDamageVisual(bool enable)
    {
        if (enable)
        {
            visualHandler.EnableDamageVisual();
        }

        else
        {
            visualHandler.DisableDamageVisual();
        }

    }

    public void CalculateDamage(Card target = null)
    {
        int calcValue = points;

        List<BattleAmountModifier> modifierList = new();

        modifierList = new List<BattleAmountModifier>(attackPointsModifiers);

        if (target != null)
        {
            modifierList.AddRange(target.hurtPointsModifiers);
        }

        modifierList.Sort((a, b) => a.ModifierType.CompareTo(b.ModifierType));

        foreach (BattleAmountModifier modifier in modifierList)
        {
            Card opponent = modifier.PointType is BattlePointType.Attack ? target : this;

            calcValue = modifier.Apply(calcValue, opponent);
        }

        attackPoints.value = calcValue;
    }



    public int TakeDamage(int incomingDamage)
    {
        int damage = incomingDamage;

        damage = MitigateWithArmor(damage);

        damage = DealWithGuardians(damage);

        points -= damage;
        visualHandler.SpawnFallingDamage(damage);

        if (points < 0)
        {
            points = 0;
        }

        visualHandler.InitiateParticleSplash();
        visualHandler.SetNumberSprites();

        return damage;
    }


    public void TakeDirectDamage(int damage)
    {
        points -= damage;
        if (points < 0) points = 0;
        visualHandler.SetNumberSprites();
    }

    public void GainArmor(int amount)
    {
        Armor += amount;

        visualHandler.HandleArmorVisual();
    }

    private int MitigateWithArmor(int damage)
    {
        int damageLeft;
        damageLeft = Mathf.Max(damage - Armor, 0);
        Armor = Mathf.Max(Armor - damage, 0);
        visualHandler.HandleArmorVisual();
        return damageLeft;
    }

    private int DealWithGuardians(int damage)
    {
        if (guardians.Count == 0) return damage;

        guardians.Sort((a, b) => a.guardianType.CompareTo(b.guardianType));

        List<Guardian> guardiansToRemove = new();

        foreach (var guardian in guardians)
        {
            damage = (guardian.ApplyAndGetRestOfDamage(damage, points));
            if (guardian.applicationType != EffectApplicationType.Persistent)
            {
                guardiansToRemove.Add(guardian);
            }
        }

        foreach (var guardian in guardiansToRemove)
        {
            guardians.Remove(guardian);
        }

        return damage;
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
            StartCoroutine(DestroySelf());
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

        //yield return effects.ApplyEffects(TriggerType.OnChange);

    }

    private IEnumerator DestroySelf()
    {
        DataProvider.RemovePlayerCard(this);
        DOTween.Kill(transform);
        yield return Tools.GetWait(0.25f);
        gameObject.SetActive(false);
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