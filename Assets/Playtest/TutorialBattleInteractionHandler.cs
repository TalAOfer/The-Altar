using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialBattleInteractionHandler : BattleInteractionHandler
{
    public override void Highlight(Card card)
    {
        bool isOnTop = card.visualHandler.GetSortingLayer() == "Top";
        base.Highlight(card);
        if (isOnTop)
        {
            card.visualHandler.SetSortingLayer("Top");
        }
    }
    public override void Dehighlight(Card card)
    {
        bool isOnTop = card.visualHandler.GetSortingLayer() == "Top";
        base.Dehighlight(card);
        if (isOnTop)
        {
            card.visualHandler.SetSortingLayer("Top");
        }
    }

    protected override void HandlePointerEnter(Card card, PointerEventData eventData)
    {
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;
        if (card.cardState != CardState.Default) return;

        bool isThisCardAPlayerCard = card.cardOwner == CardOwner.Player;

        if (isThisCardAPlayerCard)
        {
            Highlight(card);
        }

        else
        {
            card.visualHandler.Animate("Jiggle");
        }

        events.ShowTooltip.Raise(this, card);
    }
}
