using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleStateMachine : RoomManager
{
    public bool IsSwitchingStates {  get; private set; }

    private BaseBattleState _currentBattleState;
    public BattleStateFactory States { get; private set; }
    public NewFloorManager FloorCtx { get; private set; }
    public BattleBlueprint BattleBlueprint { get; private set; }
    public PlayerCardManager PlayerCardManager { get; private set; }
    public EnemyCardManager EnemyCardManager { get; private set; }
    public BattleRoomDataProvider DataProvider { get; private set; }

    public override void Initialize(NewFloorManager floorCtx, Room room)
    {
        States = new BattleStateFactory(this);
        DataProvider = new BattleRoomDataProvider(this);

        FloorCtx = floorCtx;

        BattleBlueprint = room.BattleBlueprint;

        PlayerCardManager.Init(this);
        EnemyCardManager.Init(this);

        SwitchState(States.SpawnEnemies());
    }


    public override void OnRoomReady()
    {
        SwitchState(States.DrawHand());
    }

    public void SwitchState(BaseBattleState newState)
    {
        StartCoroutine(SwitchStateRoutine(newState));
    }

    public IEnumerator SwitchStateRoutine(BaseBattleState newState)
    {
        IsSwitchingStates = true;

        if (_currentBattleState != null)
        {
            yield return StartCoroutine(_currentBattleState.ExitState());
        }

        _currentBattleState = newState;

        yield return StartCoroutine(_currentBattleState.EnterState());

        IsSwitchingStates = false;
    }

    public IEnumerator HandleAllShapeshiftsUntilStable()
    {
        bool changesOccurred;
        List<Card> allCards = PlayerCardManager.ActiveCards.Concat(EnemyCardManager.ActiveEnemies).ToList();
        if (allCards.Count <= 0) yield break;

        do
        {
            changesOccurred = false;
            List<Coroutine> shapeshiftCoroutines = new List<Coroutine>();

            foreach (Card card in allCards)
            {
                if (card.ShouldShapeshift())
                {
                    changesOccurred = true;
                    Coroutine coroutine = StartCoroutine(card.HandleShapeshift());
                    shapeshiftCoroutines.Add(coroutine);
                }
            }

            // Wait for all shapeshift coroutines to finish
            foreach (Coroutine coroutine in shapeshiftCoroutines)
            {
                yield return coroutine;
            }

            // If changesOccurred is true, the loop will continue
        } while (changesOccurred);

        // All shapeshifts are done and no more changes, proceed with the next operation
        // ...
    }
}

[System.Serializable]
public class EnemySpawn
{
    public CardBlueprint Blueprint;
    public GridPlacement_3 Placement;

    public EnemySpawn(CardBlueprint blueprint, GridPlacement_3 placement)
    {
        Blueprint = blueprint;
        Placement = placement;
    }
}