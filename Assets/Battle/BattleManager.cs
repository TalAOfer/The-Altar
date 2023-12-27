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
    public Button backButton;

    public float backwardSpeed = 5;
    public float moveDistanceBackward = 0.5f;
    public float forwardSpeed = 20f;
    public float moveDistanceForward = 1.5f;
    public float backToPlaceSpeed = 4f;
    public float toPlaceSpeed = 8f;

    public float battleCardScale = 1.25f;
    public float scaleLerpSpeed = 5f;

    public void OnCardDroppedOnCard(Component sender, object data)
    {
        attackedCard = sender as Card;
        attackingCard = data as Card;

        attackedCardOriginalPosition = attackedCard.transform.position;
        attackingCardOriginalPosition = attackingCard.GetComponent<CardInteractionHandler>().startPos;

        attackedCard.SetSortingLayer(GameConstants.BOTTOM_BATTLE_LAYER);
        attackingCard.SetSortingLayer(GameConstants.TOP_BATTLE_LAYER);

        StartCoroutine(MoveToPosition(attackedCard.transform, topBattleTransform.position, toPlaceSpeed));
        StartCoroutine(MoveToPosition(attackingCard.transform, bottomBattleTransform.position, toPlaceSpeed));
        
        StartCoroutine(LerpToScale(attackedCard.transform, Vector3.one * battleCardScale, scaleLerpSpeed));
        StartCoroutine(LerpToScale(attackingCard.transform, Vector3.one * battleCardScale, scaleLerpSpeed));

        battleButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);

        curtainColl.enabled = true;
        StartColorLerp(curtainSr, 0.5f, 0.7f);
    }

    public void DoBattle()
    {
        battleButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        StartCoroutine(BattleRoutine());
    }

    private IEnumerator BattleRoutine()
    {
        Coroutine ApplyAttackingCardBeforeBattleEffects = StartCoroutine(attackingCard.ApplyBeforeBattleEffects(attackedCard));
        Coroutine ApplyAttackedCardBeforeBattleEffects = StartCoroutine(attackedCard.ApplyBeforeBattleEffects(attackingCard));

        yield return ApplyAttackingCardBeforeBattleEffects;
        yield return ApplyAttackedCardBeforeBattleEffects;

        // Move both cards backward
        Coroutine moveAttackedCardBackward = StartCoroutine(MoveCard(attackedCard, topBattleTransform.position, moveDistanceBackward, backwardSpeed));
        Coroutine moveAttackingCardBackward = StartCoroutine(MoveCard(attackingCard, bottomBattleTransform.position, -moveDistanceBackward, backwardSpeed));

        // Wait for both backward movements to complete
        yield return moveAttackedCardBackward;
        yield return moveAttackingCardBackward;

        // Move both cards forward to simulate headbutt
        Coroutine moveAttackedCardForward = StartCoroutine(MoveCard(attackedCard, topBattleTransform.position, -moveDistanceForward, forwardSpeed));
        Coroutine moveAttackingCardForward = StartCoroutine(MoveCard(attackingCard, bottomBattleTransform.position, moveDistanceForward, forwardSpeed));

        // Wait for both headbutt movements to complete
        yield return moveAttackedCardForward;
        yield return moveAttackingCardForward;

        Coroutine calcAttackedCardAttackPoints = StartCoroutine(attackedCard.CalcAttackPoints());
        Coroutine calcAttackingCardAtackPoints = StartCoroutine(attackingCard.CalcAttackPoints());

        yield return calcAttackedCardAttackPoints;
        yield return calcAttackingCardAtackPoints;

        Coroutine calcAttackingCardHurtPoints = StartCoroutine(attackingCard.CalcHurtPoints(attackedCard.attackPoints));
        Coroutine calcAttackedCardHurtPoints = StartCoroutine(attackedCard.CalcHurtPoints(attackingCard.attackPoints));

        yield return calcAttackingCardHurtPoints;
        yield return calcAttackedCardHurtPoints;

        ApplyDamage();

        // Move both cards back to original position
        Coroutine moveAttackedCardBackToPlace = StartCoroutine(MoveToPosition(attackedCard.transform, topBattleTransform.position, backToPlaceSpeed));
        Coroutine moveAttackingCardBackToPlace = StartCoroutine(MoveToPosition(attackingCard.transform, bottomBattleTransform.position, backToPlaceSpeed));

        yield return moveAttackedCardBackToPlace;
        yield return moveAttackingCardBackToPlace;

        // Final logic after both cards have moved
        Coroutine attackedCardShapeshift = StartCoroutine(attackedCard.HandleShapeshift());
        Coroutine attackingCardShapeshift = StartCoroutine(attackingCard.HandleShapeshift());

        yield return attackedCardShapeshift;
        yield return attackingCardShapeshift;

        if (attackedCard.isDead) attackedCard.gameObject.SetActive(false);
        if (attackingCard.isDead) attackingCard.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        GoBackToMap();
    }

    public void GoBackToMap()
    {
        StartCoroutine(BackToMapRoutine());
    }

    public IEnumerator BackToMapRoutine()
    {
        backButton.gameObject.SetActive(false);
        battleButton.gameObject.SetActive(false);

        StartColorLerp(curtainSr, 0.5f, 0f);
        curtainColl.enabled = false;

        Coroutine moveBackToHand = null;
        Coroutine scaleBackToHandSize = null;
        Coroutine moveBackToMap = null;
        Coroutine scaleBackToMapSize = null;

        if (attackingCard != null && !attackingCard.isDead)
        {
            attackingCard.SetSortingLayer(GameConstants.HAND_LAYER);
            moveBackToHand = StartCoroutine(MoveToPosition(attackingCard.transform, attackingCardOriginalPosition, 7));
            scaleBackToHandSize = StartCoroutine(LerpToScale(attackingCard.transform, Vector3.one, 5));
        }

        if (attackedCard != null && !attackedCard.isDead)
        {
            attackedCard.SetSortingLayer(GameConstants.TOP_MAP_LAYER);
            moveBackToMap = StartCoroutine(MoveToPosition(attackedCard.transform, attackedCardOriginalPosition, 7));
            scaleBackToMapSize = StartCoroutine(LerpToScale(attackedCard.transform, Vector3.one, 5));
        }

        if (moveBackToHand != null) yield return moveBackToHand;
        if (scaleBackToHandSize != null) yield return scaleBackToHandSize;
        if (moveBackToMap != null) yield return moveBackToMap;
        if (scaleBackToMapSize != null) yield return scaleBackToMapSize;

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

    private IEnumerator LerpToScale(Transform targetTransform, Vector3 targetScale, float scaleSpeed)
    {
        while (Vector3.Distance(targetTransform.localScale, targetScale) > 0.01f)
        {
            targetTransform.localScale = Vector3.Lerp(targetTransform.localScale, targetScale, scaleSpeed * Time.deltaTime);
            yield return null;
        }

        // Ensure the final scale is set accurately
        targetTransform.localScale = targetScale;
    }

    private void ApplyDamage()
    {
        attackedCard.TakeDamage();
        attackingCard.TakeDamage();
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
