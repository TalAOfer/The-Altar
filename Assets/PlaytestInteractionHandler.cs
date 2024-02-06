using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaytestInteractionHandler : CardInteractionBase
{
    [SerializeField] private PlaytestRoom room;
    protected override void HandlePointerEnter(Card card, PointerEventData eventData)
    {
        events.ShowTooltip.Raise(this, card);
        card.visualHandler.Animate("Jiggle");
    }

    protected override void HandlePointerExit(Card card, PointerEventData eventData)
    {
        events.HideTooltip.Raise(this, card);
    }
}
