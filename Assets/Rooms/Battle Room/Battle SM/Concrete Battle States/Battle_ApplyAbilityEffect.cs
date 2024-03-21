using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_ApplyAbilityEffect : BaseBattleRoomState
{
    Ability Ability => _ctx.Ctx.CurrentAbilitySelected;
    List<Card> CardsSelected => _ctx.Ctx.CurrentCardsSelected;
    List<Card> PlayerCards => _ctx.PlayerCardManager.ActiveCards;
    List<Card> EnemyCards => _ctx.EnemyCardManager.ActiveEnemies;

    public Battle_ApplyAbilityEffect(BattleRoomStateMachine ctx) : base(ctx)
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

        yield return Ability.Effect.Trigger(CardsSelected);

        yield return _ctx.HandleAllShapeshiftsUntilStable();

        yield return Tools.GetWait(0.5f);

        foreach (var card in CardsSelected)
        {
            card.movement.Dehighlight();
            card.visualHandler.DisableOutline();
        }

        CardsSelected.Clear();

        _ctx.SwitchState(_ctx.States.Idle());
    }
}
