using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[CreateAssetMenu(menuName = "Room Data")]
public class RoomData : ScriptableObject
{
    public RoomType RoomType;

    [ShowIf("RoomType", RoomType.Battle)]
    public BattleRoomStateEnum BattleRoomState;
    [ShowIf("RoomType", RoomType.Battle)]
    public BattleRoom EnemyManager;
}

public enum BattleRoomStateEnum
{
    Setup,
    Idle,
    BattleFormation,
    Battle
}

public enum SelectionRoomState
{

}
