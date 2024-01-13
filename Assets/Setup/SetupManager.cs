using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupManager : MonoBehaviour
{
    public DeckInstance playerArchetypeDeck;

    [FoldoutGroup("Codex")]
    [SerializeField] private BlueprintPoolBlueprint playerCodexRecipe;
    [FoldoutGroup("Codex")]
    [SerializeField] private BlueprintPoolInstance playerCodex;

    [FoldoutGroup("Codex")]
    [SerializeField] private BlueprintPoolBlueprint enemyCodexRecipe;
    [FoldoutGroup("Codex")]
    [SerializeField] private BlueprintPoolInstance enemyCodex;

    [FoldoutGroup("Pools")]
    [SerializeField] private MetaPoolRecipe playerPoolRecipe;
    [FoldoutGroup("Pools")]
    [SerializeField] private MetaPoolInstance playerRuntimePool;

    [FoldoutGroup("Pools")]
    [SerializeField] private MetaPoolRecipe enemyPoolRecipe;
    [FoldoutGroup("Pools")]
    [SerializeField] private MetaPoolInstance enemyRuntimePool;

    public void Setup()
    {
        playerArchetypeDeck = new DeckInstance();
        InitializeRuntimeCodexes();
        InitializeRuntimePools();
    }

    [Button]
    public void InitializeRuntimeCodexes()
    {
        playerCodex = new();
        playerCodex.InitializeAsCodex(playerCodexRecipe);

        enemyCodex = new();
        enemyCodex.InitializeAsCodex(enemyCodexRecipe);
    }

    [Button]
    public void InitializeRuntimePools()
    {
        playerRuntimePool = new();
        playerRuntimePool.Initialize(playerPoolRecipe);

        enemyRuntimePool = new();
        enemyRuntimePool.Initialize(enemyPoolRecipe);
    }

}
