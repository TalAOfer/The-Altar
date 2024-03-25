using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private int maxCardAmount = 5;
    [SerializeField] private HandData formationData;
    [SerializeField] private EventRegistry events;
    [SerializeField] private CardData cardData;
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
                transform.position = startingPos;
                ////Handle pos
                //transform.position = startingPos;

                ////Handle formation
                //baseSpacing = formationData.baseSpacing;
                //baseRotationAngle = formationData.baseRotationAngle;
                //yOffsetFactorMinMax = formationData.yOffsetFactorMinMax;

                ////Handle SortingLayers
                //foreach (Card card in cardsInHand)
                //{
                //    card.ChangeCardState(CardState.Default);
                //}
                break;
            case HandState.Battle:
                Vector3 battlePos = startingPos;
                battlePos.y -= formationData.battleDrawbackY;
                transform.position = battlePos;
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
                    //card.ChangeCardState(CardState.Selectable);
                }

                break;
            
        }

        ReorderPlaceholders(true);
    }

    #region Transform and index handling
    [Button]
    public void ReorderPlaceholders(bool shouldMoveCards)
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
            card.movement.placeHolder = placeholder;

            // Set the new default location for the card
            card.index = 0;
            card.visualHandler.SetSortingOrder(0);

            // If needed, start the transformation coroutine for the card to move to its placeholder
            StartCoroutine(card.movement.TransformCardUniformlyToPlaceholder(cardData.ReorderSpeed, cardData.ReorderCurve));
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
                        currentCard.movement.placeHolder = cardPlaceholders[i];
                        currentCard.index = i;
                    }

                }

                else
                {
                    // Deactivate the extra placeholders
                    cardPlaceholders[i].gameObject.SetActive(false);
                }
            }

            if (shouldMoveCards) StartCoroutine(ResetCardsToPlaceholders());
        }
    }

    public IEnumerator ResetCardsToPlaceholders()
    {
        List<Coroutine> movementRoutines = new();

        foreach (var card in cardsInHand)
        {
            if (card == null || card.cardState is CardState.Battle) continue;
            card.visualHandler.SetSortingOrder(card.index);

            float speed = 0;
            Ease ease = Ease.Linear;

            switch (card.cardState)
            {
                case CardState.Default:
                    speed = cardData.ReorderSpeed;
                    ease = cardData.ReorderCurve;
                    break;
                case CardState.Draw:
                    speed = cardData.drawCardSpeed;
                    ease = cardData.drawCardCurve;
                    break;
            }

            Coroutine moveRoutine = StartCoroutine(card.movement.TransformCardUniformlyToPlaceholder(speed, ease));
            movementRoutines.Add(moveRoutine);
        }

        foreach (var routine in movementRoutines)
        {
            yield return routine;
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
        ReorderPlaceholders(true);
    }

    public void RemoveCardFromHand(Card card)
    {
        cardsInHand.Remove(card);
    }

    #endregion
}
public enum HandState
{
    Idle,
    Battle,
    Select
}
