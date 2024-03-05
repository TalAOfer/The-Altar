using System.Collections;
using System.Collections.Generic;

public class Battle_EnemySetup : BaseBattleRoomState
{
    public Battle_EnemySetup(BattleStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        yield return _ctx.StartCoroutine(SpawnEnemies());
        _ctx.SwitchState(_ctx.States.DrawHand());
    }

    public IEnumerator SpawnEnemies()
    {
        List<CardBlueprint> enemies = _ctx.BattleBlueprint.cards;

        for (int i = 0; i < enemies.Count; i++)
        {
            CardBlueprint cardBlueprint = enemies[i];
            Card enemy = _ctx.EnemyCardManager.SpawnCard(cardBlueprint);
            _ctx.EnemyCardManager.AddEnemyToManager(enemy);
        }

        foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies) 
        {
            card.transform.position = card.movement.placeHolder.position;
            yield return Tools.GetWait(0.25f);
        }
    }
}