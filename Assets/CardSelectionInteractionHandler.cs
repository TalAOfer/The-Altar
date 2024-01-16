using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectionInteractionHandler : CardInteractionBase
{
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private float hoverHeightBoostAmount;
    [SerializeField] private CardSelectionRoom room;
    protected override void HandlePointerEnter(Card card, PointerEventData eventData)
    {
        card.visualHandler.SetCardBGColor(hoverColor);

        events.ShowTooltip.Raise(this, card);

        LinkedCards linkedCards = room.linkedCardsList[card.index];

        //Boost height
        Card playerCard = linkedCards.playerCard;
        Card enemyCard = linkedCards.enemyCard;

        Vector3 temp = playerCard.interactionHandler.defaultPos;
        temp.y += hoverHeightBoostAmount;
        playerCard.transform.position = temp;

        temp = enemyCard.interactionHandler.defaultPos;
        temp.y += hoverHeightBoostAmount;
        enemyCard.transform.position = temp;
    }

    protected override void HandlePointerExit(Card card, PointerEventData eventData)
    {
        card.visualHandler.SetCardBGColor(defaultColor);

        events.HideTooltip.Raise(this, card);

        LinkedCards linkedCards = room.linkedCardsList[card.index];

        linkedCards.playerCard.interactionHandler.RestartTransformToDefault();
        linkedCards.enemyCard.interactionHandler.RestartTransformToDefault();
    }

    protected override void HandlePointerClick(Card card, PointerEventData eventData)
    {
        room.HandleChoice(card.index);
    }
}
