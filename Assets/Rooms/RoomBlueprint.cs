using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomBlueprint
{
    public RoomType roomType;

    [ShowIf("@ShouldShowDifficulty()")]
    public int difficulty = 3;
    [ShowIf("roomType", RoomType.Battle)]
    public bool predetermineEnemies;
    [ShowIf("@ShouldShowEnemiesForTest()")]
    public List<CardArchetype> enemyArchetypes;

    [ShowIf("roomType", RoomType.Battle)]
    public bool predetermineDeck;
    [ShowIf("@ShouldShowDeckBlueprint()")]
    public DeckInstance deck;

    [ShowIf("roomType", RoomType.CardPicking)]
    public int amountOfOptions;

    [ShowIf("roomType", RoomType.Playtest)]
    public CardOwner affinity;

    [ShowIf("roomType", RoomType.Playtest)]
    public CardArchetype archetype;

    [ShowIf("roomType", RoomType.Playtest)]
    public CardBlueprint cardBlueprint;

    [HideIf("@ShouldHidePlayerMinmax()")]
    public Vector2Int playerDrawMinMax;
    [HideIf("@ShouldHideEnemyMinmax()")]
    public Vector2Int enemyDrawMinMax;

    private bool ShouldShowEnemiesForTest()
    {
        return roomType is RoomType.Battle && predetermineEnemies;
    }

    private bool ShouldShowDeckBlueprint()
    {
        return roomType is RoomType.Battle && predetermineDeck;
    }
    
    private bool ShouldHidePlayerMinmax()
    {
        return roomType is RoomType.Playtest || predetermineDeck;
    }

    private bool ShouldHideEnemyMinmax()
    {
        return roomType is RoomType.Playtest || predetermineEnemies;
    }

    private bool ShouldShowDifficulty()
    {
        return roomType is RoomType.Battle && !predetermineEnemies;
    }
}
