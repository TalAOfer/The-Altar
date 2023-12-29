using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Drag Manager")]
public class DragManager : ScriptableObject
{
    public bool isCardDragged;
    public Card draggedCard;

    public void SetDraggedCard(Card card)
    {
        if (card != null)
        {
            if (draggedCard != null)
            {
                Debug.LogError("Trying to assign a dragged card while another is still assigned");
            }
            if (isCardDragged)
            {
                Debug.LogError("Trying to assign isDragged while it's true");
            }

            isCardDragged = true;
            draggedCard = card;
        } 
        
        else
        {
            isCardDragged = false;
            draggedCard = null;
        }
    }
}
