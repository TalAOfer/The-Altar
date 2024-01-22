using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private int maxCardAmount = 5;
    [SerializeField] private HandData formationData;
    [SerializeField] private AllEvents events;
    public List<Card> cardsInHand = new();
    public List<Transform> cardPlaceholders = new();

    private float baseSpacing;
    private float baseRotationAngle;
    private Vector2 yOffsetFactorMinMax;

    private Vector3 startingPos;
    private HandState state;


    private void Awake()
    {
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

        ReorderPlaceholders();
    }

    #region Transform and index handling
    [Button]
    public void ReorderPlaceholders()
    {
        int numOfCards = cardsInHand.Count;

        if (numOfCards == 1)
        {
            // Activate only the first placeholder and deactivate the rest
            for (int i = 0; i < cardPlaceholders.Count; i++)
            {
                cardPlaceholders[i].gameObject.SetActive(i == 0);
            }

            // Handle the single card
            Card card = cardsInHand[0];

            // Set the placeholder for this card
            Transform placeholder = cardPlaceholders[0];
            placeholder.SetPositionAndRotation(transform.position, transform.rotation);
            card.interactionHandler.placeHolder = placeholder;

            // Set the new default location for the card
            card.interactionHandler.SetNewDefaultLocation(placeholder.position, card.transform.localScale, placeholder.eulerAngles);
            card.index = 0;
            card.visualHandler.SetSortingOrder(0);

            // If needed, start the transformation coroutine for the card to move to its placeholder
            StartCoroutine(card.interactionHandler.TransformCardUniformlyToPlaceholder(0.25f));
        }
        else
        {
            for (int i = 0; i < cardPlaceholders.Count; i++)
            {
                if (i < numOfCards)
                {
                    float dynamicSpacing = baseSpacing;
                    float dynamicYOffsetFactor = Mathf.Lerp(yOffsetFactorMinMax.x, yOffsetFactorMinMax.y, (numOfCards - 1) / (float)maxCardAmount);
                    float dynamicRotationAngle = Mathf.Lerp(0, baseRotationAngle, (numOfCards - 1) / (float)maxCardAmount);

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
                    cardPlaceholders[i].transform.SetPositionAndRotation(cardPosition, cardRotation);
                    cardPlaceholders[i].gameObject.SetActive(true);

                    if (currentCard != null)
                    {
                        currentCard.interactionHandler.placeHolder = cardPlaceholders[i];
                        currentCard.interactionHandler.SetNewDefaultLocation(cardPosition, currentCard.transform.localScale, cardRotation.eulerAngles);
                        currentCard.index = i;
                    }

                    //currentCard.transform.SetPositionAndRotation(cardPosition, cardRotation);
                    //currentCard.visualHandler.SetSortingOrder(i);

                }
                else
                {
                    // Deactivate the extra placeholders
                    cardPlaceholders[i].gameObject.SetActive(false);
                }
            }

            foreach (var card in cardsInHand)
            {
                if (card == null || card.cardState is CardState.Battle) continue;
                card.visualHandler.SetSortingOrder(card.index);
                StartCoroutine(card.interactionHandler.TransformCardUniformlyToPlaceholder(0.25f));
            }
        }
    }

    public void ResetCardToPlaceholders()
    {
        foreach (var card in cardsInHand)
        {
            if (card.cardState is CardState.Battle) continue;
            StartCoroutine(card.interactionHandler.TransformCardUniformlyToPlaceholder(0.25f));
        }
    }

    private float CalculateYPosition(int index, int totalCards, float yOffsetFactor)
    {
        // Normalize index position
        float normalizedIndex = (float)index / (totalCards - 1);

        // Apply a quadratic curve (parabola) for Y position
        return yOffsetFactor * (4 * normalizedIndex * (1 - normalizedIndex) - 1);
    }

    #endregion

    #region Add and remove from hand
    public void AddCardToHand(Card card)
    {
        cardsInHand.Add(card);
        card.transform.localScale = Vector3.one * GameConstants.HandScale;
        //card.index = cardsInHand.IndexOf(card);
        ReorderPlaceholders();
    }

    public void RemoveCardFromHand(Card card)
    {
        cardsInHand.Remove(card);
        ReorderPlaceholders();
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

        ReorderPlaceholders();
    }

    public void SwitchCards(Card incomingCard, Card existingCard)
    {
        int incomingIndex = cardsInHand.IndexOf(incomingCard);
        int existingIndex = cardsInHand.IndexOf(existingCard);

        if (incomingIndex != -1 && existingIndex != -1)
        {
            (cardsInHand[existingIndex], cardsInHand[incomingIndex]) = (cardsInHand[incomingIndex], cardsInHand[existingIndex]);
            ReorderPlaceholders();
        }

        else
        {
            // Handle the case where one or both cards are not in the list
            Debug.Log("One or both of the cards are not in the list.");
        }
    }

    #endregion

    #region Event Handlers

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

    public void OnHandCardDroppedNowhere(Component sender, object data)
    {
        Card card = data as Card;
        InsertCardToHandByIndex(card, card.index);
    }

    public void HandleRemoveCardFromHand(Component sender, object data)
    {
        Card card = data as Card;
        RemoveCardFromHand(card);
    }

    public void HandleInsertCard(Component sender, object data)
    {
        Card card = data as Card;
        InsertCardToHandByIndex(card, card.index);
    }

    #endregion
}
public enum HandState
{
    Idle,
    Select
}
