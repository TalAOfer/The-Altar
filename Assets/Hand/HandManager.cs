using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public List<Card> cardsInHands = new();
    public int maxCardAmount;
    public float totalWidth;
    public float baseSpacing = 1.5f;
    public float baseRotationAngle = 20f;
    public Vector2 yOffsetFactorMinMax = new(0.1f, 0.3f);
    public void AddCardToHand(Card card)
    {
        card.transform.SetParent(transform);
        card.transform.localScale = Vector3.one;
        cardsInHands.Add(card);
        ReorderCards();
    }


    [Button]
    public void ReorderCards()
    {
        int numOfCards = cardsInHands.Count;

        if (numOfCards == 1)
        {
            // Place the only card in the middle
            cardsInHands[0].transform.localPosition = new Vector3(0, 0, 0);
            cardsInHands[0].transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        else
        {
            float dynamicSpacing = CalculateSpacing(numOfCards);
            float dynamicYOffsetFactor = Mathf.Lerp(yOffsetFactorMinMax.x, yOffsetFactorMinMax.y, (numOfCards - 1) / (float) maxCardAmount);

            float dynamicRotationAngle = Mathf.Lerp(0, baseRotationAngle, (numOfCards - 1) / (float) maxCardAmount);

            for (int i = 0; i < numOfCards; i++)
            {
                float xPos = (i - (numOfCards - 1) / 2f) * dynamicSpacing;
                float yPos = CalculateYPosition(i, numOfCards, dynamicYOffsetFactor);
                cardsInHands[i].transform.localPosition = new Vector3(xPos, yPos, 0);


                // Calculate rotation angle for this card
                float rotationFactor = (float)i / (numOfCards - 1);
                float angle = Mathf.Lerp(dynamicRotationAngle, -dynamicRotationAngle, rotationFactor);
                cardsInHands[i].transform.localRotation = Quaternion.Euler(0, 0, angle);

                cardsInHands[i].visualHandler.SetSortingOrder(i);
            }
        }
    }

    private IEnumerator MoveToPosition(Transform targetTransform, Vector3 targetPosition, float moveSpeed)
    {
        while (Vector3.Distance(targetTransform.position, targetPosition) > 0.01f)
        {
            targetTransform.position = Vector3.Lerp(targetTransform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Ensure the final position is set accurately
        targetTransform.position = targetPosition;
    }

    float CalculateSpacing(int numCards)
    {
        // Adjust this formula as needed to suit the spacing style you want
        return baseSpacing / Mathf.Log(numCards, 2);
    }

    private float CalculateYPosition(int index, int totalCards, float yOffsetFactor)
    {
        // Normalize index position
        float normalizedIndex = (float)index / (totalCards - 1);

        // Apply a quadratic curve (parabola) for Y position
        return yOffsetFactor * (4 * normalizedIndex * (1 - normalizedIndex) - 1);
    }
}
