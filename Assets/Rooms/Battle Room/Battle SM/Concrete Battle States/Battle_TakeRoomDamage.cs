using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_TakeRoomDamage : BaseRoomState
{
    public Battle_TakeRoomDamage(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        AttackPlayer();

        if (FloorCtx.RunData.PlayerHealth.Current > 0)
        {
            SwitchTo(States.DrawHand());
        }

        return base.EnterState();
    }

    private void AttackPlayer()
    {
        List<Card> enemyCardsLeft = new(_sm.EnemyCardManager.ActiveEnemies);
        int damage = 0;
        foreach (Card enemy in enemyCardsLeft)
        {
            damage += enemy.points;
        }

        FloorCtx.RunData.TakeGlobalDamage(damage);
    }
}
