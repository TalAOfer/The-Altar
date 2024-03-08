using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_TakeRoomDamage : BaseBattleRoomState
{
    public Battle_TakeRoomDamage(BattleStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        AttackPlayer();

        if (_ctx.FloorCtx.RunData.PlayerHealth.Current > 0)
        {
            _ctx.SwitchState(_ctx.States.DrawHand());
        }

        return base.EnterState();
    }

    private void AttackPlayer()
    {
        List<Card> enemyCardsLeft = new(_ctx.EnemyCardManager.ActiveEnemies);
        int damage = 0;
        foreach (Card enemy in enemyCardsLeft)
        {
            damage += enemy.points;
        }

        _ctx.FloorCtx.RunData.TakeGlobalDamage(damage);
    }
}
