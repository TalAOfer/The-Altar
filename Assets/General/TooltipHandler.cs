using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipHandler : MonoBehaviour
{
    [SerializeField] private Tooltip tooltip;
    [SerializeField] private float xOffset = 150;
    [SerializeField] private float yOffset = 100;
    [SerializeField] private float yCardPosOffsetTrigger = -6;
    public void ShowTooltip(Component sender, object data)
    {
        Card card = (Card)data;
        tooltip.InitializeTooltip(card);
        tooltip.gameObject.SetActive(true);

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(card.visualHandler.transform.position);
        screenPosition.x += xOffset;
        
        if (card.visualHandler.transform.position.y < yCardPosOffsetTrigger)
        {
            screenPosition.y += yOffset;
        }

        tooltip.GetComponent<RectTransform>().position = screenPosition;
    }

    public void HideTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }
}
