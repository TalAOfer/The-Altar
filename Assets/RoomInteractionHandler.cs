using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomInteractionHandler : CardInteractionHandler
{
    private RoomStateMachine _ctx;

    private void Awake()
    {
        _ctx = GetComponentInParent<RoomStateMachine>();
    }
    public override void HandleDrag()
    {
        base.HandleDrag();
    }

    protected override void HandleCardBeginDrag(Card card, PointerEventData eventData)
    {
        if (card.Affinity == Affinity.Player) _ctx.OnPlayerCardBeginDrag(card, eventData);
        isDragging = true;
    }

    protected override void HandleCardEndDrag(Card card, PointerEventData eventData)
    {
        if (card.Affinity == Affinity.Player) _ctx.OnPlayerCardEndDrag(card, eventData);
        isDragging = false;
    }

    protected override void HandleCardPointerClick(Card card, PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            return;
        }

        if (card.Affinity == Affinity.Player) _ctx.OnPlayerCardClicked(card, eventData);
        else _ctx.OnEnemyCardClicked(card, eventData);
    }

    protected override void HandleCardPointerEnter(Card card, PointerEventData eventData)
    {
        if (card.Affinity == Affinity.Player) _ctx.OnPlayerCardPointerEnter(card, eventData);
        else _ctx.OnEnemyCardPointerEnter(card, eventData);
    }

    protected override void HandleCardPointerExit(Card card, PointerEventData eventData)
    {
        if (card.Affinity == Affinity.Player) _ctx.OnPlayerCardPointerExit(card, eventData);
        else _ctx.OnEnemyCardPointerExit(card, eventData);
    }

    protected override void HandleHandColliderPointerEnter(HandCollisionDetector HandCollider, PointerEventData eventData)
    {
        _ctx.OnHandColliderPointerEnter(HandCollider, eventData);
    }
    protected override void HandleHandColliderPointerExit(HandCollisionDetector HandCollider, PointerEventData eventData)
    {
        _ctx.OnHandColliderPointerExit(HandCollider, eventData);
    }

    public void OnAbilityClicked(Component sender, object data)
    {
        _ctx.OnAbilityClicked(sender as AbilityManager, data as Ability);
    }

}
