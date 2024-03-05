using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class InteractionState
{

    public CardInteractionStateMachine StateMachine { get; private set; }
    protected EventRegistry _events;
    public InteractionState(CardInteractionStateMachine stateMachine, EventRegistry events)
    {
        StateMachine = stateMachine;
        _events = events;
    }

    protected virtual void HandlePointerEnter(Interactable interactable, PointerEventData eventData) { }

    protected virtual void HandlePointerExit(Interactable interactable, PointerEventData eventData) { }

    protected virtual void HandleBeginDrag(Interactable interactable, PointerEventData eventData)
    {
        StateMachine.isDragging = true;
    }

    protected virtual void HandleDrag() { }

    protected virtual void HandleEndDrag(Interactable interactable, PointerEventData eventData)
    {
        StateMachine.isDragging = false;
    }

    protected virtual void HandlePointerClick(Interactable interactable, PointerEventData eventData) { }

}

public class Interactable : MonoBehaviour
{
    public InteractableType Type;
}