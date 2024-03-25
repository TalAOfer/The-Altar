using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        HeadbuttEffect headbutt = new(null, null, PlayerCard);
        EffectNode headbuttNode = new(headbutt, null, EnemyCard);

        yield return _effectApplier.InitializeEffectSequence(headbuttNode);

        Debug.Log("Death phase");

        yield return DeathPhase();

        Debug.Log("Death Phase ended");

        yield return _ctx.HandleAllShapeshiftsUntilStable();

        Debug.Log("Finished shapeshifts");
    }


    #region Effect Routines

    private IEnumerator RallyRoutine()
    {
        bool effectsCompleted = false;

        _effectApplier.OnEffectsCompleted += () => effectsCompleted = true;

        PlayerCard.effects.TriggerEffects(TriggerType.Rally, new NormalEventData(PlayerCard));
        EnemyCard.effects.TriggerEffects(TriggerType.Rally, new NormalEventData(EnemyCard));

        yield return Tools.GetWait(0.15f);

        if (_effectApplier.RootEffectNode != null)
        {
            yield return new WaitUntil(() => effectsCompleted);
        }
    }

    protected virtual IEnumerator DeathPhase()
    {
        List<Card> pendingDestruction = data.GetAllActiveCards().Where(card => card.PENDING_DESTRUCTION).ToList();

        if (pendingDestruction.Count <= 0) yield break;

        bool effectsCompleted = false;

        _effectApplier.OnEffectsCompleted += () => effectsCompleted = true;

        foreach (Card card in pendingDestruction)
        {
            card.effects.TriggerEffects(TriggerType.LastBreath, null);
        }

        yield return Tools.GetWait(0.15f);

        if (_effectApplier.RootEffectNode != null)
        {
            yield return new WaitUntil(() => effectsCompleted);
        }

        foreach (Card card in pendingDestruction)
        {
            StartCoroutine(card.DestroySelf());
        }
    }

    private IEnumerator BloodthirstRoutine()
    {
        bool effectsCompleted = false;

        _effectApplier.OnEffectsCompleted += () => effectsCompleted = true;

        foreach (Card card in _ctx.PlayerCardManager.ActiveCards)
        {
            card.effects.TriggerEffects(TriggerType.Bloodthirst, null);
        }

        foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies)
        {
            card.effects.TriggerEffects(TriggerType.Bloodthirst, null);
        }

        // Wait for the effect applier to complete processing
        yield return new WaitUntil(() => effectsCompleted);
    }

    #endregion

    #region Animation Routines 


    #endregion


    #region Helpers

    private bool DidEnemyCardDie => EnemyCard.PENDING_DESTRUCTION;
    private bool DidPlayerCardDie => PlayerCard.PENDING_DESTRUCTION;
    #endregion
}

