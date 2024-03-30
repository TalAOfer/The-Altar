using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Battle_CardSearchTarget : BaseBattleRoomState
{
    private Card TargetSeekingCard => _ctx.Ctx.CurrentActorCard;

    public Battle_CardSearchTarget(BattleRoomStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        HighlightAttackableTargets();
        _ctx.Events.EnableBezierArrow.Raise(_ctx, TargetSeekingCard);
        TargetSeekingCard.visualHandler.EnableDamageVisual();
        return base.EnterState();
    }

    public override IEnumerator ExitState()
    {
        ClearTargetSelection();
        _ctx.Events.DisableBezierArrow.Raise();
        return base.ExitState();
    }

    private void HighlightAttackableTargets()
    {
        var tauntCards = _ctx.EnemyCardManager.ActiveEnemies.Where(card => card.Taunt).ToList();
        bool isThereATauntOnMap = tauntCards.Any();

        foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies)
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
        foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies)
        {
            card.visualHandler.DisableOutline();
            card.visualHandler.ToggleDarkOverlay(false);
            card.Targetable = true;
            card.visualHandler.DisableDamageVisual();
        }

        TargetSeekingCard.visualHandler.DisableOutline();
        TargetSeekingCard.visualHandler.DisableDamageVisual();

        if (_ctx.Ctx.BattlingEnemyCard != null)
        {
            _ctx.Ctx.BattlingEnemyCard.visualHandler.DisableDamageVisual();
        }
    }

    public void UpdateTargetVisuals(Card newTarget)
    {
        if (_ctx.Ctx.CurrentTargetCard != null)
        {
            _ctx.Ctx.CurrentTargetCard.visualHandler.DisableOutline();
            _ctx.Ctx.CurrentTargetCard.visualHandler.DisableDamageVisual();
        }

        _ctx.Ctx.CurrentTargetCard = newTarget;

        if (newTarget != null)
        {
            newTarget.visualHandler.EnableOutline(PaletteColor.attackable);
            TargetSeekingCard.CalculateDamage(newTarget);
            newTarget.visualHandler.EnableDamageVisual();
        }

        TargetSeekingCard.CalculateDamage(null);

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
        _ctx.SwitchState(_ctx.States.Idle());
    }

    //Fight
    public override void HandleEnemyCardPointerClick(Card cardClicked, PointerEventData eventData)
    {
        _ctx.Ctx.BattlingPlayerCard = _ctx.Ctx.CurrentActorCard;
        _ctx.Ctx.BattlingEnemyCard = cardClicked;
        _ctx.SwitchState(_ctx.States.Battle());
    }

    public override void HandlePlayerCardEndDrag(Card card, PointerEventData eventData)
    {
        GameObject goHit = eventData.pointerCurrentRaycast.gameObject;

        if (goHit == null)
        {
            _ctx.SwitchState(_ctx.States.Idle());
            return;
        }

        InteractionEventEmitter cardEmitter = goHit.GetComponent<InteractionEventEmitter>();
        Card droppedCard = cardEmitter != null ? cardEmitter.card : null;

        if (droppedCard == null)
        {
            _ctx.SwitchState(_ctx.States.Idle());
            return;
        }

        if (droppedCard.Affinity == Affinity.Enemy && droppedCard.Targetable)
        {
            _ctx.Ctx.BattlingPlayerCard = _ctx.Ctx.CurrentActorCard;
            _ctx.Ctx.CurrentActorCard.visualHandler.DisableOutline();

            _ctx.Ctx.BattlingEnemyCard = droppedCard;

            _ctx.SwitchState(_ctx.States.Battle());
        }

        else
        {
            _ctx.SwitchState(_ctx.States.Idle());
        }
    }

    #endregion

}
