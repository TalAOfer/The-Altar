using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class BattleManager : MonoBehaviour
{
    private PlayerManager playerManager;
    private BattleRoom roomManager;

    private Card enemyCard;
    private Card playerCard;

    [FoldoutGroup("Dependencies")]
    [SerializeField] private RoomData roomData;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private AllEvents events;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private BattleManagerData movementData;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private BattleInteractionHandler interactionHandler;

    public void Initialize(FloorManager floorManager)
    {
        playerManager = FindObjectOfType<PlayerManager>();
        roomManager = GetComponentInParent<BattleRoom>();

        roomData.PlayerManager = playerManager;
        roomData.EnemyManager = roomManager;
        roomData.BattleRoomState = BattleRoomState.Setup;
        roomData.floorManager = floorManager;
    }

    #region Event & button handlers


    public void OnAttack(Component sender, object data)
    {
        playerCard = sender as Card;
        enemyCard = data as Card;
        roomData.BattlingPlayerCard = playerCard;
        roomData.BattlingEnemyCard = enemyCard;
        interactionHandler.state = BattleInteractionState.Battle;

        StartCoroutine(BattleRoutine());
    }

    #endregion

    #region Battle
    private IEnumerator BattleRoutine()
    {
        events.SetGameState.Raise(this, GameState.Battle);

        yield return StartCoroutine(StartOfBattleRoutine());

        //events.AddLogEntry.Raise(this, "Support");
        yield return StartCoroutine(SupportEffectsRoutine());

        //events.AddLogEntry.Raise(this, "Before Attacking");
        yield return StartCoroutine(BeforeAttackingRoutine());

        //events.AddLogEntry.Raise(this, "Calculating Battle Points");
        yield return StartCoroutine(CalculateBattlePointsRoutine());

        playerCard.ToggleDamageVisual(true);
        enemyCard.ToggleDamageVisual(true);

        //events.AddLogEntry.Raise(this, "Battle Animation");
        yield return StartCoroutine(AttackAnimationRoutine());

        //events.AddLogEntry.Raise(this, "Global Death");
        yield return StartCoroutine(GlobalDeathRoutine());

        yield return StartCoroutine(SurviveRoutine());

        //events.AddLogEntry.Raise(this, "Aftermath Shapeshifts");
        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        //If dead, disable object and raise onglobaldeath
        if (enemyCard.IsDead)
        {
            enemyCard.gameObject.SetActive(false);
            ///removing happens in post battle- check why
        }

        if (playerCard.IsDead)
        {
            playerCard.gameObject.SetActive(false);
            playerManager.activeCards.Remove(playerCard);
            playerManager.hand.RemoveCardFromHand(playerCard);
        } 
        
        else
        {
            StartCoroutine(playerCard.interactionHandler.TransformVisualUniformlyToPlaceholder(0.25f));
            playerCard.visualHandler.SetSortingOrder(playerCard.index);
        }
        //events.AddLogEntry.Raise(this, "Back To Map");
        yield return StartCoroutine(ReturnPlayerCardToHandRoutine());


        yield return StartCoroutine(PostBattleRoutine());
    }

    #region Battle Routines
    private IEnumerator StartOfBattleRoutine()
    {
        //events.AddLogEntry.Raise(this, "Start Of Battle");
        //int amountOfOccurances = 1;

        Coroutine ApplyPlayerCardStartOfBattleEffects = StartCoroutine(playerCard.effects.ApplyStartOfBattleEffects(enemyCard));
        Coroutine ApplyEnemyCardStartOfBattleEffects = StartCoroutine(enemyCard.effects.ApplyStartOfBattleEffects(playerCard));

        yield return ApplyPlayerCardStartOfBattleEffects;
        yield return ApplyEnemyCardStartOfBattleEffects;

        bool battleStartEnded = false;

        while (!battleStartEnded)
        {
            //if (amountOfOccurances > 1)
            //{
            //    string eventName = "Start Of Battle " + amountOfOccurances.ToString(); 
            //    events.AddLogEntry.Raise(this, eventName);
            //}
            //amountOfOccurances++;

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
        List<Card> allCards = new(roomManager.activeEnemies);
        allCards.AddRange(playerManager.activeCards);
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
        foreach (Card card in playerManager.activeCards)
        {
            if (card.effects.SupportEffects.Count > 0)
            {
                yield return StartCoroutine(card.effects.ApplySupportEffects(playerCard));
            }
        }

        foreach (Card card in roomManager.activeEnemies)
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

    private IEnumerator AttackAnimationRoutine()
    {
        Vector3 originalPos = playerCard.visualHandler.transform.position;
        Vector3 originalScale = playerCard.visualHandler.transform.localScale;

        Vector3 targetPos = originalPos;
        targetPos.y -= movementData.readyingDistance;
        Coroutine playerCardReadying = StartCoroutine(playerCard.interactionHandler.TransformCardUniformly(playerCard.visualHandler.transform, targetPos, Vector3.one, null, movementData.readyingDuration, null));

        yield return playerCardReadying;

        Vector2 enemyCardClosestCollPos = enemyCard.interactionHandler.GetClosestCollPosToOtherCard(playerCard.visualHandler.transform.position);
        Coroutine playerCardHeadbutt = StartCoroutine(playerCard.interactionHandler.TransformCardUniformly(playerCard.visualHandler.transform, enemyCardClosestCollPos, null, null, movementData.headbuttDuration, null));

        yield return playerCardHeadbutt;

        playerCard.ToggleDamageVisual(false);
        enemyCard.ToggleDamageVisual(false);
        ApplyDamage();
        events.ShakeScreen.Raise(this, CameraShakeTypes.Classic);

        yield return StartCoroutine(DeathRoutine());

        //Coroutine enemyCardBackoff = StartCoroutine(enemyCard.interactionHandler.MoveCardToPositionOverTime(topBattleTransform.position, movementData.backOffDuration));
        Coroutine playerCardBackoff = StartCoroutine(playerCard.interactionHandler.TransformCardUniformly(playerCard.visualHandler.transform, originalPos, originalScale, null, movementData.backOffDuration, null));

        //yield return enemyCardBackoff;
        yield return playerCardBackoff;
    }

    private IEnumerator DeathRoutine()
    {
        Coroutine ApplyPlayerCardOnDeathEffects = null;
        Coroutine ApplyEnemyCardOnDeathEffects = null;

        if (playerCard.IsDead) ApplyPlayerCardOnDeathEffects = StartCoroutine(playerCard.effects.ApplyOnDeathEffects(enemyCard));
        if (enemyCard.IsDead) ApplyEnemyCardOnDeathEffects = StartCoroutine(enemyCard.effects.ApplyOnDeathEffects(playerCard));

        if (ApplyPlayerCardOnDeathEffects != null) yield return ApplyPlayerCardOnDeathEffects;
        if (ApplyEnemyCardOnDeathEffects != null) yield return ApplyEnemyCardOnDeathEffects;

        Coroutine ApplyPlayerDeathShapeshift = null;
        Coroutine ApplyEnemyDeathShapeshift = null;

        if (playerCard.IsDead) ApplyPlayerDeathShapeshift = StartCoroutine(playerCard.Shapeshift());
        if (enemyCard.IsDead) ApplyEnemyDeathShapeshift = StartCoroutine(enemyCard.Shapeshift());

        if (ApplyPlayerCardOnDeathEffects != null) yield return ApplyPlayerDeathShapeshift;
        if (ApplyEnemyCardOnDeathEffects != null) yield return ApplyEnemyDeathShapeshift;

        if (ApplyPlayerCardOnDeathEffects == null && ApplyEnemyCardOnDeathEffects == null)
        {
            yield return new WaitForSeconds(movementData.impactFreezeDuration);
        }
    }

    private IEnumerator SurviveRoutine()
    {
        Coroutine ApplyPlayerCardOnSurviveEffects = null;
        Coroutine ApplyEnemyCardOnSurviveEffects = null;

        if (!playerCard.IsDead) ApplyPlayerCardOnSurviveEffects = StartCoroutine(playerCard.effects.ApplyOnSurviveEffects(enemyCard));
        if (!enemyCard.IsDead) ApplyEnemyCardOnSurviveEffects = StartCoroutine(enemyCard.effects.ApplyOnSurviveEffects(playerCard));

        if (ApplyPlayerCardOnSurviveEffects != null) yield return ApplyPlayerCardOnSurviveEffects;
        if (ApplyEnemyCardOnSurviveEffects != null) yield return ApplyEnemyCardOnSurviveEffects;
    }


    private IEnumerator GlobalDeathRoutine()
    {
        if (enemyCard.IsDead || playerCard.IsDead)
        {
            foreach (Card card in playerManager.activeCards)
            {
                if (!card.IsDead && card.effects.SupportEffects.Count > 0)
                {
                    yield return StartCoroutine(card.effects.ApplyOnGlobalDeathEffects());
                }
            }

            foreach (Card card in roomManager.activeEnemies)
            {
                if (!card.IsDead && card.effects.SupportEffects.Count > 0)
                {
                    yield return StartCoroutine(card.effects.ApplyOnGlobalDeathEffects());
                }
            }
        }
    }

    #endregion

    #endregion

    #region Back To Map / Post Battle

    private IEnumerator PostBattleRoutine()
    {
        bool didEnemyDie = enemyCard.IsDead;
        int deathIndex = enemyCard.index;

        if (didEnemyDie)
        {
            //events.AddLogEntry.Raise(this, "Marking Death");
            //enemyManager.MarkAndDestroyDeadEnemy(enemyCard);
            //Wait for player to draw
            //playerDeck.DrawPlayerCard();
            roomManager.RemoveEnemyFromManager(enemyCard);
        }

        //events.AddLogEntry.Raise(this, "On Obtain");
        yield return StartCoroutine(playerCard.effects.ApplyOnObtainEffects());

        //events.AddLogEntry.Raise(this, "After On Obtain Shapeshift");
        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        //events.AddLogEntry.Raise(this, "On Action");
        yield return StartCoroutine(OnActionEffectsRoutine());

        //events.AddLogEntry.Raise(this, "After On Action Shapeshift");
        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        //events.AddLogEntry.Raise(this, "End Of Turn");
        yield return StartCoroutine(EndOfTurnEffectsRoutine());

        //events.AddLogEntry.Raise(this, "After End Of Turn Shapeshift");
        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        roomData.BattlingPlayerCard = null;
        roomData.BattlingEnemyCard = null;

        events.AddLogEntry.Raise(this, "New Turn Started");
        events.SetGameState.Raise(this, GameState.Idle);
        interactionHandler.SetState(BattleInteractionState.Idle);
    }



    private IEnumerator OnActionEffectsRoutine()
    {
        //Debug.Log("Starting player on action effects application");
        foreach (Card card in playerManager.activeCards)
        {
            if (card.effects.OnActionTakenEffects.Count > 0)
            {
                //Debug.Log("Applying " + card.name + "'s effects");
                yield return StartCoroutine(card.effects.ApplyOnActionTakenEffects());
            }
        }

        //Debug.Log("Starting enemy on action effects application");
        foreach (Card card in roomManager.activeEnemies)
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
        foreach (Card card in playerManager.activeCards)
        {
            if (card.effects.EndOfTurnEffects.Count > 0)
            {
                //Debug.Log("Applying " + card.name + "'s effects");
                yield return StartCoroutine(card.effects.ApplyEndOfTurnEffects());
            }
        }

        foreach (Card card in roomManager.activeEnemies)
        {
            if (card.effects.EndOfTurnEffects.Count > 0)
            {
                //Debug.Log("Applying " + card.name + "'s effects");
                yield return StartCoroutine(card.effects.ApplyEndOfTurnEffects());
            }
        }
    }

    private IEnumerator ReturnPlayerCardToHandRoutine()
    {
        if (playerCard != null && !playerCard.IsDead) yield break;

        playerManager.hand.RemoveCardFromHand(playerCard);
    }

    #endregion

    #region Helpers

    private void ApplyDamage()
    {
        enemyCard.TakeDamage(this);
        playerCard.TakeDamage(this);
    }


    #endregion
}

