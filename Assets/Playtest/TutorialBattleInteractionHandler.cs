using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
