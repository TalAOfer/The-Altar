using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class CardInteractionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Card card;
    [SerializeField] private Collider2D coll;
    public Vector3 startPos;
    public Vector3 startRotation;
    private Vector3 temp;

    [SerializeField] Color defaultColor;
    [SerializeField] Color hoverColor;
    [SerializeField] AllEvents events;


    private void Awake()
    {
        card.visualHandler.SetCardBGColor(defaultColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        card.visualHandler.SetCardBGColor(hoverColor);
        card.visualHandler.SetSortingOrder(10);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        card.visualHandler.SetCardBGColor(defaultColor);
        card.visualHandler.SetSortingOrder(card.index);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = card.transform.position;
        startRotation = card.transform.rotation.eulerAngles;
        card.transform.localRotation = Quaternion.Euler(Vector3.zero);
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
        card.transform.position = temp;
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
            card.transform.position = startPos;
            card.transform.localRotation = Quaternion.Euler(startRotation);
        }

        SetCollState(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.name);
    }

    public void OnDrop(PointerEventData eventData)
    {
        CardInteractionHandler attackingCardInteractionHandler = eventData.pointerDrag.GetComponent<CardInteractionHandler>();
        Debug.Log(card.name + " attacked by " + attackingCardInteractionHandler.card);
        events.OnCardDropOnCard.Raise(card, attackingCardInteractionHandler.card);
    }
}
