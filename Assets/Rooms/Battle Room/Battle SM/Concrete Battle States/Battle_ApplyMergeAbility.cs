using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_ApplyMergeAbility : BaseBattleRoomState
{
    List<Card> PlayerCards => _ctx.PlayerCardManager.ActiveCards;
    List<Card> EnemyCards => _ctx.EnemyCardManager.ActiveEnemies;
    List<Card> CardsSelected => _ctx.Ctx.CurrentCardsSelected;
    Card FirstCardSelected => CardsSelected[0];
    Card SecondCardSelected => CardsSelected[1];

    public Battle_ApplyMergeAbility(BattleStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        foreach (Card card in PlayerCards)
        {
            card.visualHandler.ToggleDarkOverlay(false);
        }

        foreach (Card card in EnemyCards)
        {
            card.visualHandler.ToggleDarkOverlay(false);
        }

        CardColor color = FirstCardSelected.cardColor;
        int total = FirstCardSelected.points + SecondCardSelected.points;

        CardArchetype mergeArchetype = new(total, color);

        FirstCardSelected.gameObject.SetActive(false);
        SecondCardSelected.gameObject.SetActive(false);

        _ctx.PlayerCardManager.RemoveCardFromManager(FirstCardSelected);
        _ctx.PlayerCardManager.RemoveCardFromManager(SecondCardSelected);

        _ctx.DataProvider.SpawnCardToHandByArchetype(mergeArchetype);

        CardsSelected.Clear();

        _ctx.SwitchState(_ctx.States.Idle());

        yield break;
    }


}
