using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour
{
    [SerializeField] private AllEvents events;
    [SerializeField] private RunData runData;
    [SerializeField] private bool setupOnAwake;

    [FoldoutGroup("Room Data")]
    [SerializeField] private RoomData roomData;
    [FoldoutGroup("Room Data")]
    [SerializeField] private PlayerManager playerManager;
    [FoldoutGroup("Room Data")]
    [SerializeField] private FloorManager floorManager;

    [FoldoutGroup("Player Deck")]
    [SerializeField] private int minDrawability;
    [FoldoutGroup("Player Deck")]
    [SerializeField] private int maxDrawability;

    [FoldoutGroup("Player Codex")]
    [SerializeField] private BlueprintPoolBlueprint playerCodexRecipe;
    [FoldoutGroup("Player Codex")]
    [Title("Test")]
    [SerializeField] private bool playerTest;
    [FoldoutGroup("Player Codex")]
    [SerializeField] private BlueprintPoolBlueprint playerTestCodexRecipe;

    [FoldoutGroup("Enemy Codex")]
    [SerializeField] private BlueprintPoolBlueprint enemyCodexRecipe;
    [FoldoutGroup("Enemy Codex")]
    [Title("Test")]
    [SerializeField] private bool enemyTest;
    [FoldoutGroup("Enemy Codex")]
    [SerializeField] private BlueprintPoolBlueprint enemyTestCodexRecipe;

    [FoldoutGroup("Pools")]
    [SerializeField] private MetaPoolRecipe playerPoolRecipe;
    [FoldoutGroup("Pools")]
    [SerializeField] private MetaPoolRecipe enemyPoolRecipe;

    public void Awake()
    {
        if (setupOnAwake) Setup();
    }

    [Button]
    public void Setup()
    {
        events.SetGameState.Raise(this, GameState.GameSetup);
        InitializeRoomData();
        InitializePlayerDeck();
        InitializeRuntimeCodexes();
        InitializeRuntimePools();
    }

    private void InitializeRoomData()
    {
        roomData.PlayerManager = playerManager;
        roomData.floorManager = floorManager;
    }

    private void InitializePlayerDeck()
    {
        runData.playerDeck = new DeckInstance(minDrawability, maxDrawability, true);
    }

    private void InitializeRuntimeCodexes()
    {
        runData.playerCodex = new();
        runData.playerCodex.InitializeAsCodex(playerCodexRecipe);

        runData.enemyCodex = new();
        runData.enemyCodex.InitializeAsCodex(enemyCodexRecipe);
    }

    private void InitializeRuntimePools()
    {
        runData.playerPool = new();
        runData.playerPool.Initialize(playerPoolRecipe);

        runData.enemyPool = new();
        runData.enemyPool.Initialize(enemyPoolRecipe);
    }
}
