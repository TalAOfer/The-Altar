using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Battle_PlayerSetup : BaseBattleRoomState
{
    public Battle_PlayerSetup(BattleStateMachine ctx) : base(ctx)
    {
    }
    private void InitializeDeck()
    {
        _ctx.FloorCtx.RunData.playerDeck = new Deck(FloorCtx.RunData.playerDeck.min, FloorCtx.RunData.playerDeck.max);
    }

    public override IEnumerator EnterState()
    {
        InitializeDeck();
        yield return _ctx.StartCoroutine(_ctx.PlayerCardManager.DrawCardsToHand(3));
        yield break;
    }


}
