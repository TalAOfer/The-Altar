using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class BattleManager : MonoBehaviour
{
    protected BattleRoomDataProvider data;
    private RoomStateMachine _sm;
    private SMContext _ctx;
    private EffectApplier _effectApplier;
    protected Card EnemyCard => _ctx.BattlingEnemyCard;
    protected Card PlayerCard => _ctx.BattlingPlayerCard;

    [FoldoutGroup("Dependencies")]
    [SerializeField] protected EventRegistry events;
    [FoldoutGroup("Dependencies")]
    [SerializeField] protected CardData cardData;

    public void Initialize(RoomStateMachine sm, SMContext ctx)
    {
        _sm = sm;
        _ctx = ctx;
        _effectApplier = GetComponent<EffectApplier>();
        data = _sm.DataProvider;
    }

    public virtual IEnumerator BattleRoutine()
    {
        PlayerCard.movement.SnapCardToVisual();

        yield return RallyRoutine();

        yield return Tools.GetWait(0.5f);

        HeadbuttEffect headbutt = new(null, null, PlayerCard);
        EffectNode headbuttNode = new(headbutt, null, EnemyCard);

        yield return _effectApplier.InitializeEffectSequence(headbuttNode);

        yield return BloodthirstRoutine();

        yield return DeathPhase();

        yield return _sm.HandleAllShapeshiftsUntilStable();
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

    private IEnumerator BloodthirstRoutine()
    {
        bool effectsCompleted = false;

        _effectApplier.OnEffectsCompleted += () => effectsCompleted = true;

        List<Card> allCards = data.GetAllActiveCards();

        foreach (Card card in allCards)
        {
            card.effects.TriggerEffects(TriggerType.Bloodthirst, new NormalEventData(PlayerCard));
        }

        yield return Tools.GetWait(0.15f);

        if (_effectApplier.RootEffectNode != null)
        {
            yield return new WaitUntil(() => effectsCompleted);
        }
    }


    protected virtual IEnumerator DeathPhase()
    {
        bool moreCardsPendingDestruction;
        do
        {
            List<Card> pendingDestruction = data.GetAllActiveCards().Where(card => card.PENDING_DESTRUCTION).ToList();

            if (pendingDestruction.Count <= 0) yield break;

            moreCardsPendingDestruction = false;

            bool effectsCompleted = false;

            void OnEffectsComplete()
            {
                effectsCompleted = true;
                _effectApplier.OnEffectsCompleted -= OnEffectsComplete;
            }

            _effectApplier.OnEffectsCompleted += OnEffectsComplete;

            foreach (Card card in pendingDestruction)
            {
                card.effects.TriggerEffects(TriggerType.LastBreath, null);
            }

            yield return Tools.GetWait(0.1f);

            if (_effectApplier.RootEffectNode != null)
            {
                yield return new WaitUntil(() => effectsCompleted);
            }

            yield return Tools.GetWait(0.25f);

            pendingDestruction = data.GetAllActiveCards().Where(card => card.PENDING_DESTRUCTION).ToList();

            List<Coroutine> destructionRoutines = new();

            foreach (Card card in pendingDestruction)
            {
                destructionRoutines.Add(StartCoroutine(card.DestroySelf()));
            }

            foreach (Coroutine routine in destructionRoutines)
            {
                yield return routine;
            }

            data.ReorderCards();

            moreCardsPendingDestruction = data.GetAllActiveCards().Any(card => card.PENDING_DESTRUCTION);

        } while (moreCardsPendingDestruction);
    }


    #endregion

    #region Animation Routines 


    #endregion

}

