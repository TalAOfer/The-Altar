using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoom : Room
{
    private PlayerActionProvider playerActionProvider;


    [SerializeField] private BattleManager battleManager;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private EnemyCardSpawner spawner;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private RunData runData;
    [FoldoutGroup("Dependencies")]
    [SerializeField] protected RoomData roomData;


    [FoldoutGroup("Map Objects")]
    [SerializeField] protected Door door;
    [FoldoutGroup("Map Objects")]
    public List<MapSlot> grid;

    public List<Card> activeEnemies;
    private RoomBlueprint roomBlueprint;

    private void Awake()
    {
        playerActionProvider = Locator.PlayerActionProvider;
    }

    public override void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint)
    {
        this.roomBlueprint = roomBlueprint;

        base.InitializeRoom(floorManager, roomBlueprint);

        InitializeDeckForRoom();

        roomData.EnemyManager = this;
        roomData.BattleRoomState = BattleRoomStateEnum.Setup;

        door.Initialize(floorManager);

        SpawnEnemies();
    }

    public override void OnRoomFinishedLerping()
    {
        base.OnRoomFinishedLerping();

        playerActionProvider.FillHand();
    }

    private void InitializeDeckForRoom()
    {
        bool isPredetermined = roomBlueprint.predetermineDeck;
        if (!isPredetermined) runData.playerDeck = new Deck(runData.playerDeck.min, runData.playerDeck.max);
        else runData.playerDeck.cards = new List<CardArchetype>(roomBlueprint.deck.cards);
    }

    public void SpawnEnemies()
    {
        List<EnemySpawn> enemies = GetEnemies();

        for (int i = 0; i < enemies.Count; i++)
        {
            Card enemy = spawner.SpawnEnemyInIndexByBlueprint((int)enemies[i].Placement, enemies[i].Blueprint);

            activeEnemies.Add(enemy);
        }
    }

    private List<EnemySpawn> GetEnemies()
    {
        List<EnemySpawn> enemies = new();


        for (int row = 0; row < roomBlueprint.battle.cardGrid.GetLength(0); row++)
        {
            for (int col = 0; col < roomBlueprint.battle.cardGrid.GetLength(1); col++)
            {
                if (roomBlueprint.battle.cardGrid[row, col] != null)
                {
                    GridPlacement_3 placement = GetGridPlacementFromIndices(row, col);
                    enemies.Add(new EnemySpawn(roomBlueprint.battle.cardGrid[row, col], placement));
                }
            }
        }

        return enemies;
    }

    private GridPlacement_3 GetGridPlacementFromIndices(int row, int col)
    {
        // Assuming 3x3 grid, adjust if your grid size is different
        return (GridPlacement_3)(row * 3 + col);
    }

    public void RemoveEnemyFromManager(Card card)
    {
        activeEnemies.Remove(card);
    }

    public void OpenDoor()
    {
        door.OpenDoor();
    }

    [Button]
    public void DestroyAllEnemies()
    {
        List<Card> enemiesToDestroy = new(activeEnemies);
        foreach (Card card in enemiesToDestroy)
        {
            Destroy(card.gameObject);
        }

        activeEnemies.Clear();
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


