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

        yield return Tools.GetWait(0.1f);

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

        yield return Tools.GetWait(0.1f);

        if (_effectApplier.RootEffectNode != null)
        {
            yield return new WaitUntil(() => effectsCompleted);
        }
    }


    protected virtual IEnumerator DeathPhase()
    {
        HashSet<Card> cardsTriggered = new HashSet<Card>(); // Tracks cards that have had effects triggered
        bool moreCardsPendingDestruction;

        do
        {
            moreCardsPendingDestruction = false;

            // Get all active cards pending destruction that haven't had effects triggered yet
            List<Card> pendingDestruction = data.GetAllActiveCards()
                .Where(card => card.PENDING_DESTRUCTION && !cardsTriggered.Contains(card))
                .ToList();

            if (pendingDestruction.Count <= 0) yield break; // Exit if no cards are pending destruction

            bool effectsCompleted = false;

            void OnEffectsComplete()
            {
                effectsCompleted = true;
                _effectApplier.OnEffectsCompleted -= OnEffectsComplete;
            }

            _effectApplier.OnEffectsCompleted += OnEffectsComplete;

            // Trigger effects for all cards pending destruction
            foreach (Card card in pendingDestruction)
            {
                cardsTriggered.Add(card); // Mark card as having had its effects triggered
                card.effects.TriggerEffects(TriggerType.LastBreath, null);
            }

            yield return Tools.GetWait(0.1f);

            if (_effectApplier.RootEffectNode != null)
            {
                yield return new WaitUntil(() => effectsCompleted);
            }

            pendingDestruction = pendingDestruction
                .Where(card => card.PENDING_DESTRUCTION) // Only include cards still marked for destruction
                .ToList();

            foreach (Card card in pendingDestruction)
            {
                StartCoroutine(card.DestroySelf()); // Initiate destruction of the card
            }

            yield return Tools.GetWait(0.25f);

            data.ReorderCards();

            // Check if there are any new cards pending destruction
            moreCardsPendingDestruction = data.GetAllActiveCards().Any(card => card.PENDING_DESTRUCTION && !cardsTriggered.Contains(card));

        } while (moreCardsPendingDestruction); // Repeat if there are more cards pending destruction
    }


    #endregion
}

