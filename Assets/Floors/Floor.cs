using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

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


