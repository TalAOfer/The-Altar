using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Floor
{
    [TableList(ShowIndexLabels = true)]
    public List<FloorLevel> Levels;
    public BattleRoomPool BattlePool { get; private set; }

    public Floor(FloorBlueprint floorBlueprint)
    {
        BattlePool = new(floorBlueprint.BattleRoomPool);

        Levels = new List<FloorLevel>(floorBlueprint.Levels);

        for (int i = 0; i < Levels.Count; i++)
        {
            Levels[i].InitializeRooms(i, Levels.Count, BattlePool);
        }
    }

}

[Serializable]
public class FloorLevel
{
    public void InitializeRooms(int totalRoomNumber, int index, BattleRoomPool battlePool)
    {
        LeftRoom.InitializeRoom(totalRoomNumber, index, battlePool);
        RightRoom.InitializeRoom(totalRoomNumber, index, battlePool);
    }

    public Room LeftRoom;
    public Room RightRoom;
}


