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

    [ShowIf("roomType", RoomType.CardPicking)]
    public int amountOfOptions;
    [ShowIf("roomType", RoomType.CardPicking)]
    public int minDraw;
    [ShowIf("roomType", RoomType.CardPicking)]
    public int maxDraw;
}
