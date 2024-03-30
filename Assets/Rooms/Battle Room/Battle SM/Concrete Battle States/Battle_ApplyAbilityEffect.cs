using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_ApplyAbilityEffect : BaseRoomState
{
    public Battle_ApplyAbilityEffect(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    Ability Ability => _ctx.CurrentAbilitySelected;
    List<Card> CardsSelected => _ctx.CurrentCardsSelected;
    List<Card> PlayerCards => _sm.PlayerCardManager.ActiveCards;
    List<Card> EnemyCards => _sm.EnemyCardManager.ActiveEnemies;


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

        yield return _sm.HandleAllShapeshiftsUntilStable();

        yield return Tools.GetWait(0.5f);

        foreach (var card in CardsSelected)
        {
            card.movement.Dehighlight();
            card.visualHandler.DisableOutline();
        }

        CardsSelected.Clear();

        _sm.SwitchState(_sm.States.Idle());
    }
}
