using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoom : Room
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private int difficulty;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private AllEvents events;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private EnemyCardSpawner spawner;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private RunData runData;

    [FoldoutGroup("Map Objects")]
    [SerializeField] private Door door;
    [FoldoutGroup("Map Objects")]
    public List<MapSlot> grid;

    public List<Card> activeEnemies;
    private RoomBlueprint roomBlueprint;

    public override void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint)
    {
        this.roomBlueprint = roomBlueprint;

        base.InitializeRoom(floorManager, roomBlueprint);

        InitializeDeckForRoom();

        battleManager.Initialize(floorManager);
        door.Initialize(floorManager);
        difficulty = roomBlueprint.difficulty;

        if (!roomBlueprint.predetermineEnemies) SpawnEnemies();
        else SpawnTestEnemies();
    }

    private void InitializeDeckForRoom()
    {
        bool isPredetermined = roomBlueprint.predetermineDeck;
        if (!isPredetermined) runData.playerDeck = new DeckInstance(roomBlueprint.playerDrawMinMax.x, roomBlueprint.playerDrawMinMax.y, true);
        else runData.playerDeck.cards = new List<CardArchetype>(roomBlueprint.deck.cards);
    }

    public void SpawnEnemies()
    {
        List<int> enemyConfig = Tools.DivideRandomly(difficulty, roomBlueprint.playerDrawMinMax.x, roomBlueprint.playerDrawMinMax.y);
        List<int> slots = Tools.GetXUniqueRandoms(enemyConfig.Count, 0, 8);

        for (int i = 0; i < enemyConfig.Count; i++)
        {
            int slotIndex = slots[i];
            int strength = enemyConfig[i];

            Card enemy = spawner.SpawnEnemyInIndexByStrength(slotIndex, strength);

            activeEnemies.Add(enemy);
        }
    }

    public void SpawnTestEnemies()
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
