using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomBlueprint
{
    public RoomType roomType;

    [ShowIf("roomType", RoomType.Battle)]
    public int difficulty;
    [ShowIf("roomType", RoomType.Battle)]
    public bool isEnemyTest;
    [ShowIf("@ShouldShowEnemiesForTest()")]
    public List<CardBlueprint> enemiesForTest;

    [ShowIf("roomType", RoomType.CardPicking)]
    public int amountOfOptions;

    public Vector2Int playerDrawMinMax;
    public Vector2Int enemyDrawMinMax;

    private bool ShouldShowEnemiesForTest()
    {
        return roomType is RoomType.Battle && isEnemyTest;
    }
}
