using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class Battle_Idle : BaseRoomState
{
    public Battle_Idle(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        if (_ctx.CurrentActorCard != null && _ctx.CurrentActorCard.isActiveAndEnabled)
        {
            yield return Tools.GetWait(0.1f);
            if (!_ctx.CurrentActorCard.interactionHandler.IsCursorOn)
            {
                _sm.DemarkCardAsSelected(_ctx.CurrentActorCard);
            }

            _ctx.CurrentActorCard = null;
        }

        _sm.Events.OnTurnEnd.Raise();

        yield return base.EnterState();
    }

    public override void HandlePlayerCardPointerEnter(Card card, PointerEventData eventData)
    {
        Events.ShowTooltip.Raise(_sm, card);
        card.movement.Highlight();
    }

    public override void HandlePlayerCardPointerExit(Card card, PointerEventData eventData)
    {
        Events.HideTooltip.Raise(_sm, card);
        card.movement.Dehighlight();
    }

    public override void HandleEnemyCardPointerEnter(Card card, PointerEventData eventData)
    {
        Events.ShowTooltip.Raise(_sm, card);
        card.visualHandler.Jiggle();
    }

    public override void HandleEnemyCardPointerExit(Card card, PointerEventData eventData)
    {
        Events.HideTooltip.Raise(_sm, card);
    }

    #region Transitions

    public override void HandlePlayerCardPointerClick(Card cardClicked, PointerEventData eventData)
    {
        _ctx.CardClicked = cardClicked;
        _ctx.CardClicked.movement.Highlight();
        SwitchTo(States.CardSelected());
    }

    public override void HandlePlayerCardBeginDrag(Card cardDragged, PointerEventData eventData)
    {
        _ctx.CardClicked = cardDragged;
        _ctx.CardClicked.movement.Highlight();
        SwitchTo(States.CardSelected());
    }

    public override void OnAbilityClicked(AbilityManager abilityManager, Ability ability)
    {
        _ctx.CurrentAbilitySelected = ability;
        SwitchTo(States.AbilitySelected());
    }

    #endregion
}
