using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_ApplySplitAbility : BaseBattleRoomState
{
    List<Card> PlayerCards => _ctx.PlayerCardManager.ActiveCards;
    List<Card> EnemyCards => _ctx.EnemyCardManager.ActiveEnemies;
    List<Card> CardsSelected => _ctx.Ctx.CurrentCardsSelected;
    Card SelectedCard => _ctx.Ctx.CurrentCardsSelected[0];

    public Battle_ApplySplitAbility(BattleRoomStateMachine ctx) : base(ctx)
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

        CardColor color = SelectedCard.cardColor;
        int firstHalf = Tools.DivideAndRoundUp(SelectedCard.points, 2);
        int secondHalf = SelectedCard.points - firstHalf;

        CardArchetype firstArchetype = new (firstHalf, color);
        CardArchetype secondArchetype = new(secondHalf, color);

        SelectedCard.gameObject.SetActive(false);
        _ctx.PlayerCardManager.RemoveCardFromManager(SelectedCard);

        _ctx.DataProvider.SpawnCardToHandByArchetype(firstArchetype);
        yield return new WaitForSeconds(0.25f);
        _ctx.DataProvider.SpawnCardToHandByArchetype(secondArchetype);

        CardsSelected.Clear();

        _ctx.SwitchState(_ctx.States.Idle());
    }

}
