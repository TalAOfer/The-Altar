using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCollisionDetector : MonoBehaviour
{
    public HandManager handManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CardBoundry"))
        {
            Card card = collision.transform.parent.GetComponent<Card>();
            if (card != null)
            {
                if (handManager.dragManager.isCardDragged)
                {
                    handManager.InsertCardToHandByIndex(card, card.index);
                }
            }

            else
            {
                Debug.LogError("GO tagged card boundry but collision isn't a child of card");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CardBoundry"))
        {
            Card card = collision.transform.parent.GetComponent<Card>();
            if (card != null)
            {
                handManager.RemoveCardFromHand(card);
            }

            else
            {
                Debug.LogError("GO tagged card boundry but collision isn't a child of card");
            }
        }
    }
}
