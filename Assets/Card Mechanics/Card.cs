using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private Codex codex;
    public int SpawnIndex { get; private set; }
    public BattleRoomDataProvider DataProvider { get; private set; }
    private EventRegistry _events;

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

    public bool HasTakenDamageThisTurn { get; private set; }
    public bool Targetable { get; set; } = true;
    public bool Taunt;
    public int Armor { get; private set; } = 0;
    public int Might { get; private set; } = 0;

    public bool FATALLY_WOUNDED
    {
        get { return points <= 0; }
    }
    public bool PENDING_DESTRUCTION
    {
        get { return FATALLY_WOUNDED && !DESTROYING; }
    }

    public bool DESTROYING { get; private set; } = false;

    public void Init(Codex codex, CardBlueprint blueprint, string startingSortingLayer, CardInteractionType cardInteractionType, BattleRoomDataProvider dataProvider)
    {
        this.codex = codex;
        this.cardInteractionType = cardInteractionType;
        DataProvider = dataProvider;

        Mask = blueprint;
        SetCardColor(blueprint.Archetype.color);
        Affinity = blueprint.Affinity;
        points = blueprint.Archetype.points;

        attackPoints = new BattlePoint(points, BattlePointType.Attack);

        visualHandler.Init(startingSortingLayer, cardInteractionType);

        OnChange();

        _events = Locator.Events;
    }

    public void OnChange()
    {
        gameObject.name = Mask.name;

        higherBeing = new HigherBeing(Mask.SpecialEffects.HasFlag(SpecialEffects.HigherBeing), 0);
        Taunt = Mask.SpecialEffects.HasFlag(SpecialEffects.Taunt);

        effects.InstantiateDefaultCardEffects(Mask);

        if (cardInteractionType is CardInteractionType.Playable)
        {
            StartCoroutine(effects.ApplyEffects(TriggerType.OnChange, null));
        }
    }

    public bool ShouldShapeshift()
    {
        return !FATALLY_WOUNDED && Mask != GetCurrentMask();
    }

    public IEnumerator HandleShapeshift()
    {
        if (!ShouldShapeshift()) yield break;
        else yield return Shapeshift();
    }

    public IEnumerator Shapeshift()
    {
        CardBlueprint newMask = GetCurrentMask();
        Mask = newMask;

        Tools.PlaySound("Shapeshift", transform);
        yield return visualHandler.ToggleSpritesVanish(true);
        yield return effects.RemoveAllEffects();
        ResetPointAlterations();

        OnChange();

        visualHandler.SetNewCardVisual();
        yield return visualHandler.ToggleSpritesVanish(false);
    }

    public CardBlueprint GetCurrentMask()
    {
        if (higherBeing.isLocked && !FATALLY_WOUNDED) return Mask;
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

        modifierList = new List<BattleAmountModifier>(attackPointsModifiers)
        {
            GetCurrentMight()
        };

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



    public int TakeDamage(int incomingDamage, Card inflictor)
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

        _events.OnDamage.Raise(this, new AmountEventData(this, inflictor, damage));

        HasTakenDamageThisTurn = true;

        return damage;
    }


    public void TakeDirectDamage(int damage)
    {
        points -= damage;
        if (points < 0) points = 0;
        visualHandler.SetNumberSprites();
    }

    public void GainMight(int amount)
    {
        Might += amount;

        visualHandler.HandleMightVisual();
    }
    public BattleAmountModifier GetCurrentMight()
    {
        return new BattleAmountModifier(ModifierType.Addition, this, BattlePointType.Attack, Might, GetAmountStrategy.Value, null);
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
            if (guardian.IsPersistent)
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

    public IEnumerator DestroySelf()
    {
        DESTROYING = true;
        DataProvider?.RemoveCard(this);
        visualHandler.SetSortingOrder(-1);
        visualHandler.DisableBuffVisuals();
        yield return visualHandler.ToggleOverallVanish(true);
        _events.OnDeath.Raise(this, new NormalEventData(this));
        DOTween.Kill(transform);
        yield return Tools.GetWait(1f);
        gameObject.SetActive(false);
    }

    private void ResetPointAlterations()
    {
        attackPointsModifiers.Clear();
        hurtPointsModifiers.Clear();
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

    public void ResetTurnVariables()
    {
        HasTakenDamageThisTurn = false;
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