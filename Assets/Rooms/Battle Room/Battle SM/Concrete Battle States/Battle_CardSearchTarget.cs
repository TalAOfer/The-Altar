using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Battle_CardSearchTarget : BaseRoomState
{
    public Battle_CardSearchTarget(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    private Card TargetSeekingCard => _ctx.CurrentActorCard;


    public override IEnumerator EnterState()
    {
        HighlightAttackableTargets();
        Events.EnableBezierArrow.Raise(_sm, TargetSeekingCard);
        TargetSeekingCard.visualHandler.EnableDamageVisual();
        return base.EnterState();
    }

    public override IEnumerator ExitState()
    {
        ClearTargetSelection();
        Events.DisableBezierArrow.Raise();
        return base.ExitState();
    }

    private void HighlightAttackableTargets()
    {
        var tauntCards = _sm.EnemyCardManager.ActiveEnemies.Where(card => card.Taunt).ToList();
        bool isThereATauntOnMap = tauntCards.Any();

        foreach (Card card in _sm.EnemyCardManager.ActiveEnemies)
        {
            if (!isThereATauntOnMap || card.Taunt)
            {
                card.visualHandler.EnableOutline(PaletteColor.attackable);
            }
            else
            {
                card.Targetable = false;
                card.visualHandler.ToggleDarkOverlay(true);
            }
        }
    }

    private void ClearTargetSelection()
    {
        foreach (Card card in _sm.EnemyCardManager.ActiveEnemies)
        {
            card.visualHandler.DisableOutline();
            card.visualHandler.ToggleDarkOverlay(false);
            card.Targetable = true;
            card.visualHandler.DisableDamageVisual();
        }

        TargetSeekingCard.visualHandler.DisableOutline();
        TargetSeekingCard.visualHandler.DisableDamageVisual();

        if (_ctx.BattlingEnemyCard != null)
        {
            _ctx.BattlingEnemyCard.visualHandler.DisableDamageVisual();
        }
    }

    public void UpdateTargetVisuals(Card newTarget)
    {
        if (_ctx.CurrentTargetCard != null)
        {
            _ctx.CurrentTargetCard.visualHandler.DisableOutline();
            _ctx.CurrentTargetCard.visualHandler.DisableDamageVisual();
        }

        _ctx.CurrentTargetCard = newTarget;

        if (newTarget != null)
        {
            newTarget.visualHandler.EnableOutline(PaletteColor.attackable);
            newTarget.CalculateDamage(TargetSeekingCard);
            newTarget.visualHandler.EnableDamageVisual();
        }

        TargetSeekingCard.CalculateDamage(newTarget);

    }

    // Simplify the pointer event handlers by directly calling UpdateTargetVisuals with the appropriate card or null
    public override void HandleEnemyCardPointerEnter(Card card, PointerEventData eventData)
    {
        if (card.Targetable)
        {
            UpdateTargetVisuals(card);
        }
    }

    public override void HandleEnemyCardPointerExit(Card card, PointerEventData eventData)
    {
        UpdateTargetVisuals(null); // Null indicates no target is currently hovered
    }

    #region Transitions

    //Back to idle
    public override void OnHandColliderPointerEnter(HandCollisionDetector HandCollisionManager, PointerEventData data)
    {
        SwitchTo(States.Idle());
    }

    //Fight
    public override void HandleEnemyCardPointerClick(Card cardClicked, PointerEventData eventData)
    {
        _ctx.BattlingPlayerCard = _ctx.CurrentActorCard;
        _ctx.BattlingEnemyCard = cardClicked;
        SwitchTo(States.Battle());
    }

    public override void HandlePlayerCardEndDrag(Card card, PointerEventData eventData)
    {
        GameObject goHit = eventData.pointerCurrentRaycast.gameObject;

        if (goHit == null)
        {
            SwitchTo(States.Idle());
            return;
        }

        InteractionEventEmitter cardEmitter = goHit.GetComponent<InteractionEventEmitter>();
        Card droppedCard = cardEmitter != null ? cardEmitter.card : null;

        if (droppedCard == null)
        {
            SwitchTo(States.Idle());
            return;
        }

        if (droppedCard.Affinity == Affinity.Enemy && droppedCard.Targetable)
        {
            _ctx.BattlingPlayerCard = _ctx.CurrentActorCard;
            _ctx.CurrentActorCard.visualHandler.DisableOutline();

            _ctx.BattlingEnemyCard = droppedCard;

            SwitchTo(States.Battle());
        }

        else
        {
            SwitchTo(States.Idle());
        }
    }

    #endregion

}
