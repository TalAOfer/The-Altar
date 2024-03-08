using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardInteractionState
{
    public CardInteractionHandler StateMachine { get; private set; }
    protected EventRegistry _events;
    public CardInteractionState(CardInteractionHandler stateMachine, EventRegistry events)
    {
        StateMachine = stateMachine;
        _events = events;
    }

    protected virtual void HandlePointerEnter(Card card, PointerEventData eventData) { }

    protected virtual void HandlePointerExit(Card card, PointerEventData eventData) { }

    protected virtual void HandleBeginDrag(Card card, PointerEventData eventData) 
    {
        StateMachine.isDragging = true;
    }

    protected virtual void HandleDrag() { }

    protected virtual void HandleEndDrag(Card card, PointerEventData eventData) 
    {
        StateMachine.isDragging = false;
    }

    protected virtual void HandlePointerClick(Card card, PointerEventData eventData) { }

}
