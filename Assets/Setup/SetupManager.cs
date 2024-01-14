using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour
{
    [SerializeField] private AllEvents events;
    [SerializeField] private RunData runData;
    [SerializeField] private bool setupOnAwake;

    [FoldoutGroup("Player Deck")]
    [SerializeField] private int minDrawability;
    [FoldoutGroup("Player Deck")]
    [SerializeField] private int maxDrawability;

    [FoldoutGroup("Codex")]
    [SerializeField] private BlueprintPoolBlueprint playerCodexRecipe;
    [FoldoutGroup("Codex")]
    [SerializeField] private BlueprintPoolBlueprint enemyCodexRecipe;

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
        InitializePlayerDeck();
        InitializeRuntimeCodexes();
        InitializeRuntimePools();
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
