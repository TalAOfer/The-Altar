using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateFactory
{
    private BattleStateMachine _battleCtx;
    public BattleStateFactory(BattleStateMachine battleCtx)
    {
        _battleCtx = battleCtx;
    }

    public BaseBattleState SpawnEnemies() 
    {
        return new Battle_EnemySetup(_battleCtx);
    }

    public BaseBattleState DrawHand()
    {
        return new Battle_PlayerSetup(_battleCtx);
    }

}
