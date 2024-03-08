using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardInteractionHandler : MonoBehaviour
{
    [SerializeField] protected EventRegistry events;

    protected Card selectedCard = null;
    protected Card draggedCard = null;
    public bool isDragging = false;

    public void OnCardPointerEnter(Component sender, object data) 
    {
        HandleCardPointerEnter(sender as Card, data as PointerEventData);
    }
    
    protected virtual void HandleCardPointerEnter(Card card, PointerEventData eventData){}

    public void OnCardPointerExit(Component sender, object data)
    {
        HandleCardPointerExit(sender as Card, data as PointerEventData);
    }

    protected virtual void HandleCardPointerExit(Card card, PointerEventData eventData) { }


    public void OnCardBeginDrag(Component sender, object data)
    {
        HandleCardBeginDrag(sender as Card, data as PointerEventData);
    }

    protected virtual void HandleCardBeginDrag(Card card, PointerEventData eventData) { }

    private void Update()
    {
        if (isDragging)
        {
            HandleDrag();
        }
    }

    public virtual void HandleDrag() { }

    public void OnCardEndDrag(Component sender, object data)
    {
        HandleCardEndDrag(sender as Card, data as PointerEventData);
    }

    protected virtual void HandleCardEndDrag(Card card, PointerEventData eventData) { }

    public void OnCardPointerClick(Component sender, object data)
    {
        HandleCardPointerClick(sender as Card, data as PointerEventData);
    }

    protected virtual void HandleCardPointerClick(Card card, PointerEventData eventData) { }

    public void OnHandColliderPointerEnter(Component sender, object data)
    {
        HandleHandColliderPointerEnter(sender as HandCollisionDetector, data as PointerEventData);
    }

    protected virtual void HandleHandColliderPointerEnter(HandCollisionDetector HandCollider, PointerEventData eventData) { }

    public void OnHandColliderPointerExit(Component sender, object data)
    {
        HandleHandColliderPointerExit(sender as HandCollisionDetector, data as PointerEventData);
    }

    protected virtual void HandleHandColliderPointerExit(HandCollisionDetector HandCollider, PointerEventData eventData) { }

}
