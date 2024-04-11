using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Level")]
public class LevelBlueprint : ScriptableObject
{
    public Biome Biome;

    [Title("Enemy")]
    [OnValueChanged("InitializeEnemyCodex")]
    public List<CardArchetype> EnemyProgressionMap;
    public Codex EnemyCodex {  get; private set; }

    [ValueDropdown("AvailableEnemyCards")]
    [Title("Enemy Cards")]
    public List<CardBlueprint> EnemyCards;

    [Title("Player")]
    public List<CardArchetype> PlayerProgressionMap;
    public Codex PlayerCodex { get; private set; }

    [ValueDropdown("AvailablePlayerCards")]
    [Title("Player Cards")]
    public List<CardBlueprint> PlayerCards;

    // Enemy Methods
    private IEnumerable<CardBlueprint> AvailableEnemyCards
    {
        get
        {
            if (Biome == null) return null;

            if (EnemyCodex == null || !EnemyCodex.red.Any() || !EnemyCodex.black.Any())
            {
                InitializeEnemyCodex();
            }

            return EnemyCodex.black.Concat(EnemyCodex.red);
        }
    }

    private IEnumerable<CardBlueprint> AvailablePlayerCards
    {
        get
        {
            if (Biome == null) return null;

            if (PlayerCodex == null || !PlayerCodex.red.Any() || !PlayerCodex.black.Any())
            {
                InitializePlayerCodex();
            }

            return PlayerCodex.black.Concat(PlayerCodex.red);
        }
    }

    private void InitializeEnemyCodex()
    {
        if (Biome == null) return;

        EnemyCodex = new Codex(Biome.DefaultEnemyCodex, Biome.FullEnemyCodex, EnemyProgressionMap);
    }

    private void InitializePlayerCodex()
    {
        if (Biome == null) return;

        PlayerCodex = new Codex(Biome.DefaultPlayerCodex, Biome.FullPlayerCodex, PlayerProgressionMap);
    }

    
    [Button("Update Codices")]
    public void ReinitializeCodices()
    {
        InitializeEnemyCodex();
        InitializePlayerCodex();
    }
}
