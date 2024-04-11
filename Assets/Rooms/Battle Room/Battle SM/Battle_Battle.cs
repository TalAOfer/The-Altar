using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

internal class Battle_Battle : BaseRoomState
{
    public Battle_Battle(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    private bool IsEnemyOutOfCards => _sm.EnemyCardManager.ActiveEnemies.Count == 0;
    private bool IsPlayerOutOfCards => _sm.PlayerCardManager.ActiveCards.Count == 0;


    public override IEnumerator EnterState()
    {

        yield return _sm.BattleManager.BattleRoutine();

        if (IsEnemyOutOfCards)
        {
            List<Coroutine> destructionRoutines = new();
            List<Card> CardsToDestroy = new(_sm.PlayerCardManager.ActiveCards);

            foreach (Card card in CardsToDestroy)
            {
                destructionRoutines.Add(_sm.StartCoroutine(card.DestroySelf()));
            }

            foreach (Coroutine routine in destructionRoutines)
            {
                yield return routine;
            }

            SwitchTo(States.OpenDoors());

            yield break;
        }

        else if (IsPlayerOutOfCards)
        {
            //SwitchTo(States.TakeRoomDamage());
            SwitchTo(States.Lose());
            yield break;
        }

        else
        {
            SwitchTo(States.Idle());
            yield break;
        }
    }

    public override IEnumerator ExitState()
    {
        _ctx.BattlingPlayerCard = null;
        _ctx.BattlingEnemyCard = null;
        yield return null;
    }
}