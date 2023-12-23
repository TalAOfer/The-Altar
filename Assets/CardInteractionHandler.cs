using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class CardInteractionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Card card;
    public Vector3 startPos;
    private Vector3 temp;
    private Collider2D coll;

    [SerializeField] Color defaultColor;
    [SerializeField] Color hoverColor;
    [SerializeField] AllEvents events;


    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        card = GetComponent<Card>();
        card.cardSr.color = defaultColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        card.cardSr.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        card.cardSr.color = defaultColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = transform.position;
    }

    public void SetCollState(bool enable)
    {
        coll.enabled = enable;
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetCollState(false);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        temp = mousePos;
        temp.z = 0;
        transform.position = temp;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            //GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;

            //Debug.Log("Dropped on " + hitObject.name);
        }

        else
        {
            transform.position = startPos;
        }

        SetCollState(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.name);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Card attackingCard = eventData.pointerDrag.GetComponent<Card>(); 
        events.OnCardDropOnCard.Raise(card, attackingCard);
    }
}
