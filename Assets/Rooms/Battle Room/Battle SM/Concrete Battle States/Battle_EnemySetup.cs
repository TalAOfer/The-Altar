using System.Collections;
using System.Collections.Generic;

public class Battle_EnemySetup : BaseBattleRoomState
{
    public Battle_EnemySetup(BattleRoomStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        yield return SpawnEnemies();
        _ctx.SwitchState(_ctx.States.DrawHand());
    }

    public IEnumerator SpawnEnemies()
    {
        List<CardBlueprint> enemies = _ctx.BattleBlueprint.cards;

        yield return _ctx.EnemyCardManager.SpawnEnemies(enemies);
    }

}