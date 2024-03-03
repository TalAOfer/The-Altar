using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomManager : MonoBehaviour
{
    public abstract void Initialize(NewFloorManager floorCtx, Room room);
    public abstract void OnRoomReady();
}
