using System.Collections;
using System.Diagnostics;
using UnityEngine;

internal class Battle_Battle : BaseBattleRoomState
{
    private bool IsEnemyOutOfCards => _ctx.EnemyCardManager.ActiveEnemies.Count == 0;
    private bool IsPlayerOutOfCards => _ctx.PlayerCardManager.ActiveCards.Count == 0;

    public Battle_Battle(BattleRoomStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {

        yield return _ctx.BattleManager.BattleRoutine();

        if (IsEnemyOutOfCards)
        {
            _ctx.SwitchState(new Base_OpenDoors(_ctx));
            yield break;
        }

        else if (IsPlayerOutOfCards)
        {
            _ctx.SwitchState(_ctx.States.TakeRoomDamage());
            yield break;
        } 
        
        else
        {
            _ctx.SwitchState(_ctx.States.Idle());
            yield break;
        }
    }

    public override IEnumerator ExitState()
    {
        _ctx.Ctx.BattlingPlayerCard = null;
        _ctx.Ctx.BattlingEnemyCard = null;
        yield return null;
    }
}