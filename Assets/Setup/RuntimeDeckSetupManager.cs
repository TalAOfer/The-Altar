using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeDeckSetupManager : MonoBehaviour
{
    [FoldoutGroup("Player")]
    [SerializeField] private BlueprintPoolBlueprint playerDeckRecipe;
    [FoldoutGroup("Player")]
    [SerializeField] private BlueprintPoolInstance playerRuntimeDeck;

    [FoldoutGroup("Enemy")]
    [SerializeField] private BlueprintPoolBlueprint enemyDeckRecipe;
    [FoldoutGroup("Enemy")]
    [SerializeField] private BlueprintPoolInstance enemyRuntimeDeck;

    public void InitializeRuntimeDecks()
    {
        playerRuntimeDeck = new();
        playerRuntimeDeck.Initialize(playerDeckRecipe);

        enemyRuntimeDeck = new();
        enemyRuntimeDeck.Initialize(enemyDeckRecipe);
    }
}
