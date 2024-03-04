using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_OpenDoors : BaseRoomState<RoomStateMachine>
{
    public Base_OpenDoors(RoomStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        _ctx.StartCoroutine(_ctx.OpenDoors());
        yield return null;
    }
}
