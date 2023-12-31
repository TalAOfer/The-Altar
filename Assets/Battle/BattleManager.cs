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

    private Card attackedCard;
    private Card attackingCard;

    public Button battleButton;
    public Button backButton;

    [SerializeField] private AllEvents events;
    [SerializeField] private BattleCardMovementData movementData;

    public void OnCardDroppedOnCard(Component sender, object data)
    {
        attackingCard = sender as Card;
        attackedCard = data as Card;

        StartCoroutine(BattleFormationRoutine());
    }

    public IEnumerator BattleFormationRoutine()
    {
        StartCoroutine(attackedCard.ChangeCardState(CardState.Battle));
        StartCoroutine(attackingCard.ChangeCardState(CardState.Battle));

        curtainColl.enabled = true;
        StartColorLerp(curtainSr, 0.5f, 0.7f);

        Coroutine moveAttackedCardToTop = StartCoroutine(attackedCard.interactionHandler.TransformCardUniformly
            (topBattleTransform.position, Vector3.one * movementData.battleCardScale, Vector3.zero, movementData.toFormationDuration));

        Coroutine moveAttackingcardToBottom = StartCoroutine(attackingCard.interactionHandler.TransformCardUniformly
            (bottomBattleTransform.position, Vector3.one * movementData.battleCardScale, Vector3.zero, movementData.toFormationDuration));

        yield return moveAttackedCardToTop;
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
        Coroutine ApplyAttackingCardBeforeBattleEffects = StartCoroutine(attackingCard.effects.ApplyBeforeBattleEffects(attackedCard));
        Coroutine ApplyAttackedCardBeforeBattleEffects = StartCoroutine(attackedCard.effects.ApplyBeforeBattleEffects(attackingCard));

        yield return ApplyAttackingCardBeforeBattleEffects;
        yield return ApplyAttackedCardBeforeBattleEffects;

        Coroutine attackedCardReadying = StartCoroutine(attackedCard.interactionHandler.MoveCardByAmountOverTime(movementData.readyingDistance, movementData.readyingDuration));
        Coroutine attackingCardReadying = StartCoroutine(attackingCard.interactionHandler.MoveCardByAmountOverTime(-movementData.readyingDistance, movementData.readyingDuration));

        yield return attackedCardReadying;
        yield return attackingCardReadying;

        Coroutine attackedCardHeadbutt = StartCoroutine(attackedCard.interactionHandler.MoveCardByAmountOverTime(-movementData.headButtDistance, movementData.headbuttDuration));
        Coroutine attackingCardHeadbutt = StartCoroutine(attackingCard.interactionHandler.MoveCardByAmountOverTime(movementData.headButtDistance, movementData.headbuttDuration));

        yield return attackedCardHeadbutt;
        yield return attackingCardHeadbutt;

        Coroutine calcAttackedCardAttackPoints = StartCoroutine(attackedCard.CalcAttackPoints());
        Coroutine calcAttackingCardAtackPoints = StartCoroutine(attackingCard.CalcAttackPoints());

        yield return calcAttackedCardAttackPoints;
        yield return calcAttackingCardAtackPoints;

        Coroutine calcAttackingCardHurtPoints = StartCoroutine(attackingCard.CalcHurtPoints(attackedCard.attackPoints));
        Coroutine calcAttackedCardHurtPoints = StartCoroutine(attackedCard.CalcHurtPoints(attackingCard.attackPoints));

        yield return calcAttackingCardHurtPoints;
        yield return calcAttackedCardHurtPoints;

        ApplyDamage();

        Coroutine attackedCardBackoff = StartCoroutine(attackedCard.interactionHandler.MoveCardToPositionOverTime(topBattleTransform.position, movementData.backOffDuration));
        Coroutine attackingCardBackoff = StartCoroutine(attackingCard.interactionHandler.MoveCardToPositionOverTime(bottomBattleTransform.position, movementData.backOffDuration));

        yield return attackedCardBackoff;
        yield return attackingCardBackoff;

        yield return new WaitForSeconds(movementData.endBattleDelay);

        Coroutine ApplyAttackingCardOnSurviveEffects = null;
        Coroutine ApplyAttackedCardOnSurviveEffects = null;
            
        if (!attackingCard.IsDead) ApplyAttackingCardOnSurviveEffects = StartCoroutine(attackingCard.effects.ApplyOnSurviveEffects(attackedCard));
        if (!attackedCard.IsDead) ApplyAttackedCardOnSurviveEffects = StartCoroutine(attackedCard.effects.ApplyOnSurviveEffects(attackingCard));
        
        if (ApplyAttackingCardOnSurviveEffects != null) yield return ApplyAttackingCardOnSurviveEffects;
        if (ApplyAttackedCardOnSurviveEffects != null) yield return ApplyAttackedCardOnSurviveEffects;

        // Final logic after both cards have moved
        Coroutine attackedCardShapeshift = StartCoroutine(attackedCard.HandleShapeshift());
        Coroutine attackingCardShapeshift = StartCoroutine(attackingCard.HandleShapeshift());

        yield return attackedCardShapeshift;
        yield return attackingCardShapeshift;

        if (attackedCard.IsDead) attackedCard.Die();
        if (attackingCard.IsDead) attackingCard.Die();

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

        Coroutine moveAttackingCardBackToHand = null;
        Coroutine moveAttackedCardBackToMap = null;


        if (attackingCard != null && !attackingCard.IsDead)
        {
            StartCoroutine(attackingCard.ChangeCardState(CardState.Default));

            handManager.InsertCardToHandByIndex(attackingCard, attackingCard.index);
            //moveAttackingCardBackToHand = StartCoroutine(attackingCard.interactionHandler.TransformCardUniformly
            //(attackingCard.interactionHandler.defaultPos, attackingCard.interactionHandler.defaultScale, attackingCard.interactionHandler.defaultRotation, movementData.toFormationDuration));
        }

        if (attackedCard != null && !attackedCard.IsDead)
        {
            StartCoroutine(attackedCard.ChangeCardState(CardState.Default));
            moveAttackedCardBackToMap = StartCoroutine(attackedCard.interactionHandler.TransformCardUniformly
            (attackedCard.interactionHandler.defaultPos, attackedCard.interactionHandler.defaultScale, attackedCard.interactionHandler.defaultRotation, movementData.toFormationDuration));
        }

        if (moveAttackingCardBackToHand != null) yield return moveAttackingCardBackToHand;
        if (moveAttackedCardBackToMap != null) yield return moveAttackedCardBackToMap;

        if (attackedCard.IsDead)
        {
            events.OnMapCardDied.Raise(this, attackedCard.index);
        }
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
