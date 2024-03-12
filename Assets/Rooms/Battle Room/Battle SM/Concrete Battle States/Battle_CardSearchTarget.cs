using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Battle_CardSearchTarget : BaseBattleRoomState
{
    private Card TargetSeekingCard => _ctx.Ctx.CurrentActorCard;
    private Card TargetCard => _ctx.Ctx.CurrentTargetCard;
    public Battle_CardSearchTarget(BattleStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies)
        {
            card.visualHandler.EnableOutline(PaletteColor.attackable);
        }

        _ctx.Events.EnableBezierArrow.Raise(_ctx, TargetSeekingCard);

        TargetSeekingCard.CalculateDamage(TargetCard);
        TargetSeekingCard.visualHandler.EnableDamageVisual();

        if (TargetCard != null)
        {
            TargetCard.CalculateDamage(TargetSeekingCard);
            TargetCard.visualHandler.EnableDamageVisual();
        }
        
        else
        {
            foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies)
            {
                card.visualHandler.DisableDamageVisual();
            }
        }

        return base.EnterState();
    }

    public override IEnumerator ExitState()
    {
        _ctx.Ctx.CardClicked = null;

        foreach (Card card in _ctx.EnemyCardManager.ActiveEnemies)
        {
            card.visualHandler.DisableOutline();
        }

        TargetSeekingCard.visualHandler.DisableDamageVisual();

        _ctx.Events.DisableBezierArrow.Raise();
        return base.ExitState();
    }

    public override void HandleEnemyCardPointerEnter(Card card, PointerEventData eventData)
    {
        _ctx.Ctx.CurrentTargetCard = card;
        TargetCard.visualHandler.Animate("Jiggle");
        _ctx.SwitchState(_ctx.States.CardSearchTarget());
    }

    public override void HandleEnemyCardPointerExit(Card card, PointerEventData eventData)
    {
        _ctx.Ctx.CurrentTargetCard = null;
        _ctx.SwitchState(_ctx.States.CardSearchTarget());
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

        if (droppedCard.Affinity == Affinity.Enemy)
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
