using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Run Data")]
public class RunData : ScriptableObject, IResetOnPlaymodeExit
{
    [ShowInInspector]
    [ReadOnly]
    public PlayerHealth PlayerHealth { get; private set; } = new(100);

    public CodexBlueprint playerCodexRecipe;
    public MetaPoolRecipe playerPoolRecipe;
    public EventRegistry events;

    public Vector2Int defaultPlayerDrawMinMax;

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
        InitializePlayerCodex();
        InitializePlayerPool();
        InitializePlayerDeck();

        isInitialized = true;
    }

    private void InitializePlayerDeck()
    {
        playerDeck = new Deck(defaultPlayerDrawMinMax.x, defaultPlayerDrawMinMax.y);
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

    public void TakeGlobalDamage(int damage)
    {
        int newHealth = PlayerHealth.Current - damage;
        SetHealth(newHealth);
    }
    public void SetHealth(int newHealth)
    {
        PlayerHealth.Current = newHealth;
        events.UpdateHealth.Raise(null, PlayerHealth);
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        PlayerHealth.Max = newMaxHealth;    
        events.UpdateHealth.Raise(null, PlayerHealth);
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

[Serializable]
public class PlayerHealth
{
    public int Current;
    public int Max;

    public PlayerHealth(int max)
    {
        Current = max;
        Max = max;
    }
}