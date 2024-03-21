using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Battle_PlayerSetup : BaseBattleRoomState
{
    public Battle_PlayerSetup(BattleRoomStateMachine ctx) : base(ctx)
    {
    }
    private void InitializeDeck()
    {
        _ctx.FloorCtx.RunData.playerDeck = new Deck(FloorCtx.RunData.playerDeck.min, FloorCtx.RunData.playerDeck.max);
    }

    public override IEnumerator EnterState()
    {
        InitializeDeck();
        yield return _ctx.PlayerCardManager.DrawCardsToHand(3);
        yield return Tools.GetWait(0.5f);
        _ctx.SwitchState(_ctx.States.Idle());
        yield break;
    }


}