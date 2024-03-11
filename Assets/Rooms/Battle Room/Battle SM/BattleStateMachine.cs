using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleStateMachine : RoomStateMachine
{
    public BattleStateFactory States { get; private set; }
    public BattleBlueprint BattleBlueprint { get; private set; }
    public PlayerCardManager PlayerCardManager { get; private set; }
    public EnemyCardManager EnemyCardManager { get; private set; }
    public BattleRoomDataProvider DataProvider { get; private set; }
    public BattleManager BattleManager { get; private set; }
    public LoseManager LoseManager { get; private set; }
    [ShowInInspector]
    public BattleContext Ctx { get; private set; } = new();

    public override void Initialize(FloorManager floorCtx, Room room)
    {
        base.Initialize(floorCtx, room);

        States = new BattleStateFactory(this);
        DataProvider = new BattleRoomDataProvider(this);

        BattleBlueprint = room.BattleBlueprint;

        BattleManager = GetComponentInChildren<BattleManager>();
        BattleManager.Initialize(this);

        PlayerCardManager = GetComponentInChildren<PlayerCardManager>();
        PlayerCardManager.Initialize(this);

        EnemyCardManager = GetComponentInChildren<EnemyCardManager>();
        EnemyCardManager.Initialize(this);

        LoseManager = GetComponentInChildren<LoseManager>();
    }

    public override void InitializeStateMachine()
    {
        SwitchState(States.SpawnEnemies());
    }

    public IEnumerator HandleAllShapeshiftsUntilStable()
    {
        bool changesOccurred;
        List<Card> allCards = PlayerCardManager.ActiveCards.Concat(EnemyCardManager.ActiveEnemies).ToList();
        if (allCards.Count <= 0) yield break;

        do
        {
            changesOccurred = false;
            List<Coroutine> shapeshiftCoroutines = new();

            foreach (Card card in allCards)
            {
                if (card.ShouldShapeshift())
                {
                    changesOccurred = true;
                    Coroutine coroutine = StartCoroutine(card.HandleShapeshift());
                    shapeshiftCoroutines.Add(coroutine);
                }
            }

            foreach (Coroutine coroutine in shapeshiftCoroutines)
            {
                yield return coroutine;
            }

        } while (changesOccurred);
    }

    public void SelectCard(Card card)
    {
        card.ChangeCardState(CardState.Selected);
        card.movement.Highlight();
        card.visualHandler.EnableOutline(PaletteColor.white);
        Ctx.CurrentActorCard = card;
    }

    public void DeselectCurrentCard()
    {
        Ctx.CurrentActorCard.visualHandler.DisableOutline();
        Ctx.CurrentActorCard.movement.Dehighlight();
        Ctx.CurrentActorCard.ChangeCardState(CardState.Default);
        Ctx.CurrentActorCard = null;
    }

    #region Event Handlers

    public void OnPlayerCardClicked(Card card, PointerEventData data)
    {
        _currentState?.HandlePlayerCardPointerClick(card, data);
    }

    public void OnPlayerCardPointerEnter(Card card, PointerEventData data)
    {
        _currentState?.HandlePlayerCardPointerEnter(card, data);
    }

    public void OnPlayerCardPointerExit(Card card, PointerEventData data)
    {
        _currentState?.HandlePlayerCardPointerExit(card, data);
    }

    public void OnEnemyCardClicked(Card card, PointerEventData data)
    {
        _currentState?.HandleEnemyCardPointerClick(card, data);
    }

    public void OnEnemyCardPointerEnter(Card card, PointerEventData data)
    {
        _currentState?.HandleEnemyCardPointerEnter(card, data);
    }

    public void OnEnemyCardPointerExit(Card card, PointerEventData data)
    {
        _currentState?.HandleEnemyCardPointerExit(card, data);
    }

    public void OnPlayerCardBeginDrag(Card card, PointerEventData data)
    {
        _currentState?.HandlePlayerCardBeginDrag(card, data);
    }

    public void OnPlayerCardEndDrag(Card card, PointerEventData data)
    {
        _currentState?.HandlePlayerCardEndDrag(card, data);
    }


    public void OnHandColliderPointerEnter(HandCollisionDetector HandCollisionManager, PointerEventData data)
    {
        _currentState?.OnHandColliderPointerEnter(HandCollisionManager, data);
    }

    public void OnHandColliderPointerExit(HandCollisionDetector HandCollisionManager, PointerEventData data)
    {
        _currentState?.OnHandColliderPointerExit(HandCollisionManager, data);
    }

    public void OnAbilityClicked(AbilityManager abilityManager, Ability ability)
    {
        _currentState?.OnAbilityClicked(abilityManager, ability);
    }

    #endregion
}

[Serializable]
public class BattleContext
{
    public Card BattlingPlayerCard;
    public Card BattlingEnemyCard;

    public Card CardClicked;

    public Card CurrentActorCard;
    public Card CurrentTargetCard;

    public List<Card> CurrentCardsSelected = new();

    public Ability CurrentAbilitySelected;
}