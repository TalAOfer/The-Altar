using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoom : Room
{
    private PlayerActionProvider actionProvider;


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
        
    }

    public override void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint)
    {
        this.roomBlueprint = roomBlueprint;

        base.InitializeRoom(floorManager, roomBlueprint);

        InitializeDeckForRoom();

        roomData.EnemyManager = this;
        roomData.BattleRoomState = BattleRoomState.Setup;

        door.Initialize(floorManager);

        SpawnEnemies();
    }

    public override void OnRoomFinishedLerping()
    {
        base.OnRoomFinishedLerping();

        actionProvider.FillHand();
    }

    private void InitializeDeckForRoom()
    {
        bool isPredetermined = roomBlueprint.predetermineDeck;
        if (!isPredetermined) runData.playerDeck = new Deck(runData.playerDeck.min, runData.playerDeck.max);
        else runData.playerDeck.cards = new List<CardArchetype>(roomBlueprint.deck.cards);
    }

    public void SpawnEnemies()
    {
        int amountOfEnemies = roomBlueprint.enemyArchetypes.Count;
        List<int> slots = Tools.GetXUniqueRandoms(amountOfEnemies, 0, 8);

        for (int i = 0; i < amountOfEnemies; i++)
        {
            int slotIndex = slots[i];
            CardBlueprint enemyBlueprint = spawner.DrawEnemyByArchetype(roomBlueprint.enemyArchetypes[i]);

            Card enemy = spawner.SpawnEnemyInIndexByBlueprint(slotIndex, enemyBlueprint);

            activeEnemies.Add(enemy);
        }
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
