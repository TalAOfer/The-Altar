using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Lost : BaseRoomState
{
    public Battle_Lost(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        _sm.LoseManager.CloseWalls();
        return base.EnterState();
    }
}
