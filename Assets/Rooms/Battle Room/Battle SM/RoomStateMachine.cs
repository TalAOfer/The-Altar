using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomStateMachine : MonoBehaviour 
{ 
    public StateFactory States { get; private set; }
    public BattleBlueprint BattleBlueprint { get; private set; }
    public PlayerCardManager PlayerCardManager { get; private set; }
    public EnemyCardManager EnemyCardManager { get; private set; }
    public BattleRoomDataProvider DataProvider { get; private set; }
    public BattleManager BattleManager { get; private set; }
    public LoseManager LoseManager { get; private set; }
    [ShowInInspector]
    private readonly SMContext _ctx = new();
    protected IRoomState _currentState;

    [ShowInInspector, ReadOnly]
    private string _currentStateName => _currentState?.GetType().Name ?? "None";
    public bool IsSwitchingStates { get; private set; }
    public Room _room;

    [SerializeField] protected Door leftDoor;
    [SerializeField] protected Door rightDoor;
    public FloorManager FloorCtx { get; private set; }
    public EventRegistry Events { get; private set; }

    public PrefabRegistry Prefabs { get; private set; }
    public virtual void Initialize(FloorManager floorCtx, Room room)
    {
        FloorCtx = floorCtx;
        Events = Locator.Events;
        FloorLevel nextLevel = floorCtx.Floor.Levels[floorCtx.CurrentRoomIndex + 1];
        leftDoor.Initialize(floorCtx, nextLevel.LeftRoom);
        rightDoor.Initialize(floorCtx, nextLevel.RightRoom);
        Prefabs = Locator.Prefabs;

        States = new StateFactory(this, _ctx);

        #region Battle Init
        DataProvider = new BattleRoomDataProvider(this, _ctx);

        _room = room;
        if (room.Type is RoomType.Battle)
            BattleBlueprint = FloorCtx.Floor.BattlePool.GetBattleBlueprintAccordingToIndex(room.Difficulty);

        BattleManager = GetComponentInChildren<BattleManager>();
        if (BattleManager != null) BattleManager.Initialize(this, _ctx);

        PlayerCardManager = GetComponentInChildren<PlayerCardManager>();
        if (PlayerCardManager != null) PlayerCardManager.Initialize(this);

        EnemyCardManager = GetComponentInChildren<EnemyCardManager>();
        if (EnemyCardManager != null) EnemyCardManager.Initialize(this);

        LoseManager = GetComponentInChildren<LoseManager>();
        #endregion
    }

    public void InitializeStateMachine()
    {
        switch (_room.Type)
        {
            case RoomType.First:
                SwitchState(States.ShowTitle());
                break;
            case RoomType.Battle:
                SwitchState(States.SpawnEnemies());
                break;




            case RoomType.Nothing:
                break;
            case RoomType.Elite:
                break;
            case RoomType.Shop:
                break;
            case RoomType.Treasure:
                break;

            case RoomType.Boss:
                break;
        }

    }


public virtual void SwitchState(IRoomState newState)
    {
        StartCoroutine(SwitchStateRoutine(newState));
    }

    public IEnumerator SwitchStateRoutine(IRoomState newState)
    {
        IsSwitchingStates = true;

        if (_currentState != null)
        {
            yield return StartCoroutine(_currentState.ExitState());
        }

        _currentState = newState;

        yield return StartCoroutine(_currentState.EnterState());

        IsSwitchingStates = false;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public IEnumerator OpenDoors()
    {
        Coroutine LeftDoorRoutine = StartCoroutine(leftDoor.OpenDoorRoutine());
        Coroutine RightDoorRoutine = StartCoroutine(rightDoor.OpenDoorRoutine());

        yield return LeftDoorRoutine;
        yield return RightDoorRoutine;
    }

    public GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return Instantiate(prefab, position, rotation, parent);
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

    public void MarkCardAsSelected(Card card)
    {
        card.movement.Highlight();
        card.visualHandler.EnableOutline(PaletteColor.white);

    }

    public void DemarkCardAsSelected(Card card)
    {
        card.visualHandler.DisableOutline();
        card.movement.Dehighlight();
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
public class SMContext
{
    public TreasureChest currentTreasureChest;

    public Card BattlingPlayerCard;
    public Card BattlingEnemyCard;

    public Card CardClicked;

    public Card CurrentActorCard;
    public Card CurrentTargetCard;

    public List<Card> CurrentCardsSelected = new();

    public Ability CurrentAbilitySelected;
}