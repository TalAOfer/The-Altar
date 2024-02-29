using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectionInteractionHandler : CardInteractionStateMachine
{
    [SerializeField] private CardSelectionRoom room;

    protected override void HandlePointerEnter(Card card, PointerEventData eventData)
    {
        events.ShowTooltip.Raise(this, card);

        //switch (card.cardInteractionType)
        //{
        //    case CardInteractionType.Playable:
        //        card.movement.Highlight();
        //        break;
        //    case CardInteractionType.Selection:
        //        card.visualHandler.Animate("Jiggle");
        //        //LinkedCards linkedCards = room.linkedCardsList[card.index];
        //        if (!linkedCards.wasChosen)
        //        {
        //            linkedCards.wasChosen = true;
        //            linkedCards.OutlineCards();
        //            linkedCards.ToggleSelectionArrows(true);
        //        }
        //        break;
        //    case CardInteractionType.Codex:
        //        break;
        //}
    }

    protected override void HandlePointerExit(Card card, PointerEventData eventData)
    {
        events.HideTooltip.Raise(this, card);

        //switch (card.cardInteractionType)
        //{
        //    case CardInteractionType.Playable:
        //        card.movement.Dehighlight();
        //        break;
        //    case CardInteractionType.Selection:
        //        //LinkedCards linkedCards = room.linkedCardsList[card.index];
        //        if (!linkedCards.IsPointerOverLinkCollider && linkedCards.wasChosen)
        //        {
        //            linkedCards.wasChosen = false;
        //            linkedCards.DeOutlineCards();
        //            linkedCards.ToggleSelectionArrows(false);
        //        }
        //        break;
        //    case CardInteractionType.Codex:
        //        break;
        //}
    }

    protected override void HandlePointerClick(Card card, PointerEventData eventData)
    {
        if (card.cardInteractionType is CardInteractionType.Selection)
        {
            //room.HandleChoice(card.index);
        }
    }

    public void OnLinkPointerEnter(Component sender, object data)
    {
        //LinkedCards linkedCards = (LinkedCards)sender;
        //linkedCards.IsPointerOverLinkCollider = true;

        //if (!linkedCards.wasChosen)
        //{
        //    linkedCards.wasChosen = true;
        //    linkedCards.OutlineCards();
        //    linkedCards.ToggleSelectionArrows(true);
        //}
    }

    public void OnLinkPointerExit(Component sender, object data)
    {
        //LinkedCards linkedCards = (LinkedCards)sender;
        //linkedCards.IsPointerOverLinkCollider = false;

        //if (linkedCards.wasChosen)
        //{
        //    linkedCards.wasChosen = false;
        //    linkedCards.DeOutlineCards();
        //    linkedCards.ToggleSelectionArrows(false);
        //}
    }

    public void OnLinkClick(Component sender, object data)
    {
        //LinkedCards linkedCards = (LinkedCards)sender;
        //room.HandleChoice(linkedCards.index);
    }
}
