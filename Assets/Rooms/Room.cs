using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Room : MonoBehaviour
{
    public abstract void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint);

    public abstract void OnRoomFinishedLerping();
    public abstract void OnRoomFinished();
}

public enum RoomType
{
    Battle,
    CardPicking
}