using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Lost : BaseRoomState
{
    public Battle_Lost(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        //_sm.LoseManager.CloseWalls();
        yield return Tools.GetWait(0.5f);
        List<Coroutine> destructionRoutines = new();
        List<Card> CardsToDestroy = new(_sm.EnemyCardManager.ActiveEnemies);

        foreach (Card card in CardsToDestroy)
        {
            destructionRoutines.Add(_sm.StartCoroutine(card.DestroySelf()));
        }

        foreach (Coroutine routine in destructionRoutines)
        {
            yield return routine;
        }

        SwitchTo(States.OpenDoors());
    }
}
