using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Run Data")]
public class RunData : ScriptableObject, IResetOnPlaymodeExit
{
    public Vector2Int Hp_MaxHp { get; private set; } = new(100, 100);
    public Vector2Int DefaultDrawabilityMinMax;
    public CodexBlueprint playerCodexRecipe;
    public MetaPoolRecipe playerPoolRecipe;
    public EventRegistry events;

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

        ResetPlayerHealth();
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

    public void SetMaxHealth(int newMaxHealth)
    {
        int currentHp = Hp_MaxHp.x;
        Hp_MaxHp.Set(currentHp, newMaxHealth);
    }
    public void SetHealth(int newHealth)
    {
        int currentMaxHp = Hp_MaxHp.y;
        Hp_MaxHp.Set(newHealth, currentMaxHp);
    }

    private void ResetPlayerHealth()
    {
        SetHealth(100);
        SetMaxHealth(100);
    }

    public void PlaymodeExitReset()
    {
        isInitialized = false;
        ResetPlayerHealth();
        playerDeck = null;
        playerCodex = null;
        playerPool = null;
    }
}
