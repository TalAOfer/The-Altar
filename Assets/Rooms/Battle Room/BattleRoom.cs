using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoom : Room
{
    [SerializeField] private int difficulty;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private AllEvents events;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private EnemyCardSpawner spawner;

    [FoldoutGroup("Map Objects")]
    [SerializeField] private Door door;
    [FoldoutGroup("Map Objects")]
    public List<MapSlot> grid;

    public List<Card> activeEnemies;


    public override void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint)
    {
        door.floorManager = floorManager;
        difficulty = roomBlueprint.difficulty;
        SpawnEnemies();
    }

    public override void OnRoomFinishedLerping()
    {
        
    }

    public override void OnRoomFinished()
    {

    }

    public void SpawnEnemies()
    {
        List<int> enemyConfig = Tools.DivideRandomly(difficulty, 1, difficulty);
        List<int> slots = Tools.GetXUniqueRandoms(enemyConfig.Count, 0, 8);

        for (int i = 0; i < enemyConfig.Count; i++)
        {
            int slotIndex = slots[i];
            int strength = enemyConfig[i];

            Card enemy = spawner.SpawnEnemyInIndexByStrength(slotIndex, strength);

            activeEnemies.Add(enemy);
        }
    }

    public void RemoveEnemyFromManager(Card card)
    {
        activeEnemies.Remove(card);
        if (activeEnemies.Count == 0)
        {
            door.OpenDoor();
        }
    }

    [Button]
    public void DestroyAllEnemies()
    {
        List<Card> enemiesToDestroy = new(activeEnemies);
        foreach(Card card in enemiesToDestroy)
        {
            Destroy(card.gameObject);
        }

        activeEnemies.Clear();
    }

}
