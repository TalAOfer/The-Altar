using System.Collections.Generic;

public class Floor
{
    public Room FirstRoom { get; private set; }
    public List<FloorLevel> Levels { get; private set; }
    public Room FinalRoom;

    public BattleRoomPool BattlePool { get; private set; }

    public Floor(BattleRoomPoolAsset poolBlueprint)
    {
        BattlePool = new(poolBlueprint);
    }
}

public class Room
{
    public Room(RoomBlueprint blueprint, BattleRoomPool pool)
    {
        
    }

    public BattleBlueprint BattleBlueprint { get; private set; }
    public Room LeftDoor { get; private set; }
    public Room RightDoor { get; private set; }
}

public class FloorLevel
{
    public Room LeftRoom { get; private set; }
    public Room RightRoom { get; private set; }
}
