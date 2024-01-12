using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<Card> activeCards = new();

    public void SetAllCardState(CardState newState)
    {
        foreach (Card card in activeCards)
        {
            card.ChangeCardState(newState);
        }
    }
}
