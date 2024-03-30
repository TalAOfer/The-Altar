using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class Battle_Idle : BaseBattleRoomState
{
    public Battle_Idle(BattleRoomStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        if (_ctx.Ctx.CurrentActorCard != null && _ctx.Ctx.CurrentActorCard.isActiveAndEnabled)
        {
            yield return Tools.GetWait(0.1f);
            if (!_ctx.Ctx.CurrentActorCard.interactionHandler.IsCursorOn)
            {
                _ctx.DemarkCardAsSelected(_ctx.Ctx.CurrentActorCard);
            }

            _ctx.Ctx.CurrentActorCard = null;
        }

        _ctx.Events.OnTurnEnd.Raise();

        yield return base.EnterState();
    }

    public override void HandlePlayerCardPointerEnter(Card card, PointerEventData eventData)
    {
        _ctx.Events.ShowTooltip.Raise(_ctx, card);
        card.movement.Highlight();
    }

    public override void HandlePlayerCardPointerExit(Card card, PointerEventData eventData)
    {
        _ctx.Events.HideTooltip.Raise(_ctx, card);
        card.movement.Dehighlight();
    }

    public override void HandleEnemyCardPointerEnter(Card card, PointerEventData eventData)
    {
        _ctx.Events.ShowTooltip.Raise(_ctx, card);
        card.visualHandler.Animate("Jiggle");
    }

    public override void HandleEnemyCardPointerExit(Card card, PointerEventData eventData)
    {
        _ctx.Events.HideTooltip.Raise(_ctx, card);
    }

    #region Transitions

    public override void HandlePlayerCardPointerClick(Card cardClicked, PointerEventData eventData)
    {
        _ctx.Ctx.CardClicked = cardClicked;
        _ctx.Ctx.CardClicked.movement.Highlight();
        _ctx.SwitchState(_ctx.States.CardSelected());
    }

    public override void HandlePlayerCardBeginDrag(Card cardDragged, PointerEventData eventData)
    {
        _ctx.Ctx.CardClicked = cardDragged;
        _ctx.Ctx.CardClicked.movement.Highlight();
        _ctx.SwitchState(_ctx.States.CardSelected());
    }

    public override void OnAbilityClicked(AbilityManager abilityManager, Ability ability)
    {
        _ctx.Ctx.CurrentAbilitySelected = ability;
        _ctx.SwitchState(_ctx.States.AbilitySelected());
    }

    #endregion
}
