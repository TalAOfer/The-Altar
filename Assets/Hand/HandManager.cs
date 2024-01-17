using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public List<Card> cardsInHand = new();
    [SerializeField] private int maxCardAmount = 5;
    //Hand Fan Formation
    [SerializeField] private HandData formationData;
    private float baseSpacing;
    private float baseRotationAngle;
    private Vector2 yOffsetFactorMinMax;

    public DragManager dragManager;
    private Vector3 startingPos;
    private HandState state;
    [SerializeField] private AllEvents events;

    private void Awake()
    {
        dragManager.SetDraggedCard(null);
        startingPos = transform.position;
        baseSpacing = formationData.baseSpacing;
        baseRotationAngle = formationData.baseRotationAngle;
        yOffsetFactorMinMax = formationData.yOffsetFactorMinMax;
    }

    public void ChangeHandState(HandState newState)
    {
        if (state == newState) return;

        state = newState;

        switch (newState)
        {
            case HandState.Idle:
                //Handle pos
                transform.position = startingPos;

                //Handle formation
                baseSpacing = formationData.baseSpacing;
                baseRotationAngle = formationData.baseRotationAngle;
                yOffsetFactorMinMax = formationData.yOffsetFactorMinMax;

                //Handle SortingLayers
                foreach (Card card in cardsInHand)
                {
                    card.ChangeCardState(CardState.Default);
                }
                break;
            case HandState.Select:
                //Handle pos
                transform.position = new Vector3(startingPos.x, startingPos.y + formationData.selectStateHeightIncrease, startingPos.z);

                //Handle formation
                baseSpacing = 2;
                baseRotationAngle = 0;
                yOffsetFactorMinMax = Vector2.zero;

                //Handle SortingLayers
                foreach (Card card in cardsInHand)
                {
                    card.ChangeCardState(CardState.Selectable);
                }

                break;
        }

        ReorderCards();
    }

    public void GetAllCardsFromHand(Component sender, object data)
    {
        SelectEffect askerEffect = (SelectEffect)data;
        StartCoroutine(askerEffect.HandleResponse(this, new List<Card>(cardsInHand)));
    }

    public void AddCardToHand(Card card)
    {
        cardsInHand.Add(card);
        card.transform.localScale = Vector3.one * GameConstants.HandScale;
        //card.index = cardsInHand.IndexOf(card);
        ReorderCards();
    }
    public void OnHandCardDroppedNowhere(Component sender, object data)
    {
        Card card = data as Card;
        InsertCardToHandByIndex(card, card.index);
    }

    public void InsertCardToHandByIndex(Card card, int index)
    {
        int cardIndex = index;
        card.transform.localScale = Vector3.one * GameConstants.HandScale;


        if (index > cardsInHand.Count)
        {
            cardIndex = cardsInHand.Count;
        }

        if (!cardsInHand.Contains(card))
        {
            cardsInHand.Insert(cardIndex, card);
        }

        ReorderCards();
    }

    public void SwitchCards(Card incomingCard, Card existingCard)
    {
        int incomingIndex = cardsInHand.IndexOf(incomingCard);
        int existingIndex = cardsInHand.IndexOf(existingCard);

        if (incomingIndex != -1 && existingIndex != -1)
        {
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


    public void OnCardDroppedOnCard(Component sender, object data)
    {
        Card card = data as Card;
        RemoveCardFromHand(card);
    }
    public void RemoveCardFromHand(Card card)
    {
        cardsInHand.Remove(card);
        card.transform.localScale = Vector3.one * GameConstants.MapScale;
        ReorderCards();
    }

    [Button]
    public void ReorderCards()
    {
        int numOfCards = cardsInHand.Count;

        if (numOfCards == 1)
        {
            // Directly set the position for the single card
            Card card = cardsInHand[0];
            card.transform.SetPositionAndRotation(transform.position, transform.rotation);

            // Additional settings for the single card
            card.visualHandler.SetSortingOrder(0);
            card.interactionHandler.SetNewDefaultLocation(transform.position, card.transform.localScale, transform.rotation.eulerAngles);
        }
        else
        {
            // Calculate dynamic values based on the number of cards
            float dynamicSpacing = baseSpacing;
            float dynamicYOffsetFactor = Mathf.Lerp(yOffsetFactorMinMax.x, yOffsetFactorMinMax.y, (numOfCards - 1) / (float)maxCardAmount);
            float dynamicRotationAngle = Mathf.Lerp(0, baseRotationAngle, (numOfCards - 1) / (float)maxCardAmount);

            for (int i = 0; i < numOfCards; i++)
            {

                Card currentCard = cardsInHand[i];
                // Calculate the position offset for this card
                float xPos = (i - (numOfCards - 1) / 2f) * dynamicSpacing;
                float yPos = CalculateYPosition(i, numOfCards, dynamicYOffsetFactor); // Assuming this is a method you've defined

                // Apply the hand's position and rotation to this offset
                Vector3 cardPosition = transform.position + transform.right * xPos + transform.up * yPos;

                // Calculate and apply rotation
                float rotationFactor = (float)i / (numOfCards - 1);
                float angle = Mathf.Lerp(dynamicRotationAngle, -dynamicRotationAngle, rotationFactor);
                Quaternion cardRotation = transform.rotation * Quaternion.Euler(0, 0, angle);

                // Set the card's position and rotation
                if (cardsInHand[i] != dragManager.draggedCard)
                {
                    currentCard.transform.position = cardPosition;
                    currentCard.transform.rotation = cardRotation;
                    currentCard.visualHandler.SetSortingOrder(i);
                }

                currentCard.index = i;
                currentCard.interactionHandler.SetNewDefaultLocation(cardPosition, currentCard.transform.localScale, cardRotation.eulerAngles);
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
public enum HandState
{
    Idle,
    Select
}
