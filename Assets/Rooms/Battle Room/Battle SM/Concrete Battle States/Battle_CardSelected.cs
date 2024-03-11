using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class Battle_CardSelected : BaseBattleRoomState
{
    public Battle_CardSelected(BattleStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        Card currentlySelectedCard = _ctx.Ctx.CurrentActorCard;
        bool isACardCurrentlySelected = currentlySelectedCard != null;
        Card cardClicked = _ctx.Ctx.CardClicked;

        if (isACardCurrentlySelected)
        {
            _ctx.DeselectCurrentCard();
        }

        _ctx.SelectCard(cardClicked);

        return base.EnterState();
    }

    public override void HandlePlayerCardPointerEnter(Card card, PointerEventData eventData)
    {
        _ctx.Events.ShowTooltip.Raise(_ctx, card);

        bool isThisCardAPlayerCard = card.Affinity == Affinity.Player;

        if (isThisCardAPlayerCard)
            card.movement.Highlight();

        else
            card.visualHandler.Animate("Jiggle");
    }

    public override void HandlePlayerCardPointerExit(Card card, PointerEventData eventData)
    {
        _ctx.Events.HideTooltip.Raise(_ctx, card);

        bool isThisCardSelected = _ctx.Ctx.CurrentActorCard == card;
        if (isThisCardSelected) return;

        card.movement.Dehighlight();
    }
    
    public override void HandlePlayerCardPointerClick(Card cardClicked, PointerEventData eventData)
    {
        bool isCardClickedAlreadySelected = cardClicked == _ctx.Ctx.CurrentActorCard;

        if (isCardClickedAlreadySelected)
        {
            _ctx.DeselectCurrentCard();
            _ctx.SwitchState(_ctx.States.Idle());
        }

        else
        {
            _ctx.Ctx.CardClicked = cardClicked;
            _ctx.SwitchState(_ctx.States.CardSelected());
        }
    }

    public override void OnHandColliderPointerExit(HandCollisionDetector HandCollisionManager, PointerEventData data)
    {
        _ctx.SwitchState(_ctx.States.CardSearchTarget());
    }

    
}
