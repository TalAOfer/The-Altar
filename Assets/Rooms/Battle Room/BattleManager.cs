using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;



public class BattleManager : MonoBehaviour
{
    protected BattleRoomDataProvider data;
    private BattleRoomStateMachine _ctx;
    private EffectApplier _effectApplier;
    protected Card EnemyCard => _ctx.Ctx.BattlingEnemyCard;
    protected Card PlayerCard => _ctx.Ctx.BattlingPlayerCard;

    [FoldoutGroup("Dependencies")]
    [SerializeField] protected EventRegistry events;
    [FoldoutGroup("Dependencies")]
    [SerializeField] protected CardData cardData;

    public void Initialize(BattleRoomStateMachine ctx)
    {
        _ctx = ctx;
        _effectApplier = GetComponent<EffectApplier>();
        data = _ctx.DataProvider;
    }

    public virtual IEnumerator BattleRoutine()
    {
        PlayerCard.movement.SnapCardToVisual();

        yield return RallyRoutine();

        InitialDamageEffect headbutt = new(null, null, PlayerCard);

        yield return _effectApplier.InitializeEffectSequence(headbutt, EnemyCard);

        //events.AddLogEntry.Raise(this, "On Action");
        yield return BloodthirstEffectsRoutine();

        //events.AddLogEntry.Raise(this, "After On Action Shapeshift");
        yield return _ctx.HandleAllShapeshiftsUntilStable();

        if (!PlayerCard.IsDead)
        {
            yield return PlayerCard.movement.TransformCardUniformlyToPlaceholder(cardData.backOffSpeed, cardData.backoffCurve);
        }
    }


    #region Effect Routines



    private IEnumerator RallyRoutine()
    {
        Coroutine ApplyPlayerCardBeforeAttackingEffects = StartCoroutine(PlayerCard.effects.ApplyEffects(TriggerType.Rally, null));
        Coroutine ApplyEnemyCardBeforeAttackingEffects = StartCoroutine(EnemyCard.effects.ApplyEffects(TriggerType.Rally, null));

        yield return ApplyPlayerCardBeforeAttackingEffects;
        yield return ApplyEnemyCardBeforeAttackingEffects;
    }

    protected virtual IEnumerator LastBreathRoutine()
    {
        Coroutine ApplyPlayerCardOnDeathEffects = null;
        Coroutine ApplyEnemyCardOnDeathEffects = null;
        bool playerCardDied = PlayerCard.IsDead;
        bool enemyCardDied = EnemyCard.IsDead;

        if (playerCardDied) ApplyPlayerCardOnDeathEffects = StartCoroutine(PlayerCard.effects.ApplyEffects(TriggerType.LastBreath, null));
        if (enemyCardDied) ApplyEnemyCardOnDeathEffects = StartCoroutine(EnemyCard.effects.ApplyEffects(TriggerType.LastBreath, null));

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

    private IEnumerator BloodthirstEffectsRoutine()
    {
        //Debug.Log("Starting player on action effects application");
        foreach (Card card in _ctx.PlayerCardManager.ActiveCards)
        {
            //Debug.Log("Applying " + card.name + "'s effects");
            yield return card.effects.ApplyEffects(TriggerType.Bloodthirst, null);
        }

        //Debug.Log("Starting enemy on action effects application");
        foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies)
        {
            //Debug.Log("Applying " + card.name + "'s effects");
            yield return card.effects.ApplyEffects(TriggerType.Bloodthirst, null);
        }
    }

    #endregion

    #region Animation Routines 

    private IEnumerator AnimateReadyAndHeadbutt()
    {
        Vector3 targetPos = PlayerCard.transform.position;
        targetPos.y -= cardData.readyingDistance;
        Tools.PlaySound("Card_Attack_Prepare", PlayerCard.transform);
        Coroutine playerCardReadying = StartCoroutine(PlayerCard.movement.MoveCard(PlayerCard.transform, targetPos, Vector3.one, null, cardData.readyingSpeed, cardData.readyingCurve));

        yield return playerCardReadying;

        Tools.PlaySound("Card_Attack_Woosh", PlayerCard.transform);
        StartCoroutine(RemoveCardFromHand());

        Vector2 enemyCardClosestCollPos = EnemyCard.movement.GetClosestCollPosToOtherCard(PlayerCard.transform.position);
        Coroutine playerCardHeadbutt = StartCoroutine(PlayerCard.movement.MoveCard(PlayerCard.transform, enemyCardClosestCollPos, Vector3.one, null, cardData.headbuttSpeed, cardData.headbuttCurve));

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

    private bool DidEnemyCardDie => EnemyCard.IsDead;
    private bool DidPlayerCardDie => PlayerCard.IsDead;
    #endregion
}

