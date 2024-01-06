using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public HandManager handManager;
    public EnemyManager enemyManager;
    public PlayerDeck playerDeck;

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

    #region Event & button handlers
    public void OnCardDroppedOnCard(Component sender, object data)
    {
        playerCard = sender as Card;
        enemyCard = data as Card;

        StartCoroutine(BattleFormationRoutine());
    }
    public void DoBattle()
    {
        ToggleButtons(false);
        StartCoroutine(BattleRoutine());
    }

    public void OnPressedBack()
    {
        StartCoroutine(BackToMapRoutine());
    }

    #endregion

    #region Formation

    private IEnumerator BattleFormationRoutine()
    {
        StartCoroutine(enemyCard.ChangeCardState(CardState.Battle));
        StartCoroutine(playerCard.ChangeCardState(CardState.Battle));

        ToggleCurtain(true);

        yield return StartCoroutine(MoveCardsToFormation());

        ToggleButtons(true);

    }

    private IEnumerator MoveCardsToFormation()
    {
        Coroutine moveEnemyCardToTop = StartCoroutine(enemyCard.interactionHandler.TransformCardUniformly
            (topBattleTransform.position, Vector3.one * movementData.battleCardScale, Vector3.zero, movementData.toFormationDuration));

        Coroutine moveAttackingcardToBottom = StartCoroutine(playerCard.interactionHandler.TransformCardUniformly
            (bottomBattleTransform.position, Vector3.one * movementData.battleCardScale, Vector3.zero, movementData.toFormationDuration));

        yield return moveEnemyCardToTop;
        yield return moveAttackingcardToBottom;
    }

    #endregion

    #region Battle
    private IEnumerator BattleRoutine()
    {
        yield return StartCoroutine(StartOfBattleRoutine());

        yield return StartCoroutine(SupportEffectsRoutine());

        yield return StartCoroutine(BeforeAttackingRoutine());

        yield return StartCoroutine(CalculateBattlePointsRoutine());

        yield return StartCoroutine(ReadyingRoutine());

        yield return StartCoroutine(HeadbuttRoutine());

        yield return StartCoroutine(BackoffRoutine());

        yield return new WaitForSeconds(0.5f);

        ApplyDamage();

        yield return new WaitForSeconds(movementData.endBattleDelay);

        yield return StartCoroutine(DeathAndSurviveRoutine());

        yield return StartCoroutine(GlobalDeathRoutine());

        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        //If dead, disable object and raise onglobaldeath
        if (enemyCard.IsDead) enemyCard.gameObject.SetActive(false);
        if (playerCard.IsDead) playerCard.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(BackToMapRoutine());
        yield return StartCoroutine(PostBattleRoutine());
    }

    #region Battle Routines
    private IEnumerator StartOfBattleRoutine()
    {
        Coroutine ApplyPlayerCardStartOfBattleEffects = StartCoroutine(playerCard.effects.ApplyStartOfBattleEffects(enemyCard));
        Coroutine ApplyEnemyCardStartOfBattleEffects = StartCoroutine(enemyCard.effects.ApplyStartOfBattleEffects(playerCard));

        yield return ApplyPlayerCardStartOfBattleEffects;
        yield return ApplyEnemyCardStartOfBattleEffects;

        bool battleStartEnded = false;

        while (!battleStartEnded)
        {
            bool didPlayerShapeshift = playerCard.ShouldShapeshift();
            bool didEnemyShapeshift = enemyCard.ShouldShapeshift();
            battleStartEnded = (!didPlayerShapeshift && !didEnemyShapeshift);

            if (!battleStartEnded)
            {
                yield return (HandleAllShapeshiftsUntilStable());
            }

            Coroutine ReapplyPlayerCardStartOfBattleEffects = null;
            Coroutine ReapplyEnemyCardStartOfBattleEffects = null;

            if (didPlayerShapeshift) ReapplyPlayerCardStartOfBattleEffects = StartCoroutine(playerCard.effects.ApplyStartOfBattleEffects(enemyCard));
            if (didEnemyShapeshift) ReapplyEnemyCardStartOfBattleEffects = StartCoroutine(enemyCard.effects.ApplyStartOfBattleEffects(playerCard));

            if (ReapplyPlayerCardStartOfBattleEffects != null) yield return ReapplyPlayerCardStartOfBattleEffects;
            if (ReapplyEnemyCardStartOfBattleEffects != null) yield return ReapplyEnemyCardStartOfBattleEffects;

            

            yield return null;
        }
    }

    private IEnumerator HandleAllShapeshiftsUntilStable()
    {
        bool changesOccurred;
        List<Card> allCards = new (enemyManager.activeEnemies);
        allCards.AddRange(handManager.cardsInHand);
        allCards.Add(playerCard);

        do
        {
            changesOccurred = false;
            List<Coroutine> shapeshiftCoroutines = new List<Coroutine>();

            foreach (Card card in allCards)
            {
                if (card.ShouldShapeshift())
                {
                    changesOccurred = true;
                    Coroutine coroutine = StartCoroutine(card.HandleShapeshift());
                    shapeshiftCoroutines.Add(coroutine);
                }
            }

            // Wait for all shapeshift coroutines to finish
            foreach (Coroutine coroutine in shapeshiftCoroutines)
            {
                yield return coroutine;
            }

            // If changesOccurred is true, the loop will continue
        } while (changesOccurred);

        // All shapeshifts are done and no more changes, proceed with the next operation
        // ...
    }

    private IEnumerator BeforeAttackingRoutine()
    {
        Coroutine ApplyPlayerCardBeforeAttackingEffects = StartCoroutine(playerCard.effects.ApplyBeforeAttackingEffects(enemyCard));
        Coroutine ApplyEnemyCardBeforeAttackingEffects = StartCoroutine(enemyCard.effects.ApplyBeforeAttackingEffects(playerCard));

        yield return ApplyPlayerCardBeforeAttackingEffects;
        yield return ApplyEnemyCardBeforeAttackingEffects;
    }
    private IEnumerator SupportEffectsRoutine()
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
    }

    private IEnumerator CalculateBattlePointsRoutine()
    {
        Coroutine calcEnemyCardAttackPoints = StartCoroutine(enemyCard.CalcAttackPoints());
        Coroutine calcPlayerCardAtackPoints = StartCoroutine(playerCard.CalcAttackPoints());

        yield return calcEnemyCardAttackPoints;
        yield return calcPlayerCardAtackPoints;

        Coroutine calcPlayerCardHurtPoints = StartCoroutine(playerCard.CalcHurtPoints(enemyCard.attackPoints.value));
        Coroutine calcEnemyCardHurtPoints = StartCoroutine(enemyCard.CalcHurtPoints(playerCard.attackPoints.value));

        yield return calcPlayerCardHurtPoints;
        yield return calcEnemyCardHurtPoints;
    }

    private IEnumerator ReadyingRoutine()
    {
        Coroutine enemyCardReadying = StartCoroutine(enemyCard.interactionHandler.MoveCardByAmountOverTime(movementData.readyingDistance, movementData.readyingDuration));
        Coroutine playerCardReadying = StartCoroutine(playerCard.interactionHandler.MoveCardByAmountOverTime(-movementData.readyingDistance, movementData.readyingDuration));

        yield return enemyCardReadying;
        yield return playerCardReadying;
    }

    private IEnumerator HeadbuttRoutine()
    {
        Coroutine enemyCardHeadbutt = StartCoroutine(enemyCard.interactionHandler.MoveCardByAmountOverTime(-movementData.headButtDistance, movementData.headbuttDuration));
        Coroutine playerCardHeadbutt = StartCoroutine(playerCard.interactionHandler.MoveCardByAmountOverTime(movementData.headButtDistance, movementData.headbuttDuration));

        yield return enemyCardHeadbutt;
        yield return playerCardHeadbutt;
    }

    private IEnumerator BackoffRoutine()
    {
        Coroutine enemyCardBackoff = StartCoroutine(enemyCard.interactionHandler.MoveCardToPositionOverTime(topBattleTransform.position, movementData.backOffDuration));
        Coroutine playerCardBackoff = StartCoroutine(playerCard.interactionHandler.MoveCardToPositionOverTime(bottomBattleTransform.position, movementData.backOffDuration));

        yield return enemyCardBackoff;
        yield return playerCardBackoff;
    }

    private IEnumerator DeathAndSurviveRoutine()
    {
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
    }
    private IEnumerator GlobalDeathRoutine()
    {
        if (enemyCard.IsDead || playerCard.IsDead)
        {
            foreach (Card card in handManager.cardsInHand)
            {
                if (!card.IsDead && card.effects.SupportEffects.Count > 0)
                {
                    yield return StartCoroutine(card.effects.ApplyOnGlobalDeathEffects());
                }
            }

            foreach (Card card in enemyManager.activeEnemies)
            {
                if (!card.IsDead && card.effects.SupportEffects.Count > 0)
                {
                    yield return StartCoroutine(card.effects.ApplyOnGlobalDeathEffects());
                }
            }
        }
    }
    private IEnumerator AftermathShapeshiftRoutine()
    {
        Coroutine HandleEnemyPostBattleShapeshift = StartCoroutine(enemyCard.HandleShapeshift());
        Coroutine HandlePlayerPostBattleShapeshift = StartCoroutine(playerCard.HandleShapeshift());

        yield return HandleEnemyPostBattleShapeshift;
        yield return HandlePlayerPostBattleShapeshift;
    }

    #endregion

    #endregion

    #region Back To Map / Post Battle
    private IEnumerator BackToMapRoutine()
    {
        ToggleButtons(false);
        ToggleCurtain(false);

        Coroutine returnPlayerCard = StartCoroutine(ReturnPlayerCardToHand());
        Coroutine returnEnemyCard = StartCoroutine(ReturnEnemyCardToMap());

        yield return returnPlayerCard;
        yield return returnEnemyCard;
    }

    private IEnumerator PostBattleRoutine()
    {
        bool didEnemyDie = enemyCard.IsDead;
        int deathIndex = enemyCard.index;

        if (didEnemyDie)
        {
            enemyManager.MarkAndDestroyDeadEnemy(enemyCard);
            //Wait for player to draw
            yield return StartCoroutine(playerDeck.DrawAfterBattleRoutine());
        }

        yield return StartCoroutine(playerCard.effects.ApplyOnObtainEffects());

        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        if (didEnemyDie)
        {
            enemyManager.HightlightNeighboringSlots(deathIndex);
        } 
        
        else
        {
            yield return StartCoroutine(EndOfTurnRoutine());
        }
    }

    public void StartEndOfTurnRoutine()
    {
        StartCoroutine(EndOfTurnRoutine());
    }

    private IEnumerator EndOfTurnRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(OnActionEffectsRoutine());
        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        yield return StartCoroutine(EndOfTurnEffectsRoutine());
        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());
    }

    private IEnumerator OnActionEffectsRoutine()
    {
        //Debug.Log("Starting player on action effects application");
        foreach (Card card in handManager.cardsInHand)
        {
            if (card.effects.OnActionTakenEffects.Count > 0)
            {
                //Debug.Log("Applying " + card.name + "'s effects");
                yield return StartCoroutine(card.effects.ApplyOnActionTakenEffects());
            }
        }

        //Debug.Log("Starting enemy on action effects application");
        foreach (Card card in enemyManager.activeEnemies)
        {
            if (card.effects.OnActionTakenEffects.Count > 0)
            {
                //Debug.Log("Applying " + card.name + "'s effects");
                yield return StartCoroutine(card.effects.ApplyOnActionTakenEffects());
            }
        }
    }

    private IEnumerator EndOfTurnEffectsRoutine()
    {
        //Debug.Log("Starting player end of turn effects application");
        foreach (Card card in handManager.cardsInHand)
        {
            if (card.effects.EndOfTurnEffects.Count > 0)
            {
                //Debug.Log("Applying " + card.name + "'s effects");
                yield return StartCoroutine(card.effects.ApplyEndOfTurnEffects());
            }
        }

        foreach (Card card in enemyManager.activeEnemies)
        {
            if (card.effects.EndOfTurnEffects.Count > 0)
            {
                //Debug.Log("Applying " + card.name + "'s effects");
                yield return StartCoroutine(card.effects.ApplyEndOfTurnEffects());
            }
        }
    }

    private IEnumerator ReturnPlayerCardToHand()
    {
        if (playerCard == null || playerCard.IsDead) yield break;
        {
            yield return StartCoroutine(playerCard.ChangeCardState(CardState.Default));
            handManager.InsertCardToHandByIndex(playerCard, playerCard.index);
        }
    }

    private IEnumerator ReturnEnemyCardToMap()
    {
        if (enemyCard == null || enemyCard.IsDead) yield break;

        StartCoroutine(enemyCard.ChangeCardState(CardState.Default));
        yield return StartCoroutine(enemyCard.interactionHandler.TransformCardUniformly
        (enemyCard.interactionHandler.defaultPos, enemyCard.interactionHandler.defaultScale, enemyCard.interactionHandler.defaultRotation, movementData.toFormationDuration));
    }

    #endregion

    #region Helpers

    private void ApplyDamage()
    {
        enemyCard.TakeDamage();
        playerCard.TakeDamage();
    }

    private void ToggleCurtain(bool enable)
    {
        float curtainAlpha = enable ? 0.7f : 0f;
        StartColorLerp(curtainSr, 0.5f, curtainAlpha);
        curtainColl.enabled = enable;
    }

    private void ToggleButtons(bool enable)
    {
        backButton.gameObject.SetActive(enable);
        battleButton.gameObject.SetActive(enable);
    }

    private void StartColorLerp(SpriteRenderer spriteRenderer, float duration, float to)
    {
        StartCoroutine(LerpColorCoroutine(spriteRenderer, duration, to));
    }

    private IEnumerator LerpColorCoroutine(SpriteRenderer spriteRenderer, float duration, float to)
    {
        if (spriteRenderer != null)
        {
            float elapsed = 0;
            Color startColor = spriteRenderer.color;
            Color endColor = new(startColor.r, startColor.g, startColor.b, to);

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

    #endregion
}

