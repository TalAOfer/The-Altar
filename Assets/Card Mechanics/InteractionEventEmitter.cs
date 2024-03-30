using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
[RequireComponent(typeof(Collider2D))]
public class InteractionEventEmitter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [FoldoutGroup("Components")]
    public Card card;
    [FoldoutGroup("Components")]
    public Collider2D coll;

    private EventRegistry _events;
    public bool IsCursorOn {  get; private set; }

    private void Awake()
    {
        _events = Locator.Events;
    }

    public void SetInteractability(bool enable)
    {
        coll.enabled = enable;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        _events.OnCardPointerEnter.Raise(card, eventData);
        IsCursorOn = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _events.OnCardPointerExit.Raise(card, eventData);
        IsCursorOn = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _events.OnCardBeginDrag.Raise(card, eventData);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        _events.OnCardEndDrag.Raise(card, eventData);
        //Debug.Log("End Drag");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _events.OnCardClicked.Raise(card, eventData);
        //Debug.Log("Click");
    }
}