using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleStateMachine : RoomStateMachine
{
    public BattleStateFactory States { get; private set; }
    public BattleBlueprint BattleBlueprint { get; private set; }
    public PlayerCardManager PlayerCardManager { get; private set; }
    public EnemyCardManager EnemyCardManager { get; private set; }
    public BattleRoomDataProvider DataProvider { get; private set; }
    public BattleManager BattleManager { get; private set; }
    public BattleInteractionHandler InteractionHandler { get; private set; }

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

        InteractionHandler = GetComponentInChildren<BattleInteractionHandler>();
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