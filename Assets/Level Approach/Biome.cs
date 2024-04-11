using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Biome")]
public class Biome : ScriptableObject
{
    public CodexBlueprint DefaultPlayerCodex;
    public CodexBlueprint FullPlayerCodex;
    public CodexBlueprint DefaultEnemyCodex;
    public CodexBlueprint FullEnemyCodex;

    public LevelBlueprint[] Levels;
    public LevelBlueprint GetNextLevel(LevelBlueprint currentLevel)
    {
        int currentIndex = Array.IndexOf(Levels, currentLevel);
        if (currentIndex >= 0 && currentIndex < Levels.Length - 1) // Ensure the current level is found and not the last one
        {
            return Levels[currentIndex + 1]; // Return the next level
        }
        return null; // Return null if currentLevel is the last one or not found
    }
}
