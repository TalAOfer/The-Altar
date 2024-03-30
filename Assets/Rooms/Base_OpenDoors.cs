using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_OpenDoors : BaseRoomState
{
    public Base_OpenDoors(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        CoroutineRunner.Instance.StartCoroutine(_sm.OpenDoors());
        yield return null;
    }
}
