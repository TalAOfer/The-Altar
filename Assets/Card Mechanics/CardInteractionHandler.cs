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
    [FoldoutGroup("Components")]
    public Collider2D coll;

    public void SetInteractability(bool enable)
    {
        coll.enabled = enable;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Locator.Events.OnCardPointerEnter.Raise(card, eventData);
        //Debug.Log("Pointer Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Locator.Events.OnCardPointerExit.Raise(card, eventData);
        //Debug.Log("Pointer Exit");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Locator.Events.OnCardBeginDrag.Raise(card, eventData);
        //Debug.Log("Begin Drag");
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        Locator.Events.OnCardEndDrag.Raise(card, eventData);
        //Debug.Log("End Drag");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Locator.Events.OnCardClicked.Raise(card, eventData);
        //Debug.Log("Click");
    }

}