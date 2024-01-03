using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public HandManager handManager;
    public EnemyManager enemyManager;

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
        foreach (Card card in handManager.cardsInHand)
        {
            if (card.effects.SupportEffects.Count > 0)
            {
                yield return StartCoroutine(card.effects.ApplySupportEffects(playerCard));
            }
        }

        foreach (Card card in enemyManager.activeEnemies)
        {
            if (card == enemyCard) continue;
            if (card.effects.SupportEffects.Count > 0)
            {
                yield return StartCoroutine(card.effects.ApplySupportEffects(enemyCard));
            }
        }

        Coroutine ApplyPlayerCardBeforeBattleEffects = StartCoroutine(playerCard.effects.ApplyBeforeBattleEffects(enemyCard));
        Coroutine ApplyEnemyCardBeforeBattleEffects = StartCoroutine(enemyCard.effects.ApplyBeforeBattleEffects(playerCard));

        yield return ApplyPlayerCardBeforeBattleEffects;
        yield return ApplyEnemyCardBeforeBattleEffects;

        bool battleStartEnded = false;

        while(!battleStartEnded)
        {
            bool didPlayerShapeshift = playerCard.ShouldShapeshift();
            bool didEnemyShapeshift = enemyCard.ShouldShapeshift();

            Coroutine HandlePlayerPreBattleShapeshift = null;
            Coroutine HandleEnemyPreBattleShapeshift = null;

            if (didPlayerShapeshift) HandlePlayerPreBattleShapeshift = StartCoroutine(playerCard.HandleShapeshift(ShapeshiftType.Prebattle));
            if (didEnemyShapeshift) HandleEnemyPreBattleShapeshift = StartCoroutine(enemyCard.HandleShapeshift(ShapeshiftType.Prebattle));

            if (HandlePlayerPreBattleShapeshift != null) yield return HandlePlayerPreBattleShapeshift;
            if (HandleEnemyPreBattleShapeshift != null) yield return HandleEnemyPreBattleShapeshift;

            Coroutine ReapplyPlayerCardBeforeBattleEffects = null;
            Coroutine ReapplyEnemyCardBeforeBattleEffects = null;

            if (didPlayerShapeshift) ReapplyPlayerCardBeforeBattleEffects = StartCoroutine(playerCard.effects.ApplyBeforeBattleEffects(enemyCard));
            if (didEnemyShapeshift) ReapplyEnemyCardBeforeBattleEffects = StartCoroutine(enemyCard.effects.ApplyBeforeBattleEffects(playerCard));

            if (ReapplyPlayerCardBeforeBattleEffects != null) yield return ReapplyPlayerCardBeforeBattleEffects;
            if (ReapplyEnemyCardBeforeBattleEffects != null) yield return ReapplyEnemyCardBeforeBattleEffects;

            battleStartEnded = (!didPlayerShapeshift && !didEnemyShapeshift);
        }

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
        Coroutine ApplyPlayerCardOnDeathEffects = null;
        
        Coroutine ApplyEnemyCardOnSurviveEffects = null;
        Coroutine ApplyEnemyCardOnDeathEffects = null;

        if (!playerCard.IsDead) ApplyPlayerCardOnSurviveEffects = StartCoroutine(playerCard.effects.ApplyOnSurviveEffects(enemyCard));
        else ApplyPlayerCardOnDeathEffects = StartCoroutine(playerCard.effects.ApplyOnDeathEffects());

        if (!enemyCard.IsDead) ApplyEnemyCardOnSurviveEffects = StartCoroutine(enemyCard.effects.ApplyOnSurviveEffects(playerCard));
        else ApplyEnemyCardOnDeathEffects = StartCoroutine(enemyCard.effects.ApplyOnDeathEffects());

        if (ApplyPlayerCardOnSurviveEffects != null) yield return ApplyPlayerCardOnSurviveEffects;
        else if (ApplyPlayerCardOnDeathEffects != null) yield return ApplyPlayerCardOnDeathEffects;

        if (ApplyEnemyCardOnSurviveEffects != null) yield return ApplyEnemyCardOnSurviveEffects;
        else if (ApplyEnemyCardOnDeathEffects != null) yield return ApplyEnemyCardOnDeathEffects;

        Coroutine HandleEnemyPostBattleShapeshift = StartCoroutine(enemyCard.HandleShapeshift(ShapeshiftType.Postbattle));
        Coroutine HandlePlayerPostBattleShapeshift = StartCoroutine(playerCard.HandleShapeshift(ShapeshiftType.Postbattle));

        yield return HandleEnemyPostBattleShapeshift;
        yield return HandlePlayerPostBattleShapeshift;

        //If dead, disable object and raise onglobaldeath
        if (enemyCard.IsDead) enemyCard.Die();
        if (playerCard.IsDead) playerCard.Die();

        yield return new WaitForSeconds(1f);

        GoBackToMap(true);
    }

    public void OnPressedBack()
    {
        StartCoroutine(BackToMapRoutine(false));
    }

    public void GoBackToMap(bool didFight)
    {
        StartCoroutine(BackToMapRoutine(didFight));
    }

    public IEnumerator BackToMapRoutine(bool didFight)
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

        if (didFight)
        {
            //StartCoroutine(OnAction for player)
            //StartCoroutine(OnAction for enemy)
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

