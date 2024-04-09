using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[Serializable]
public class Floor
{
    [TableList(ShowIndexLabels = true)]
    public RoomBlueprint FirstRoom { get; private set; }
    public List<FloorLevel> Levels { get; private set; }
    public BattleRoomPool BattlePool { get; private set; }

    public Floor(FloorBlueprint floorBlueprint)
    {
        BattlePool = new(floorBlueprint.BattleRoomPool);
        FirstRoom = floorBlueprint.FirstRoom;
        Levels = new List<FloorLevel>(floorBlueprint.Levels);
    }
}


