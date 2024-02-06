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
    [SerializeField] private CardData cardData;
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

    public void OnAttack(Component sender, object data)
    {
        playerCard = sender as Card;
        enemyCard = data as Card;
        roomData.BattlingPlayerCard = playerCard;
        roomData.BattlingEnemyCard = enemyCard;
        interactionHandler.state = BattleInteractionState.Battle;

        StartCoroutine(BattleRoutine());
    }

    private IEnumerator BattleRoutine()
    {
        events.SetGameState.Raise(this, GameState.Battle);

        playerCard.movement.SnapCardToVisual();

        yield return StartCoroutine(StartOfBattleRoutine());

        //events.AddLogEntry.Raise(this, "Support");
        yield return StartCoroutine(SupportEffectsRoutine());

        //events.AddLogEntry.Raise(this, "Before Attacking");
        yield return StartCoroutine(BeforeAttackingRoutine());

        //events.AddLogEntry.Raise(this, "Calculating Battle Points");
        yield return StartCoroutine(CalculateBattlePointsRoutine());

        playerCard.ToggleDamageVisual(true);
        enemyCard.ToggleDamageVisual(true);

        yield return StartCoroutine(AnimateReadyAndHeadbutt());

        playerCard.ToggleDamageVisual(false);
        enemyCard.ToggleDamageVisual(false);
        ApplyDamage();
        events.ShakeScreen.Raise(this, CameraShakeTypes.Classic);

        yield return StartCoroutine(DeathRoutine());

        //events.AddLogEntry.Raise(this, "Global Death");
        yield return StartCoroutine(GlobalDeathRoutine());


        yield return StartCoroutine(SurviveRoutine());

        //events.AddLogEntry.Raise(this, "Aftermath Shapeshifts");
        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        if (enemyCard.IsDead) enemyCard.gameObject.SetActive(false);
        if (playerCard.IsDead) playerCard.gameObject.SetActive(false);

        if (!playerCard.IsDead)
        {
            yield return StartCoroutine(AnimateBackoff());
        }

        bool didEnemyDie = enemyCard.IsDead;
        int deathIndex = enemyCard.index;

        if (didEnemyDie)
        {
            roomManager.RemoveEnemyFromManager(enemyCard);
        }

        //events.AddLogEntry.Raise(this, "On Obtain");
        //yield return StartCoroutine(playerCard.effects.ApplyOnObtainEffects());

        //events.AddLogEntry.Raise(this, "After On Obtain Shapeshift");
        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        //events.AddLogEntry.Raise(this, "On Action");
        yield return StartCoroutine(BloodthirstEffectsRoutine());

        //events.AddLogEntry.Raise(this, "After On Action Shapeshift");
        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        //events.AddLogEntry.Raise(this, "End Of Turn");
        yield return StartCoroutine(MeditateEffectsRoutine());

        //events.AddLogEntry.Raise(this, "After End Of Turn Shapeshift");
        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        if (!playerCard.IsDead)
        {
            yield return StartCoroutine(playerCard.movement.TransformCardUniformlyToPlaceholder(cardData.backOffSpeed, cardData.backoffCurve));
        }


        roomData.BattlingPlayerCard = null;
        roomData.BattlingEnemyCard = null;

        events.AddLogEntry.Raise(this, "New Turn Started");
        if (roomManager.activeEnemies.Count == 0) 
        {
            roomManager.OpenDoor();
        } 
        
        else
        {
            if (playerManager.activeCards.Count == 0)
            {
                Debug.Log("you lost");
            }
        }

        events.SetGameState.Raise(this, GameState.Idle);
        interactionHandler.SetState(BattleInteractionState.Idle);
    }

    #region Effect Routines

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

    private IEnumerator StartOfBattleRoutine()
    {
        //events.AddLogEntry.Raise(this, "Start Of Battle");

        #region Loop Count Debugger
        //int amountOfOccurances = 1;
        #endregion

        Coroutine ApplyPlayerCardStartOfBattleEffects = StartCoroutine(playerCard.effects.ApplyStartOfBattleEffects(enemyCard));
        Coroutine ApplyEnemyCardStartOfBattleEffects = StartCoroutine(enemyCard.effects.ApplyStartOfBattleEffects(playerCard));

        yield return ApplyPlayerCardStartOfBattleEffects;
        yield return ApplyEnemyCardStartOfBattleEffects;

        bool battleStartEnded = false;

        while (!battleStartEnded)
        {

            #region Loop Count Debugger
            //if (amountOfOccurances > 1)
            //{
            //    string eventName = "Start Of Battle " + amountOfOccurances.ToString(); 
            //    events.AddLogEntry.Raise(this, eventName);
            //}
            //amountOfOccurances++;

            #endregion

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

        if (playerCard.IsDead)
        {
            ApplyPlayerDeathShapeshift = StartCoroutine(playerCard.Shapeshift());
            playerManager.activeCards.Remove(playerCard);
            playerManager.hand.RemoveCardFromHand(playerCard);
        }

        if (enemyCard.IsDead) ApplyEnemyDeathShapeshift = StartCoroutine(enemyCard.Shapeshift());

        if (ApplyPlayerDeathShapeshift != null) yield return ApplyPlayerDeathShapeshift;
        if (ApplyEnemyDeathShapeshift != null) yield return ApplyEnemyDeathShapeshift;

        if (ApplyPlayerCardOnDeathEffects == null && ApplyEnemyCardOnDeathEffects == null)
        {
            yield return new WaitForSeconds(cardData.impactFreezeDuration);
        }
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

    private IEnumerator SurviveRoutine()
    {
        Coroutine ApplyPlayerCardOnSurviveEffects = null;
        Coroutine ApplyEnemyCardOnSurviveEffects = null;

        if (!playerCard.IsDead) ApplyPlayerCardOnSurviveEffects = StartCoroutine(playerCard.effects.ApplyOnSurviveEffects(enemyCard));
        if (!enemyCard.IsDead) ApplyEnemyCardOnSurviveEffects = StartCoroutine(enemyCard.effects.ApplyOnSurviveEffects(playerCard));

        if (ApplyPlayerCardOnSurviveEffects != null) yield return ApplyPlayerCardOnSurviveEffects;
        if (ApplyEnemyCardOnSurviveEffects != null) yield return ApplyEnemyCardOnSurviveEffects;
    }

    private IEnumerator BloodthirstEffectsRoutine()
    {
        //Debug.Log("Starting player on action effects application");
        foreach (Card card in playerManager.activeCards)
        {
            if (card.effects.BloodthirstEffects.Count > 0)
            {
                //Debug.Log("Applying " + card.name + "'s effects");
                yield return StartCoroutine(card.effects.ApplyBloodthirstEffects());
            }
        }

        //Debug.Log("Starting enemy on action effects application");
        foreach (Card card in roomManager.activeEnemies)
        {
            if (card.effects.BloodthirstEffects.Count > 0)
            {
                //Debug.Log("Applying " + card.name + "'s effects");
                yield return StartCoroutine(card.effects.ApplyBloodthirstEffects());
            }
        }
    }

    private IEnumerator MeditateEffectsRoutine()
    {
        //Debug.Log("Starting player end of turn effects application");
        foreach (Card card in playerManager.activeCards)
        {
            if (card.effects.MeditateEffects.Count > 0)
            {
                //Debug.Log("Applying " + card.name + "'s effects");
                yield return StartCoroutine(card.effects.ApplyMeditateEffects());
            }
        }

        foreach (Card card in roomManager.activeEnemies)
        {
            if (card.effects.MeditateEffects.Count > 0)
            {
                //Debug.Log("Applying " + card.name + "'s effects");
                yield return StartCoroutine(card.effects.ApplyMeditateEffects());
            }
        }
    }

    #endregion

    #region Animation Routines 

    private IEnumerator AnimateReadyAndHeadbutt()
    {
        Vector3 targetPos = playerCard.transform.position;
        targetPos.y -= cardData.readyingDistance;
        Coroutine playerCardReadying = StartCoroutine(playerCard.movement.TransformCardUniformly(playerCard.transform, targetPos, Vector3.one, null, cardData.readyingSpeed, cardData.readyingCurve));

        yield return playerCardReadying;

        StartCoroutine(RemoveCardFromHand());

        Vector2 enemyCardClosestCollPos = enemyCard.movement.GetClosestCollPosToOtherCard(playerCard.transform.position);
        Coroutine playerCardHeadbutt = StartCoroutine(playerCard.movement.TransformCardUniformly(playerCard.transform, enemyCardClosestCollPos, Vector3.one, null, cardData.headbuttSpeed, cardData.headbuttCurve));

        yield return playerCardHeadbutt;
    }

    private IEnumerator RemoveCardFromHand()
    {
        yield return new WaitForSeconds(0.1f);
        playerManager.hand.RemoveCardFromHand(playerCard);
    }

    private IEnumerator AnimateBackoff()
    {
        playerManager.hand.AddCardToHand(playerCard);
        playerCard.visualHandler.SetSortingOrder(playerCard.index);
        yield return StartCoroutine(playerCard.movement.TransformCardUniformlyToHoveredPlaceholder(cardData.backOffSpeed, cardData.backoffCurve));
        playerCard.visualHandler.SetSortingLayer(GameConstants.PLAYER_CARD_LAYER);
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

