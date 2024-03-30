using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class Battle_CardSelected : BaseRoomState
{
    public Battle_CardSelected(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    private Card CurrentlySelectedCard => _ctx.CurrentActorCard;
    private Card CardClicked => _ctx.CardClicked;


    public override IEnumerator EnterState()
    {

        bool isACardCurrentlySelected = CurrentlySelectedCard != null;

        if (isACardCurrentlySelected)
        {
            _sm.DemarkCardAsSelected(CurrentlySelectedCard);

            bool isClickedCardIsCurrentlySelected = CardClicked == CurrentlySelectedCard;

            if (isClickedCardIsCurrentlySelected)
            {
                _ctx.CurrentActorCard = null;
                SwitchTo(States.Idle());
            } 
            
            else
            {
                _ctx.CurrentActorCard = CardClicked;
            }
        }

        else
        {
            _sm.MarkCardAsSelected(CardClicked);
            _ctx.CurrentActorCard = CardClicked;
        }

        _ctx.CardClicked = null;

        return base.EnterState();
    }

    public override void HandlePlayerCardPointerEnter(Card card, PointerEventData eventData)
    {
        Events.ShowTooltip.Raise(_sm, card);

        card.movement.Highlight();
    }

    public override void HandlePlayerCardPointerExit(Card card, PointerEventData eventData)
    {
        Events.HideTooltip.Raise(_sm, card);

        bool isThisCardSelected = CurrentlySelectedCard == card;
        if (isThisCardSelected) return;

        card.movement.Dehighlight();
    }

    public override void HandlePlayerCardPointerClick(Card cardClicked, PointerEventData eventData)
    {
        _ctx.CardClicked = cardClicked;

        SwitchTo(States.CardSelected());
    }

    public override void HandlePlayerCardBeginDrag(Card cardDragged, PointerEventData eventData)
    {
        _ctx.CardClicked = cardDragged;

        SwitchTo(States.CardSelected());
    }

    public override void OnHandColliderPointerExit(HandCollisionDetector HandCollisionManager, PointerEventData data)
    {
        SwitchTo(States.CardSearchTarget());
    }


}
