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

    protected override void HandlePointerEnter(Card card, PointerEventData eventData)
    {
        card.visualHandler.SetCardBGColor(hoverColor);

        events.ShowTooltip.Raise(this, card);

        //Boost height
        Vector3 temp = card.interactionHandler.defaultPos;
        temp.y += hoverHeightBoostAmount;
        card.transform.position = temp;
    }

    protected override void HandlePointerExit(Card card, PointerEventData eventData)
    {
        card.visualHandler.SetCardBGColor(defaultColor);

        events.HideTooltip.Raise(this, card);

        //Boost height
        card.interactionHandler.RestartTransformToDefault();
    }

    protected override void HandlePointerClick(Card card, PointerEventData eventData)
    {

    }
}
