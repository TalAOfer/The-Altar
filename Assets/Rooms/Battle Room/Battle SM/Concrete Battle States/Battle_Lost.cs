using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Lost : BaseBattleRoomState
{
    public Battle_Lost(BattleStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        _ctx.LoseManager.CloseWalls();
        return base.EnterState();
    }
}
