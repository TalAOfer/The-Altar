using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimePoolSetupManager : MonoBehaviour
{
    [FoldoutGroup("Player")]
    [SerializeField] private MetaPoolRecipe playerPoolRecipe;
    [FoldoutGroup("Player")]
    [SerializeField] private MetaPoolInstance playerRuntimePool;

    [FoldoutGroup("Enemy")]
    [SerializeField] private MetaPoolRecipe enemyPoolRecipe;
    [FoldoutGroup("Enemy")]
    [SerializeField] private MetaPoolInstance enemyRuntimePool;


    public void InitializeRuntimePools()
    {
        playerRuntimePool = new();
        playerRuntimePool.Initialize(playerPoolRecipe);

        enemyRuntimePool = new();
        enemyRuntimePool.Initialize(enemyPoolRecipe);
    }
}
