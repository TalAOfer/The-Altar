using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomStateMachine : MonoBehaviour
{
    public StateFactory States { get; private set; }
    // Room Data

    public CurrentLevel CurrentLevel { get; private set; }
    public Codex EnemyCodex { get; private set; }
    public Codex PlayerCodex {  get; private set; }
    public List<CardBlueprint> InitialPlayerSpawn { get; private set; }
    public List<CardBlueprint> InitialEnemySpawn { get; private set; }
    
    public PlayerCardManager PlayerCardManager { get; private set; }
    public EnemyCardManager EnemyCardManager { get; private set; }
    public BattleRoomDataProvider DataProvider { get; private set; }
    public BattleManager BattleManager { get; private set; }
    public EffectApplier EffectApplier { get; private set; }
    public LoseManager LoseManager { get; private set; }
    [ShowInInspector]
    private readonly SMContext _ctx = new();
    protected BaseRoomState _currentState;

    [ShowInInspector, ReadOnly]
    private string _currentStateName => _currentState?.GetType().Name ?? "None";
    public bool IsSwitchingStates { get; private set; }
    public RoomBlueprint Room { get; private set; }

    [SerializeField] protected List<Door> Doors = new();

    public EventRegistry Events { get; private set; }

    public PrefabRegistry Prefabs { get; private set; }
    public RunData RunData { get; private set; }

    private void Awake()
    {
        Events = Locator.Events;
        RunData = Locator.RunData;
        Prefabs = Locator.Prefabs;
        CurrentLevel = Locator.CurrentLevel;

        States = new StateFactory(this, _ctx);
        DataProvider = new BattleRoomDataProvider(this, _ctx);

        BattleManager = GetComponentInChildren<BattleManager>();
        if (BattleManager != null) BattleManager.Initialize(this, _ctx);

        PlayerCardManager = GetComponentInChildren<PlayerCardManager>();
        if (PlayerCardManager != null) PlayerCardManager.Initialize(this);

        EnemyCardManager = GetComponentInChildren<EnemyCardManager>();
        if (EnemyCardManager != null) EnemyCardManager.Initialize(this, DataProvider);

        EffectApplier = GetComponentInChildren<EffectApplier>();
        if (EffectApplier != null) EffectApplier.Initialize(this);

        LoseManager = GetComponentInChildren<LoseManager>();
    }

    //Standalone room initializer
    public void Initialize()
    {
        CurrentLevel.Value.ReinitializeCodices();
        EnemyCodex = CurrentLevel.Value.EnemyCodex;
        InitialEnemySpawn = CurrentLevel.Value.EnemyCards;

        PlayerCodex = CurrentLevel.Value.PlayerCodex;
        InitialPlayerSpawn = CurrentLevel.Value.PlayerCards;

        SwitchState(States.SpawnEnemies());
    }

    //Floor manager
    //public void Initialize(Floor floor, int index, RoomBlueprint room, Codex enemyCodex)
    //{
    //    _ctx.RoomIndex = index;

    //    Room = room;

    //    if (room.RoomEvents.HasFlag(RoomEvent.Battle))
    //    {
    //        InitialEnemySpawn = floor.BattlePool.GetBattleBlueprintAccordingToIndex(room.Difficulty).GetCards();
    //    }

    //    EnemyCodex = enemyCodex;
    //    PlayerCodex = RunData.playerCodex;
    //}

    public void NextLevel()
    {
        ResetDoor();
        LevelBlueprint NextLevel = CurrentLevel.Value.Biome.GetNextLevel(CurrentLevel.Value);
        CurrentLevel.Value = NextLevel;
        if (CurrentLevel.Value == null) return;
        Initialize();
    }

    public void ResetDoor()
    {
        Doors[0].gateGO.SetActive(true);
        Doors[0].gateGO.transform.localPosition = Vector3.zero;
    }


    public void InitializeStateMachine()
    {
        if (Room.RoomEvents.HasFlag(RoomEvent.Battle))
        {
            SwitchState(States.SpawnEnemies());
        }

        else if (Room.RoomEvents.HasFlag(RoomEvent.Reward))
        {
            SwitchState(States.SpawnTreasure());
        }
    }


    public void SwitchState(BaseRoomState newState)
    {
        StartCoroutine(SwitchStateRoutine(newState));
    }

    public IEnumerator SwitchStateRoutine(BaseRoomState newState)
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
        List<Coroutine> openingRoutines = new();

        foreach (Door door in Doors)
        {
            openingRoutines.Add(StartCoroutine(door.OpenDoorRoutine()));
        }

        foreach (Coroutine coroutine in openingRoutines)
        {
            yield return coroutine;
        }
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

    public void OnRoomButtonClicked(CustomButton button, int index)
    {
        _currentState?.OnRoomButtonClicked(button, index);
    }

    #endregion
}

[Serializable]
public class SMContext
{
    public int RoomIndex;

    public TreasureChest currentTreasureChest;

    public Card BattlingPlayerCard;
    public Card BattlingEnemyCard;

    public Card CardClicked;

    public Card CurrentActorCard;
    public Card CurrentTargetCard;

    public List<Card> CurrentCardsSelected = new();

    public Ability CurrentAbilitySelected;
}