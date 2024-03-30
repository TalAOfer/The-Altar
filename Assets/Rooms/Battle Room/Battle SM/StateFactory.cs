using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFactory
{
    private RoomStateMachine _sm;
    private SMContext _ctx;
    public StateFactory(RoomStateMachine sm, SMContext ctx)
    {
        _sm = sm;
        _ctx = ctx;
    }

    public BaseRoomState OpenDoors()
    {
        return new Base_OpenDoors(_sm, _ctx);
    }

    #region Treasure
    public BaseRoomState SpawnTreasure()
    {
        return new Treasure_SpawnTreasure(_sm, _ctx);
    }

    public BaseRoomState OpenTreasure()
    {
        return new Treasure_OpenTreasure(_sm, _ctx);
    }

    public BaseRoomState HandlePack()
    {
        return new Treasure_HandlePack(_sm, _ctx);
    }

    #endregion

    public BaseRoomState ShowTitle()
    {
        return new First_ShowTitle(_sm, _ctx);
    }

    public BaseRoomState SpawnEnemies() 
    {
        return new Battle_EnemySetup(_sm, _ctx);
    }

    public BaseRoomState DrawHand()
    {
        return new Battle_PlayerSetup(_sm, _ctx);
    }

    public BaseRoomState Idle()
    {
        return new Battle_Idle(_sm, _ctx);
    }

    public BaseRoomState CardSelected()
    {
        return new Battle_CardSelected(_sm, _ctx);
    }

    public BaseRoomState CardSearchTarget()
    {
        return new Battle_CardSearchTarget(_sm, _ctx);
    }

    public BaseRoomState Battle()
    {
        return new Battle_Battle(_sm, _ctx);
    }

    public BaseRoomState TakeRoomDamage()
    {
        return new Battle_TakeRoomDamage(_sm, _ctx);
    }

    public BaseRoomState Lose()
    {
        return new Battle_Lost(_sm, _ctx);
    }

    public BaseRoomState AbilitySelected()
    {
        return new Battle_AbilitySelected(_sm, _ctx);
    }

    public BaseRoomState ApplyAbilityEffect()
    {
        return new Battle_ApplyAbilityEffect(_sm, _ctx);
    }

    public BaseRoomState ApplySplitAbility()
    {
        return new Battle_ApplySplitAbility(_sm, _ctx);
    }

    public BaseRoomState ApplyMergeAbility()
    {
        return new Battle_ApplyMergeAbility(_sm, _ctx);
}
}

