using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public List<Card> cardsInHand = new();
    public float handCardScale = 1.25f;

    public int maxCardAmount;
    public float baseSpacing = 1.5f;
    public float baseRotationAngle = 20f;
    public Vector2 yOffsetFactorMinMax = new(0.1f, 0.3f);
    public DragManager dragManager;

    private void Awake()
    {
        dragManager.SetDraggedCard(null);
    }

    public void AddCardToHand(Card card)
    {
        card.transform.SetParent(transform);
        cardsInHand.Add(card);
        ReorderCards();
    }

    public void InsertCardToHandByIndex(Card card, int index)
    {
        cardsInHand.Insert(index, card);
        ReorderCards();
    }

    public void SwitchCards(Card incomingCard, Card existingCard)
    {
        int incomingIndex = cardsInHand.IndexOf(incomingCard);
        int existingIndex = cardsInHand.IndexOf(existingCard);

        if (incomingIndex != -1 && existingIndex != -1)
        {
            // Swap the cards in the list
            (cardsInHand[existingIndex], cardsInHand[incomingIndex]) = (cardsInHand[incomingIndex], cardsInHand[existingIndex]);
            ReorderCards();
        }


        else
        {
            // Handle the case where one or both cards are not in the list
            Debug.Log("One or both of the cards are not in the list.");
        }
    }

    public void OnDraggedCardHoveredOverHandCard(Component sender, object data)
    {
        Card hoveredCard = sender as Card;
        Card draggedCard = data as Card;
        if (cardsInHand.Contains(draggedCard))
        {
            SwitchCards(draggedCard, hoveredCard);
        }

        else
        {
            InsertCardToHandByIndex(draggedCard, hoveredCard.index);
        }

    }

    public void InsertCardInIndex(Component sender, object data)
    {
        Card card = data as Card;

        cardsInHand.Insert(card.index, card);
        ReorderCards();
    }

    public void RemoveCardFromHand(Component sender, object data)
    {
        Card card = data as Card;

        cardsInHand.Remove(card);
        ReorderCards();
    }

    [Button]
    public void ReorderCards()
    {
        int numOfCards = cardsInHand.Count;

        if (numOfCards == 1)
        {
            // Place the only card in the middle
            cardsInHand[0].transform.localPosition = new Vector3(0, 0, 0);
            cardsInHand[0].transform.localRotation = Quaternion.Euler(0, 0, 0);
            cardsInHand[0].interactionHandler.SetNewDefaultLocation();
        }

        else
        {
            float dynamicSpacing = baseSpacing;
            float dynamicYOffsetFactor = Mathf.Lerp(yOffsetFactorMinMax.x, yOffsetFactorMinMax.y, (numOfCards - 1) / (float)maxCardAmount);

            float dynamicRotationAngle = Mathf.Lerp(0, baseRotationAngle, (numOfCards - 1) / (float)maxCardAmount);

            for (int i = 0; i < numOfCards; i++)
            {
                cardsInHand[i].index = i;
                if (cardsInHand[i] == dragManager.draggedCard)  continue;
                float xPos = (i - (numOfCards - 1) / 2f) * dynamicSpacing;
                float yPos = CalculateYPosition(i, numOfCards, dynamicYOffsetFactor);
                cardsInHand[i].transform.localPosition = new Vector3(xPos, yPos, 0);


                // Calculate rotation angle for this card
                float rotationFactor = (float)i / (numOfCards - 1);
                float angle = Mathf.Lerp(dynamicRotationAngle, -dynamicRotationAngle, rotationFactor);
                cardsInHand[i].transform.localRotation = Quaternion.Euler(0, 0, angle);

                cardsInHand[i].visualHandler.SetSortingOrder(i);
                cardsInHand[i].interactionHandler.SetNewDefaultLocation();
            }
        }
    }

    private float CalculateYPosition(int index, int totalCards, float yOffsetFactor)
    {
        // Normalize index position
        float normalizedIndex = (float)index / (totalCards - 1);

        // Apply a quadratic curve (parabola) for Y position
        return yOffsetFactor * (4 * normalizedIndex * (1 - normalizedIndex) - 1);
    }
}
