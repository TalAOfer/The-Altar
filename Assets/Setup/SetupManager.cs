using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour
{
    [SerializeField] private AllEvents events;
    [SerializeField] private bool setupOnAwake;

    [FoldoutGroup("Player Deck")]
    [SerializeField] private int minDrawability;
    [FoldoutGroup("Player Deck")]
    [SerializeField] private int maxDrawability;
    [FoldoutGroup("Player Deck")]
    [SerializeField] private DeckInstance playerArchetypeDeck;

    [FoldoutGroup("Player Codex")]
    [SerializeField] private BlueprintPoolBlueprint playerCodexRecipe;
    [FoldoutGroup("Player Codex")]
    [SerializeField] private BlueprintPoolInstance playerCodex;
    [FoldoutGroup("Player Codex")]
    [SerializeField] private PlayerCardSpawner playerSpawner;

    [FoldoutGroup("Enemy Codex")]
    [SerializeField] private BlueprintPoolBlueprint enemyCodexRecipe;
    [FoldoutGroup("Enemy Codex")]
    [SerializeField] private BlueprintPoolInstance enemyCodex;
    [FoldoutGroup("Enemy Codex")]
    [SerializeField] private EnemyCardSpawner enemySpawner;

    [FoldoutGroup("Player Pools")]
    [SerializeField] private MetaPoolRecipe playerPoolRecipe;
    [FoldoutGroup("Player Pools")]
    [SerializeField] private MetaPoolInstance playerRuntimePool;

    [FoldoutGroup("Enemy Pools")]
    [SerializeField] private MetaPoolRecipe enemyPoolRecipe;
    [FoldoutGroup("Enemy Pools")]
    [SerializeField] private MetaPoolInstance enemyRuntimePool;



    public void Awake()
    {
        if (setupOnAwake) Setup();
    }

    [Button]
    public void Setup()
    {
        events.SetGameState.Raise(this, GameState.GameSetup);
        InitializePlayerDeck();
        InitializeRuntimeCodexes();
        InitializeRuntimePools();
    }

    private void InitializePlayerDeck()
    {
        playerArchetypeDeck = new DeckInstance(minDrawability, maxDrawability, true);
        playerSpawner.deck = playerArchetypeDeck;
    }

    private void InitializeRuntimeCodexes()
    {
        playerCodex = new();
        playerCodex.InitializeAsCodex(playerCodexRecipe);
        playerSpawner.codex = playerCodex;

        enemyCodex = new();
        enemyCodex.InitializeAsCodex(enemyCodexRecipe);
        enemySpawner.codex = enemyCodex;
    }

    private void InitializeRuntimePools()
    {
        playerRuntimePool = new();
        playerRuntimePool.Initialize(playerPoolRecipe);

        enemyRuntimePool = new();
        enemyRuntimePool.Initialize(enemyPoolRecipe);
    }


}
