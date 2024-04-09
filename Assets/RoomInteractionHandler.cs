using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomInteractionHandler : CardInteractionHandler
{
    private RoomStateMachine _sm;

    private void Awake()
    {
        _sm = GetComponentInParent<RoomStateMachine>();
    }
    public override void HandleDrag()
    {
        base.HandleDrag();
    }

    protected override void HandleCardBeginDrag(Card card, PointerEventData eventData)
    {
        if (card.Affinity == Affinity.Player) _sm.OnPlayerCardBeginDrag(card, eventData);
        isDragging = true;
    }

    protected override void HandleCardEndDrag(Card card, PointerEventData eventData)
    {
        if (card.Affinity == Affinity.Player) _sm.OnPlayerCardEndDrag(card, eventData);
        isDragging = false;
    }

    protected override void HandleCardPointerClick(Card card, PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            return;
        }

        if (card.Affinity == Affinity.Player) _sm.OnPlayerCardClicked(card, eventData);
        else _sm.OnEnemyCardClicked(card, eventData);
    }

    protected override void HandleCardPointerEnter(Card card, PointerEventData eventData)
    {
        if (card.Affinity == Affinity.Player) _sm.OnPlayerCardPointerEnter(card, eventData);
        else _sm.OnEnemyCardPointerEnter(card, eventData);
    }

    protected override void HandleCardPointerExit(Card card, PointerEventData eventData)
    {
        if (card.Affinity == Affinity.Player) _sm.OnPlayerCardPointerExit(card, eventData);
        else _sm.OnEnemyCardPointerExit(card, eventData);
    }

    protected override void HandleHandColliderPointerEnter(HandCollisionDetector HandCollider, PointerEventData eventData)
    {
        _sm.OnHandColliderPointerEnter(HandCollider, eventData);
    }
    protected override void HandleHandColliderPointerExit(HandCollisionDetector HandCollider, PointerEventData eventData)
    {
        _sm.OnHandColliderPointerExit(HandCollider, eventData);
    }

    public void OnAbilityClicked(Component sender, object data)
    {
        _sm.OnAbilityClicked(sender as AbilityManager, data as Ability);
    }

    public void OnRoomButtonClicked(Component sender, object data)
    {
        _sm.OnRoomButtonClicked(sender as CustomButton, (int)data);
    }
}
