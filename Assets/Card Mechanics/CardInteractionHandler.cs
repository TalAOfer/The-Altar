using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
[RequireComponent(typeof(Collider2D))]
public class CardInteractionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [FoldoutGroup("Components")]
    public Card card;

    public void OnPointerEnter(PointerEventData eventData)
    {
        card.events.OnCardPointerEnter.Raise(card, eventData);
        //Debug.Log("Pointer Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        card.events.OnCardPointerExit.Raise(card, eventData);
        //Debug.Log("Pointer Exit");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        card.events.OnCardBeginDrag.Raise(card, eventData);
        //Debug.Log("Begin Drag");
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        card.events.OnCardEndDrag.Raise(card, eventData);
        //Debug.Log("End Drag");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        card.events.OnCardClicked.Raise(card, eventData);
        //Debug.Log("Click");
    }

}