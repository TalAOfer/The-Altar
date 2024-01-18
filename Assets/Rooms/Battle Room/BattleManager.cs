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
    [SerializeField] private RunData runData;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private RoomData roomData;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private AllEvents events;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private BattleManagerData movementData;

    [FoldoutGroup("Formation Transforms")]
    [SerializeField] private Transform topBattleTransform;
    [FoldoutGroup("Formation Transforms")]
    [SerializeField] private Transform bottomBattleTransform;

    [FoldoutGroup("Buttons")]
    [SerializeField] private CustomButton battleButton;
    [FoldoutGroup("Buttons")]
    [SerializeField] private CustomButton backButton;


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
    public void InitializeNewRoom(Component sender, object data)
    {
        roomManager = (BattleRoom)data;
    }

    public void OnCardDroppedOnCard(Component sender, object data)
    {
        playerCard = sender as Card;
        enemyCard = data as Card;
        roomData.BattlingPlayerCard = playerCard;
        roomData.BattlingEnemyCard = enemyCard;

        StartCoroutine(BattleFormationRoutine());
    }
    public void DoBattle()
    {
        ToggleButtons(false);
        StartCoroutine(BattleRoutine());
    }

    public void OnPressedBack()
    {
        StartCoroutine(PressedBackRoutine());
    }

    private IEnumerator PressedBackRoutine()
    {
        yield return StartCoroutine(BackToMapRoutine());
        events.SetGameState.Raise(this, GameState.Idle);
    }

    #endregion

    #region Formation

    private IEnumerator BattleFormationRoutine()
    {
        enemyCard.ChangeCardState(CardState.Battle);
        playerCard.ChangeCardState(CardState.Battle);

        events.ToggleCurtain.Raise(this, true);
        events.SetGameState.Raise(this, GameState.BattleFormation);

        yield return StartCoroutine(MoveCardsToFormation());

        ToggleButtons(true);

    }

    private IEnumerator MoveCardsToFormation()
    {
        enemyCard.transform.SetParent(topBattleTransform, true);
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
        yield return StartCoroutine(ReadyingRoutine());

        yield return StartCoroutine(HeadbuttRoutine());

        yield return StartCoroutine(BackoffRoutine());

        yield return new WaitForSeconds(0.15f);

        ApplyDamage();

        playerCard.ToggleDamageVisual(false);
        enemyCard.ToggleDamageVisual(false);

        yield return new WaitForSeconds(movementData.endBattleDelay);

        //events.AddLogEntry.Raise(this, "Death & Survive");
        yield return StartCoroutine(DeathAndSurviveRoutine());

        //events.AddLogEntry.Raise(this, "Global Death");
        yield return StartCoroutine(GlobalDeathRoutine());

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
        }

        yield return new WaitForSeconds(1f);

        //events.AddLogEntry.Raise(this, "Back To Map");
        yield return StartCoroutine(BackToMapRoutine());

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
        else ApplyPlayerCardOnDeathEffects = StartCoroutine(playerCard.effects.ApplyOnDeathEffects(enemyCard));

        if (!enemyCard.IsDead) ApplyEnemyCardOnSurviveEffects = StartCoroutine(enemyCard.effects.ApplyOnSurviveEffects(playerCard));
        else ApplyEnemyCardOnDeathEffects = StartCoroutine(enemyCard.effects.ApplyOnDeathEffects(playerCard));

        if (ApplyPlayerCardOnSurviveEffects != null) yield return ApplyPlayerCardOnSurviveEffects;
        else if (ApplyPlayerCardOnDeathEffects != null) yield return ApplyPlayerCardOnDeathEffects;

        if (ApplyEnemyCardOnSurviveEffects != null) yield return ApplyEnemyCardOnSurviveEffects;
        else if (ApplyEnemyCardOnDeathEffects != null) yield return ApplyEnemyCardOnDeathEffects;
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
        events.ToggleCurtain.Raise(this, false);

        roomData.BattlingPlayerCard = null;
        roomData.BattlingEnemyCard = null;

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
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(OnActionEffectsRoutine());

        //events.AddLogEntry.Raise(this, "After On Action Shapeshift");
        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        //events.AddLogEntry.Raise(this, "End Of Turn");
        yield return StartCoroutine(EndOfTurnEffectsRoutine());

        //events.AddLogEntry.Raise(this, "After End Of Turn Shapeshift");
        yield return StartCoroutine(HandleAllShapeshiftsUntilStable());

        events.AddLogEntry.Raise(this, "New Turn Started");
        events.SetGameState.Raise(this, GameState.Idle);
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

    private IEnumerator ReturnPlayerCardToHand()
    {
        if (playerCard == null || playerCard.IsDead) yield break;
        {
            playerCard.ChangeCardState(CardState.Default);
            playerManager.hand.InsertCardToHandByIndex(playerCard, playerCard.index);
        }
    }

    private IEnumerator ReturnEnemyCardToMap()
    {
        if (enemyCard == null || enemyCard.IsDead) yield break;

        Transform originalParent = roomManager.grid[enemyCard.index].transform;
        enemyCard.transform.SetParent(originalParent, true);
        enemyCard.ChangeCardState(CardState.Default);
        yield return StartCoroutine(enemyCard.interactionHandler.TransformCardUniformly
        (originalParent.position, Vector3.one, Vector3.zero, movementData.toFormationDuration));
    }

    #endregion

    #region Helpers

    private void ApplyDamage()
    {
        enemyCard.TakeDamage();
        playerCard.TakeDamage();
    }

    private void ToggleButtons(bool enable)
    {
        backButton.gameObject.SetActive(enable);
        battleButton.gameObject.SetActive(enable);
    }


    #endregion
}

