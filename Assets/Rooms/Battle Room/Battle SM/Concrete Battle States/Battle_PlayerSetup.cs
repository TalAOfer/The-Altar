using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Battle_PlayerSetup : BaseRoomState
{
    public Battle_PlayerSetup(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    private void InitializeDeck()
    {
        RunData.playerDeck = new Deck(RunData.playerDeck.min, RunData.playerDeck.max);
    }

    public override IEnumerator EnterState()
    {
        InitializeDeck();
        yield return _sm.PlayerCardManager.SpawnCardsToHandByBlueprint(_sm.InitialPlayerSpawn);
        yield return Tools.GetWait(0.5f);
        SwitchTo(States.Idle());
        yield break;
    }


}
