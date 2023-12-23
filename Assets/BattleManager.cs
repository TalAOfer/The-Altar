using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public SpriteRenderer curtainSr;
    public BoxCollider2D curtainColl;

    public Transform topBattleTransform;
    public Transform bottomBattleTransform;

    private Card attackedCard;
    private Vector3 attackedCardOriginalPosition;
    private Card attackingCard;
    private Vector3 attackingCardOriginalPosition;

    public Button battleButton;

    public float backwardSpeed = 5;
    public float moveDistanceBackward = 0.5f;
    public float forwardSpeed = 20f;
    public float moveDistanceForward = 1.5f;
    public float backToPlaceSpeed = 4f;
    public void OnCardDroppedOnCard(Component sender, object data)
    {
        attackedCard = sender as Card;
        attackingCard = data as Card;

        Debug.Log(attackingCard.name + "is in: " + attackingCard.transform.position);
        
        attackedCardOriginalPosition = attackedCard.transform.position;
        attackingCardOriginalPosition = attackingCard.GetComponent<CardInteractionHandler>().startPos;

        attackedCard.SetSortingLayer(GameConstants.BOTTOM_BATTLE_LAYER);
        attackingCard.SetSortingLayer(GameConstants.TOP_BATTLE_LAYER);
        
        attackedCard.transform.position = topBattleTransform.position;
        attackingCard.transform.position = bottomBattleTransform.position;
        attackingCard.transform.localScale = Vector3.one * 1.25f;
        attackedCard.transform.localScale = Vector3.one * 1.25f;

        battleButton.gameObject.SetActive(true);

        curtainColl.enabled = true;
        StartColorLerp(curtainSr, 0.5f, 0.7f);
    }

    public void DoBattle()
    {
        battleButton.gameObject.SetActive(false);
        StartCoroutine(BattleRoutine());
    }

    private IEnumerator BattleRoutine()
    {
        // Move both cards backward
        Coroutine moveAttackedCardBackward = StartCoroutine(MoveCard(attackedCard, topBattleTransform.position, moveDistanceBackward, backwardSpeed));
        Coroutine moveAttackingCardBackward = StartCoroutine(MoveCard(attackingCard, bottomBattleTransform.position, -moveDistanceBackward, backwardSpeed));

        // Wait for both backward movements to complete
        yield return moveAttackedCardBackward;
        yield return moveAttackingCardBackward;

        // Apply damage or any other mid-animation logic here
        ApplyDamage();

        // Move both cards forward to simulate headbutt
        Coroutine moveAttackedCardForward = StartCoroutine(MoveCard(attackedCard, topBattleTransform.position, -moveDistanceForward, forwardSpeed));
        Coroutine moveAttackingCardForward = StartCoroutine(MoveCard(attackingCard, bottomBattleTransform.position, moveDistanceForward, forwardSpeed));

        // Wait for both headbutt movements to complete
        yield return moveAttackedCardForward;
        yield return moveAttackingCardForward;

        // Move both cards back to original position
        Coroutine moveAttackedCardBackToPlace = StartCoroutine(MoveToPosition(attackedCard.transform, topBattleTransform.position, backToPlaceSpeed));
        Coroutine moveAttackingCardBackToPlace = StartCoroutine(MoveToPosition(attackingCard.transform, bottomBattleTransform.position, backToPlaceSpeed));

        yield return moveAttackedCardBackToPlace;
        yield return moveAttackingCardBackToPlace;

        // Final logic after both cards have moved
        Coroutine attackedCardShapeshift = StartCoroutine(attackedCard.Shapeshift());
        Coroutine attackingCardShapeshift = StartCoroutine(attackingCard.Shapeshift());

        yield return attackedCardShapeshift;
        yield return attackingCardShapeshift;

        yield return new WaitForSeconds(1f);


        StartColorLerp(curtainSr, 0.5f, 0f);
        curtainColl.enabled = false;

        if (attackingCard != null)
        {
            attackingCard.transform.localScale = Vector3.one;
            StartCoroutine(MoveToPosition(attackingCard.transform, attackingCardOriginalPosition, 7));
        }

        if (attackedCard != null)
        {
            attackedCard.transform.localScale = Vector3.one;
            StartCoroutine(MoveToPosition(attackedCard.transform, attackedCardOriginalPosition, 7));
        }
    }

    private IEnumerator MoveCard(Card card, Vector3 originalPosition, float moveDistance, float moveSpeed)
    {
        Vector3 targetPosition = originalPosition + new Vector3(0, moveDistance, 0);

        yield return StartCoroutine(MoveToPosition(card.transform, targetPosition, moveSpeed));
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
    private void ApplyDamage()
    {
        int attackedCardDamage = attackedCard.points;
        int attackingCardDamage = attackingCard.points;

        attackedCard.TakeDamage(attackingCardDamage);
        attackingCard.TakeDamage(attackedCardDamage);
    }

    public void StartColorLerp(SpriteRenderer spriteRenderer, float duration, float to)
    {
        StartCoroutine(LerpColorCoroutine(spriteRenderer, duration, to));
    }

    private IEnumerator LerpColorCoroutine(SpriteRenderer spriteRenderer, float duration, float to)
    {
        if (spriteRenderer != null)
        {
            float elapsed = 0;
            Color startColor = spriteRenderer.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, to);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / duration;
                spriteRenderer.color = Color.Lerp(startColor, endColor, normalizedTime);
                yield return null;
            }

            spriteRenderer.color = endColor; // Ensure the final color is set
        }
    }


}
