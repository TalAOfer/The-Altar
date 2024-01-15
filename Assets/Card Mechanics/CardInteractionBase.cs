using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardInteractionBase : MonoBehaviour
{
    [SerializeField] protected AllEvents events;

    protected Card draggedCard = null;
    protected bool isDragging = false;

    public void OnCardPointerEnter(Component sender, object data) 
    {
        HandlePointerEnter(sender as Card, data as PointerEventData);
    }
    
    protected virtual void HandlePointerEnter(Card card, PointerEventData eventData){}

    public void OnCardPointerExit(Component sender, object data)
    {
        HandlePointerExit(sender as Card, data as PointerEventData);
    }

    protected virtual void HandlePointerExit(Card card, PointerEventData eventData) { }


    public void OnCardBeginDrag(Component sender, object data)
    {
        HandleBeginDrag(sender as Card, data as PointerEventData);
    }

    protected virtual void HandleBeginDrag(Card card, PointerEventData eventData) { }

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
        HandleEndDrag(sender as Card, data as PointerEventData);
    }

    protected virtual void HandleEndDrag(Card card, PointerEventData eventData) { }

    public void OnCardPointerClick(Component sender, object data)
    {
        HandlePointerClick(sender as Card, data as PointerEventData);
    }

    protected virtual void HandlePointerClick(Card card, PointerEventData eventData) { }

}
