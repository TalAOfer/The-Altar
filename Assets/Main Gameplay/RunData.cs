using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Run Data")]
public class RunData : ScriptableObject, IResetOnPlaymodeExit
{
    public Vector2Int DefaultDrawabilityMinMax;
    public CodexBlueprint playerCodexRecipe;
    public MetaPoolRecipe playerPoolRecipe;

    [FoldoutGroup("Runtime Data")]
    [ReadOnly]
    public bool isInitialized;
    [FoldoutGroup("Runtime Data")]
    [ReadOnly]
    public Deck playerDeck;
    [FoldoutGroup("Runtime Data")]
    [ReadOnly]
    public Codex playerCodex;
    [FoldoutGroup("Runtime Data")]
    [ReadOnly]
    public MetaPoolInstance playerPool;

    public void Initialize()
    {
        if (isInitialized) return;

        InitializePlayerDeck();
        InitializePlayerCodex();
        InitializePlayerPool();

        isInitialized = true;
    }

    private void InitializePlayerDeck()
    {
        playerDeck = new Deck(DefaultDrawabilityMinMax.x, DefaultDrawabilityMinMax.y);
    }

    private void InitializePlayerCodex()
    {
        playerCodex = new(playerCodexRecipe);
    }

    private void InitializePlayerPool()
    {
        playerPool = new();
        playerPool.Initialize(playerPoolRecipe);
    }

    public void PlaymodeExitReset()
    {
        isInitialized = false;
        playerDeck = null;
        playerCodex = null;
        playerPool = null;
    }
}
