using System;

[Serializable]
public class FloorLevel
{
    public void InitializeRooms(BattleRoomPool battlePool)
    {
        LeftRoom.InitializeRoom(battlePool);
        RightRoom.InitializeRoom(battlePool);
    }

    public Room LeftRoom;
    public Room RightRoom;
}


