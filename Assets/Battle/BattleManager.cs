using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public HandManager handManager;
    public SpriteRenderer curtainSr;
    public BoxCollider2D curtainColl;

    public Transform topBattleTransform;
    public Transform bottomBattleTransform;

    private Card enemyCard;
    private Card playerCard;

    public Button battleButton;
    public Button backButton;

    [SerializeField] private AllEvents events;
    [SerializeField] private BattleCardMovementData movementData;

    public void OnCardDroppedOnCard(Component sender, object data)
    {
        playerCard = sender as Card;
        enemyCard = data as Card;

        StartCoroutine(BattleFormationRoutine());
    }

    public IEnumerator BattleFormationRoutine()
    {
        StartCoroutine(enemyCard.ChangeCardState(CardState.Battle));
        StartCoroutine(playerCard.ChangeCardState(CardState.Battle));

        curtainColl.enabled = true;
        StartColorLerp(curtainSr, 0.5f, 0.7f);

        Coroutine moveEnemyCardToTop = StartCoroutine(enemyCard.interactionHandler.TransformCardUniformly
            (topBattleTransform.position, Vector3.one * movementData.battleCardScale, Vector3.zero, movementData.toFormationDuration));

        Coroutine moveAttackingcardToBottom = StartCoroutine(playerCard.interactionHandler.TransformCardUniformly
            (bottomBattleTransform.position, Vector3.one * movementData.battleCardScale, Vector3.zero, movementData.toFormationDuration));

        yield return moveEnemyCardToTop;
        yield return moveAttackingcardToBottom;

        battleButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);

    }

    public void DoBattle()
    {
        battleButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        StartCoroutine(BattleRoutine());
    }

    private IEnumerator BattleRoutine()
    {
        //events.OnPrebattleEnemy.Raise(this, enemyCard);

        foreach (Card card in handManager.cardsInHand)
        {
            if (card.effects.SupportEffects.Count > 0)
            {
                yield return StartCoroutine(card.effects.ApplySupportEffects(playerCard));
            }
        }

        yield return new WaitForSeconds(0.25f);

        Coroutine ApplyPlayerCardBeforeBattleEffects = StartCoroutine(playerCard.effects.ApplyBeforeBattleEffects(enemyCard));
        Coroutine ApplyEnemyCardBeforeBattleEffects = StartCoroutine(enemyCard.effects.ApplyBeforeBattleEffects(playerCard));

        yield return ApplyPlayerCardBeforeBattleEffects;
        yield return ApplyEnemyCardBeforeBattleEffects;

        Coroutine HandlePlayerPreBattleShapeshift = StartCoroutine(playerCard.HandleShapeshift(ShapeshiftType.Prebattle));
        Coroutine HandleEnemyPreBattleShapeshift = StartCoroutine(enemyCard.HandleShapeshift(ShapeshiftType.Prebattle));

        yield return HandleEnemyPreBattleShapeshift;
        yield return HandlePlayerPreBattleShapeshift;

        Coroutine enemyCardReadying = StartCoroutine(enemyCard.interactionHandler.MoveCardByAmountOverTime(movementData.readyingDistance, movementData.readyingDuration));
        Coroutine playerCardReadying = StartCoroutine(playerCard.interactionHandler.MoveCardByAmountOverTime(-movementData.readyingDistance, movementData.readyingDuration));

        yield return enemyCardReadying;
        yield return playerCardReadying;

        Coroutine enemyCardHeadbutt = StartCoroutine(enemyCard.interactionHandler.MoveCardByAmountOverTime(-movementData.headButtDistance, movementData.headbuttDuration));
        Coroutine playerCardHeadbutt = StartCoroutine(playerCard.interactionHandler.MoveCardByAmountOverTime(movementData.headButtDistance, movementData.headbuttDuration));

        yield return enemyCardHeadbutt;
        yield return playerCardHeadbutt;

        Coroutine calcEnemyCardAttackPoints = StartCoroutine(enemyCard.CalcAttackPoints(playerCard));
        Coroutine calcPlayerCardAtackPoints = StartCoroutine(playerCard.CalcAttackPoints(enemyCard));

        yield return calcEnemyCardAttackPoints;
        yield return calcPlayerCardAtackPoints;

        Coroutine calcPlayerCardHurtPoints = StartCoroutine(playerCard.CalcHurtPoints(enemyCard, enemyCard.attackPoints));
        Coroutine calcEnemyCardHurtPoints = StartCoroutine(enemyCard.CalcHurtPoints(playerCard, playerCard.attackPoints));

        yield return calcPlayerCardHurtPoints;
        yield return calcEnemyCardHurtPoints;

        ApplyDamage();

        Coroutine enemyCardBackoff = StartCoroutine(enemyCard.interactionHandler.MoveCardToPositionOverTime(topBattleTransform.position, movementData.backOffDuration));
        Coroutine playerCardBackoff = StartCoroutine(playerCard.interactionHandler.MoveCardToPositionOverTime(bottomBattleTransform.position, movementData.backOffDuration));

        yield return enemyCardBackoff;
        yield return playerCardBackoff;

        yield return new WaitForSeconds(movementData.endBattleDelay);

        Coroutine ApplyPlayerCardOnSurviveEffects = null;
        Coroutine ApplyEnemyCardOnSurviveEffects = null;

        if (!playerCard.IsDead) ApplyPlayerCardOnSurviveEffects = StartCoroutine(playerCard.effects.ApplyOnSurviveEffects(enemyCard));
        if (!enemyCard.IsDead) ApplyEnemyCardOnSurviveEffects = StartCoroutine(enemyCard.effects.ApplyOnSurviveEffects(playerCard));

        if (ApplyPlayerCardOnSurviveEffects != null) yield return ApplyPlayerCardOnSurviveEffects;
        if (ApplyEnemyCardOnSurviveEffects != null) yield return ApplyEnemyCardOnSurviveEffects;

        // Final logic after both cards have moved
        Coroutine HandleEnemyPostBattleShapeshift = StartCoroutine(enemyCard.HandleShapeshift(ShapeshiftType.Postbattle));
        Coroutine HandlePlayerPostBattleShapeshift = StartCoroutine(playerCard.HandleShapeshift(ShapeshiftType.Postbattle));

        yield return HandleEnemyPostBattleShapeshift;
        yield return HandlePlayerPostBattleShapeshift;

        if (enemyCard.IsDead) enemyCard.Die();
        if (playerCard.IsDead) playerCard.Die();

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

        Coroutine movePlayerCardBackToHand = null;
        Coroutine moveEnemyCardBackToMap = null;


        if (playerCard != null && !playerCard.IsDead)
        {
            StartCoroutine(playerCard.ChangeCardState(CardState.Default));

            handManager.InsertCardToHandByIndex(playerCard, playerCard.index);
            //movePlayerCardBackToHand = StartCoroutine(playerCard.interactionHandler.TransformCardUniformly
            //(playerCard.interactionHandler.defaultPos, playerCard.interactionHandler.defaultScale, playerCard.interactionHandler.defaultRotation, movementData.toFormationDuration));
        }

        if (enemyCard != null && !enemyCard.IsDead)
        {
            StartCoroutine(enemyCard.ChangeCardState(CardState.Default));
            moveEnemyCardBackToMap = StartCoroutine(enemyCard.interactionHandler.TransformCardUniformly
            (enemyCard.interactionHandler.defaultPos, enemyCard.interactionHandler.defaultScale, enemyCard.interactionHandler.defaultRotation, movementData.toFormationDuration));
        }

        if (movePlayerCardBackToHand != null) yield return movePlayerCardBackToHand;
        if (moveEnemyCardBackToMap != null) yield return moveEnemyCardBackToMap;

        if (enemyCard.IsDead)
        {
            events.OnMapCardDied.Raise(this, enemyCard.index);
        }
    }

    private void ApplyDamage()
    {
        enemyCard.TakeDamage();
        playerCard.TakeDamage();
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

