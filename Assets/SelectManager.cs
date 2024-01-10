using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    private Card selectedCard = null;

    [SerializeField] private HandManager handManager;
    [SerializeField] private Transform askerTransform;
    [SerializeField] private Transform selectTransform;

    public void OnCardClicked(Component sender, object data)
    {
        Card cardClicked = data as Card;
        if (cardClicked == selectedCard)
        {
            handManager.InsertCardToHandByIndex(cardClicked, cardClicked.index);
            selectedCard = null;
        }
        
        else
        {
            if (selectedCard != null)
            {
                handManager.InsertCardToHandByIndex(selectedCard, selectedCard.index);
            }

            selectedCard = cardClicked;

            handManager.cardsInHand.Remove(cardClicked);
            StartCoroutine(cardClicked.interactionHandler.TransformCardUniformly
                (selectTransform.position, Vector3.one * GameConstants.SelectCardScale, Vector3.zero, 0.15f));
        }

        handManager.ReorderCards();
    }
}
