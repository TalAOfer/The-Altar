using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class Battle_CardSelected : BaseBattleRoomState
{
    private Card CurrentlySelectedCard => _ctx.Ctx.CurrentActorCard;
    private Card CardClicked => _ctx.Ctx.CardClicked;
    public Battle_CardSelected(BattleStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {

        bool isACardCurrentlySelected = CurrentlySelectedCard != null;

        if (isACardCurrentlySelected)
        {
            _ctx.DemarkCardAsSelected(CurrentlySelectedCard);

            bool isClickedCardIsCurrentlySelected = CardClicked == CurrentlySelectedCard;

            if (isClickedCardIsCurrentlySelected)
            {
                _ctx.Ctx.CurrentActorCard = null;
                _ctx.SwitchState(_ctx.States.Idle());
            } 
            
            else
            {
                _ctx.Ctx.CurrentActorCard = CardClicked;
            }
        }

        else
        {
            _ctx.MarkCardAsSelected(CardClicked);
            _ctx.Ctx.CurrentActorCard = CardClicked;
        }

        _ctx.Ctx.CardClicked = null;

        return base.EnterState();
    }

    public override void HandlePlayerCardPointerEnter(Card card, PointerEventData eventData)
    {
        _ctx.Events.ShowTooltip.Raise(_ctx, card);

        card.movement.Highlight();
    }

    public override void HandlePlayerCardPointerExit(Card card, PointerEventData eventData)
    {
        _ctx.Events.HideTooltip.Raise(_ctx, card);

        bool isThisCardSelected = CurrentlySelectedCard == card;
        if (isThisCardSelected) return;

        card.movement.Dehighlight();
    }

    public override void HandlePlayerCardPointerClick(Card cardClicked, PointerEventData eventData)
    {
        _ctx.Ctx.CardClicked = cardClicked;

        _ctx.SwitchState(_ctx.States.CardSelected());
    }

    public override void HandlePlayerCardBeginDrag(Card cardDragged, PointerEventData eventData)
    {
        _ctx.Ctx.CardClicked = cardDragged;

        _ctx.SwitchState(_ctx.States.CardSelected());
    }

    public override void OnHandColliderPointerExit(HandCollisionDetector HandCollisionManager, PointerEventData data)
    {
        _ctx.SwitchState(_ctx.States.CardSearchTarget());
    }


}
