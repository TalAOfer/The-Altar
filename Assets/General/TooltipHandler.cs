using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipHandler : MonoBehaviour
{
    [SerializeField] Tooltip tooltip;
    public void ShowTooltip(Component sender, object data)
    {
        Card card = (Card)data;
        tooltip.InitializeTooltip(card);
        tooltip.gameObject.SetActive(true);
    }
    public void HideTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }
}
