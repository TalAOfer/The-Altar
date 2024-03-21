using System;

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


