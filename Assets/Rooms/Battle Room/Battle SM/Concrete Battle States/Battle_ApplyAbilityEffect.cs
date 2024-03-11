using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_ApplyAbilityEffect : BaseBattleRoomState
{
    Ability Ability => _ctx.Ctx.CurrentAbilitySelected;
    List<Card> CardsSelected => _ctx.Ctx.CurrentCardsSelected;

    public Battle_ApplyAbilityEffect(BattleStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        foreach (var card in CardsSelected)
        {
            yield return Ability.Effect.ApplyEffect(card);
        }

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
