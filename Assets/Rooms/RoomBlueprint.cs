using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Room")]
public class RoomBlueprint : ScriptableObject
{
    public RoomType roomType;

    [ShowIf("roomType", RoomType.Battle)]
    public int difficulty;

    [ShowIf("roomType", RoomType.CardPicking)]
    public int minDraw;
    [ShowIf("roomType", RoomType.CardPicking)]
    public int maxDraw;
}
