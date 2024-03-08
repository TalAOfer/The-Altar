using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;



public class BattleManager : MonoBehaviour
{
    protected BattleRoomDataProvider data;
    private BattleStateMachine _ctx;

    protected Card EnemyCard => _ctx.Ctx.BattlingEnemyCard;
    protected Card PlayerCard => _ctx.Ctx.BattlingPlayerCard;

    [FoldoutGroup("Dependencies")]
    [SerializeField] protected EventRegistry events;
    [FoldoutGroup("Dependencies")]
    [SerializeField] protected CardData cardData;

    public void Initialize(BattleStateMachine ctx)
    {
        _ctx = ctx;
        data = _ctx.DataProvider;
    }

    public virtual IEnumerator BattleRoutine()
    {
        PlayerCard.movement.SnapCardToVisual();

        yield return StartOfBattleRoutine();

        //events.AddLogEntry.Raise(this, "Support");
        yield return SupportEffectsRoutine();

        //events.AddLogEntry.Raise(this, "Before Attacking");
        yield return BeforeAttackingRoutine();

        //events.AddLogEntry.Raise(this, "Calculating Battle Points");
        yield return CalculateBattlePointsRoutine();

        PlayerCard.ToggleDamageVisual(true);
        EnemyCard.ToggleDamageVisual(true);

        yield return AnimateReadyAndHeadbutt();

        //Impact
        ApplyDamage();
        PlayerCard.ToggleDamageVisual(false);
        EnemyCard.ToggleDamageVisual(false);
        Tools.PlaySound("Card_Attack_Impact", PlayerCard.transform);
        events.ShakeScreen.Raise(this, CameraShakeTypes.Classic);

        yield return DeathRoutine();

        if (DidEnemyCardDie)
        {
            _ctx.EnemyCardManager.RemoveEnemyFromManager(EnemyCard);
        }

        if (DidPlayerCardDie) 
            _ctx.PlayerCardManager.RemoveCardFromManager(PlayerCard);

        //events.AddLogEntry.Raise(this, "Global Death");
        yield return GlobalDeathRoutine();

        yield return SurviveRoutine();

        //events.AddLogEntry.Raise(this, "Aftermath Shapeshifts");
        yield return _ctx.HandleAllShapeshiftsUntilStable();

        if (DidEnemyCardDie)
        {
            EnemyCard.gameObject.SetActive(false);
            _ctx.EnemyCardManager.ReorderPlaceholders();
            _ctx.EnemyCardManager.ResetCardsToPlaceholders();
        }

        if (DidPlayerCardDie) 
            PlayerCard.gameObject.SetActive(false);

        else yield return AnimateBackoff();

        //events.AddLogEntry.Raise(this, "On Action");
        yield return BloodthirstEffectsRoutine();

        //events.AddLogEntry.Raise(this, "After On Action Shapeshift");
        yield return _ctx.HandleAllShapeshiftsUntilStable();

        //events.AddLogEntry.Raise(this, "End Of Turn");
        yield return MeditateEffectsRoutine();

        //events.AddLogEntry.Raise(this, "After End Of Turn Shapeshift");
        yield return _ctx.HandleAllShapeshiftsUntilStable();

        if (!PlayerCard.IsDead)
        {
            yield return PlayerCard.movement.TransformCardUniformlyToPlaceholder(cardData.backOffSpeed, cardData.backoffCurve);
        }
    }

    #region Effect Routines



    private IEnumerator StartOfBattleRoutine()
    {
        //events.AddLogEntry.Raise(this, "Start Of Battle");

        #region Loop Count Debugger
        //int amountOfOccurances = 1;
        #endregion

        Coroutine ApplyPlayerCardStartOfBattleEffects = StartCoroutine(PlayerCard.effects.ApplyEffects(TriggerType.StartOfBattle));
        Coroutine ApplyEnemyCardStartOfBattleEffects = StartCoroutine(EnemyCard.effects.ApplyEffects(TriggerType.StartOfBattle));

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

            bool didPlayerShapeshift = PlayerCard.ShouldShapeshift();
            bool didEnemyShapeshift = EnemyCard.ShouldShapeshift();
            battleStartEnded = (!didPlayerShapeshift && !didEnemyShapeshift);

            if (!battleStartEnded)
            {
                yield return (_ctx.HandleAllShapeshiftsUntilStable());
            }

            Coroutine ReapplyPlayerCardStartOfBattleEffects = null;
            Coroutine ReapplyEnemyCardStartOfBattleEffects = null;

            if (didPlayerShapeshift) ReapplyPlayerCardStartOfBattleEffects = StartCoroutine(PlayerCard.effects.ApplyEffects(TriggerType.StartOfBattle));
            if (didEnemyShapeshift) ReapplyEnemyCardStartOfBattleEffects = StartCoroutine(EnemyCard.effects.ApplyEffects(TriggerType.StartOfBattle));

            if (ReapplyPlayerCardStartOfBattleEffects != null) yield return ReapplyPlayerCardStartOfBattleEffects;
            if (ReapplyEnemyCardStartOfBattleEffects != null) yield return ReapplyEnemyCardStartOfBattleEffects;

            yield return null;
        }
    }

    private IEnumerator BeforeAttackingRoutine()
    {
        Coroutine ApplyPlayerCardBeforeAttackingEffects = StartCoroutine(PlayerCard.effects.ApplyEffects(TriggerType.BeforeAttacking));
        Coroutine ApplyEnemyCardBeforeAttackingEffects = StartCoroutine(EnemyCard.effects.ApplyEffects(TriggerType.BeforeAttacking));

        yield return ApplyPlayerCardBeforeAttackingEffects;
        yield return ApplyEnemyCardBeforeAttackingEffects;
    }

    private IEnumerator SupportEffectsRoutine()
    {
        foreach (Card card in _ctx.PlayerCardManager.ActiveCards)
        {
            yield return StartCoroutine(card.effects.ApplyEffects(TriggerType.Support));
        }

        foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies)
        {
            if (card == EnemyCard) continue;
            yield return StartCoroutine(card.effects.ApplyEffects(TriggerType.Support));
        }
    }

    private IEnumerator CalculateBattlePointsRoutine()
    {
        Coroutine calcEnemyCardAttackPoints = StartCoroutine(EnemyCard.CalcAttackPoints());
        Coroutine calcPlayerCardAtackPoints = StartCoroutine(PlayerCard.CalcAttackPoints());

        yield return calcEnemyCardAttackPoints;
        yield return calcPlayerCardAtackPoints;

        Coroutine calcPlayerCardHurtPoints = StartCoroutine(PlayerCard.CalcHurtPoints(EnemyCard.attackPoints.value));
        Coroutine calcEnemyCardHurtPoints = StartCoroutine(EnemyCard.CalcHurtPoints(PlayerCard.attackPoints.value));

        yield return calcPlayerCardHurtPoints;
        yield return calcEnemyCardHurtPoints;
    }

    protected virtual IEnumerator DeathRoutine()
    {
        Coroutine ApplyPlayerCardOnDeathEffects = null;
        Coroutine ApplyEnemyCardOnDeathEffects = null;
        bool playerCardDied = PlayerCard.IsDead;
        bool enemyCardDied = EnemyCard.IsDead;

        if (playerCardDied) ApplyPlayerCardOnDeathEffects = StartCoroutine(PlayerCard.effects.ApplyEffects(TriggerType.OnDeath));
        if (enemyCardDied) ApplyEnemyCardOnDeathEffects = StartCoroutine(EnemyCard.effects.ApplyEffects(TriggerType.OnDeath));

        if (ApplyPlayerCardOnDeathEffects != null) yield return ApplyPlayerCardOnDeathEffects;
        if (ApplyEnemyCardOnDeathEffects != null) yield return ApplyEnemyCardOnDeathEffects;

        Coroutine ApplyPlayerDeathShapeshift = null;
        Coroutine ApplyEnemyDeathShapeshift = null;

        yield return Tools.GetWait(cardData.impactFreezeDuration);
        if (playerCardDied || enemyCardDied) Tools.PlaySound("Card_Death", transform);

        if (playerCardDied)
        {
            ApplyPlayerDeathShapeshift = StartCoroutine(PlayerCard.Shapeshift());
            _ctx.PlayerCardManager.RemoveCardFromManager(PlayerCard);
        }

        if (enemyCardDied) ApplyEnemyDeathShapeshift = StartCoroutine(EnemyCard.Shapeshift());

        if (ApplyPlayerDeathShapeshift != null) yield return ApplyPlayerDeathShapeshift;
        if (ApplyEnemyDeathShapeshift != null) yield return ApplyEnemyDeathShapeshift;

    }

    private IEnumerator GlobalDeathRoutine()
    {
        if (EnemyCard.IsDead || PlayerCard.IsDead)
        {
            foreach (Card card in _ctx.PlayerCardManager.ActiveCards)
            {
                if (!card.IsDead)
                {
                    yield return card.effects.ApplyEffects(TriggerType.OnGlobalDeath);
                }
            }

            foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies)
            {
                if (!card.IsDead)
                {
                    yield return card.effects.ApplyEffects(TriggerType.OnGlobalDeath);
                }
            }
        }
    }

    private IEnumerator SurviveRoutine()
    {
        Coroutine ApplyPlayerCardOnSurviveEffects = null;
        Coroutine ApplyEnemyCardOnSurviveEffects = null;

        if (!PlayerCard.IsDead) ApplyPlayerCardOnSurviveEffects = StartCoroutine(PlayerCard.effects.ApplyEffects(TriggerType.OnSurvive));
        if (!EnemyCard.IsDead) ApplyEnemyCardOnSurviveEffects = StartCoroutine(EnemyCard.effects.ApplyEffects(TriggerType.OnSurvive));

        if (ApplyPlayerCardOnSurviveEffects != null) yield return ApplyPlayerCardOnSurviveEffects;
        if (ApplyEnemyCardOnSurviveEffects != null) yield return ApplyEnemyCardOnSurviveEffects;
    }

    private IEnumerator BloodthirstEffectsRoutine()
    {
        //Debug.Log("Starting player on action effects application");
        foreach (Card card in _ctx.PlayerCardManager.ActiveCards)
        {
            //Debug.Log("Applying " + card.name + "'s effects");
            yield return card.effects.ApplyEffects(TriggerType.Bloodthirst);
        }

        //Debug.Log("Starting enemy on action effects application");
        foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies)
        {
            //Debug.Log("Applying " + card.name + "'s effects");
            yield return card.effects.ApplyEffects(TriggerType.Bloodthirst);
        }
    }

    private IEnumerator MeditateEffectsRoutine()
    {
        //Debug.Log("Starting player end of turn effects application");
        foreach (Card card in _ctx.PlayerCardManager.ActiveCards)
        {
            //Debug.Log("Applying " + card.name + "'s effects");
            yield return card.effects.ApplyEffects(TriggerType.Meditate);
        }

        foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies)
        {
            //Debug.Log("Applying " + card.name + "'s effects");
            yield return card.effects.ApplyEffects(TriggerType.Meditate);
        }
    }

    #endregion

    #region Animation Routines 

    private IEnumerator AnimateReadyAndHeadbutt()
    {
        Vector3 targetPos = PlayerCard.transform.position;
        targetPos.y -= cardData.readyingDistance;
        Tools.PlaySound("Card_Attack_Prepare", PlayerCard.transform);
        Coroutine playerCardReadying = StartCoroutine(PlayerCard.movement.TransformCardUniformly(PlayerCard.transform, targetPos, Vector3.one, null, cardData.readyingSpeed, cardData.readyingCurve));

        yield return playerCardReadying;

        Tools.PlaySound("Card_Attack_Woosh", PlayerCard.transform);
        StartCoroutine(RemoveCardFromHand());

        Vector2 enemyCardClosestCollPos = EnemyCard.movement.GetClosestCollPosToOtherCard(PlayerCard.transform.position);
        Coroutine playerCardHeadbutt = StartCoroutine(PlayerCard.movement.TransformCardUniformly(PlayerCard.transform, enemyCardClosestCollPos, Vector3.one, null, cardData.headbuttSpeed, cardData.headbuttCurve));

        yield return playerCardHeadbutt;
    }

    private IEnumerator RemoveCardFromHand()
    {
        yield return Tools.GetWait(0.1f);
        _ctx.PlayerCardManager.Hand.RemoveCardFromHand(PlayerCard);
    }

    public virtual IEnumerator AnimateBackoff()
    {
        _ctx.PlayerCardManager.Hand.AddCardToHand(PlayerCard);
        PlayerCard.visualHandler.SetSortingOrder(PlayerCard.index);
        Tools.PlaySound("Card_Attack_Backoff", transform);
        yield return StartCoroutine(PlayerCard.movement.TransformCardUniformlyToHoveredPlaceholder(cardData.backOffSpeed, cardData.backoffCurve));
        PlayerCard.visualHandler.SetSortingLayer(GameConstants.PLAYER_CARD_LAYER);
    }

    #endregion


    #region Helpers

    private void ApplyDamage()
    {
        EnemyCard.TakeDamage(this);
        PlayerCard.TakeDamage(this);
    }

    private bool DidEnemyCardDie => EnemyCard.IsDead;
    private bool DidPlayerCardDie => PlayerCard.IsDead;
    #endregion
}

