using System.Collections;
using System.Collections.Generic;

public class Battle_EnemySetup : BaseRoomState
{
    public Battle_EnemySetup(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        yield return SpawnEnemies();
        SwitchTo(States.DrawHand());
    }

    public IEnumerator SpawnEnemies()
    {
        List<CardBlueprint> enemies = _sm.InitialEnemySpawn;

        yield return _sm.EnemyCardManager.SpawnEnemies(enemies);
    }

}