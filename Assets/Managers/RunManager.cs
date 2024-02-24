using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    private RunData runData;    

    [SerializeField] private bool setupOnAwake;

    [SerializeField] private Vector2Int DefaultDrawabilityMinMax;

    [SerializeField] private CodexBlueprint playerCodexRecipe;

    [SerializeField] private MetaPoolRecipe playerPoolRecipe;

    public void Awake()
    {
        runData = Locator.RunData;
        if (setupOnAwake) InitializeRun();
    }

    [Button]
    public void InitializeRun()
    {
        InitializePlayerManager();
        InitializePlayerDeck();
        InitializePlayerCodex();
        InitializePlayerPool();
    }

    private void InitializePlayerManager()
    {
        runData.playerManager = Locator.PlayerManager;
    }

    private void InitializePlayerDeck()
    {
        runData.playerDeck = new Deck(DefaultDrawabilityMinMax.x, DefaultDrawabilityMinMax.y);
    }

    private void InitializePlayerCodex()
    {
        runData.playerCodex = new(playerCodexRecipe);
    }

    private void InitializePlayerPool()
    {
        runData.playerPool = new();
        runData.playerPool.Initialize(playerPoolRecipe);
    }

}
