using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Idle : BaseBattleRoomState
{
    public Battle_Idle(BattleStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        _ctx.InteractionHandler.SetState(BattleInteractionStates.Idle);
        yield break;
    }
}
