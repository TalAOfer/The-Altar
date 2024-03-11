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


    public BaseBattleRoomState SpawnEnemies() 
    {
        return new Battle_EnemySetup(_battleCtx);
    }

    public BaseBattleRoomState DrawHand()
    {
        return new Battle_PlayerSetup(_battleCtx);
    }

    public BaseBattleRoomState Idle()
    {
        return new Battle_Idle(_battleCtx);
    }

    public BaseBattleRoomState CardSelected()
    {
        return new Battle_CardSelected(_battleCtx);
    }

    public BaseBattleRoomState CardSearchTarget()
    {
        return new Battle_CardSearchTarget(_battleCtx);
    }

    public BaseBattleRoomState Battle()
    {
        return new Battle_Battle(_battleCtx);
    }

    public BaseBattleRoomState TakeRoomDamage()
    {
        return new Battle_TakeRoomDamage(_battleCtx);
    }

    public BaseBattleRoomState Lose()
    {
        return new Battle_Lost(_battleCtx);
    }

    public BaseBattleRoomState AbilitySelected()
    {
        return new Battle_AbilitySelected(_battleCtx);
    }

    public BaseBattleRoomState ApplyAbilityEffect()
    {
        return new Battle_ApplyAbilityEffect(_battleCtx);
    }

    public BaseBattleRoomState ApplySplitAbility()
    {
        return new Battle_ApplySplitAbility(_battleCtx);
    }

    public BaseBattleRoomState ApplyMergeAbility()
    {
        return new Battle_ApplyMergeAbility(_battleCtx);
}
}

