using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_ApplySplitAbility : BaseRoomState
{
    public Battle_ApplySplitAbility(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    List<Card> PlayerCards => _sm.PlayerCardManager.ActiveCards;
    List<Card> EnemyCards => _sm.EnemyCardManager.ActiveEnemies;
    List<Card> CardsSelected => _ctx.CurrentCardsSelected;
    Card SelectedCard => _ctx.CurrentCardsSelected[0];



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

        CardColor color = SelectedCard.cardColor;
        int firstHalf = Tools.DivideAndRoundUp(SelectedCard.points, 2);
        int secondHalf = SelectedCard.points - firstHalf;

        CardArchetype firstArchetype = new (firstHalf, color);
        CardArchetype secondArchetype = new(secondHalf, color);

        SelectedCard.gameObject.SetActive(false);
        _sm.PlayerCardManager.RemoveCardFromManager(SelectedCard);

        _sm.DataProvider.SpawnCardToHandByArchetype(firstArchetype);
        yield return new WaitForSeconds(0.25f);
        _sm.DataProvider.SpawnCardToHandByArchetype(secondArchetype);

        CardsSelected.Clear();

        SwitchTo(States.Idle());
    }

}
