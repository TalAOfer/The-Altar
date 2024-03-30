using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_ApplyMergeAbility : BaseRoomState
{
    public Battle_ApplyMergeAbility(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    List<Card> PlayerCards => _sm.PlayerCardManager.ActiveCards;
    List<Card> EnemyCards => _sm.EnemyCardManager.ActiveEnemies;
    List<Card> CardsSelected => _ctx.CurrentCardsSelected;
    Card FirstCardSelected => CardsSelected[0];
    Card SecondCardSelected => CardsSelected[1];

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

        _sm.PlayerCardManager.RemoveCardFromManager(FirstCardSelected);
        _sm.PlayerCardManager.RemoveCardFromManager(SecondCardSelected);

        _sm.DataProvider.SpawnCardToHandByArchetype(mergeArchetype);

        CardsSelected.Clear();

        SwitchTo(States.Idle());

        yield break;
    }


}
