using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class CardInteractionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Card card;
    [SerializeField] private Collider2D coll;
    public Vector3 startScale;
    public Vector3 startPos;
    public Vector3 startRotation;
    private Vector3 temp;

    [SerializeField] Color defaultColor;
    [SerializeField] Color hoverColor;
    [SerializeField] AllEvents events;
    [SerializeField] private DragManager dragManager;
    public void Initialize()
    {
        card.visualHandler.SetCardBGColor(defaultColor);
        SetNewDefaultLocation();
    }

    public void SetNewDefaultLocation()
    {
        startScale = card.transform.localScale;
        startPos = card.transform.localPosition;
        startRotation = card.transform.rotation.eulerAngles;
    }

    private void RestartTransformToDefault()
    {
        card.transform.localScale = startScale;
        card.transform.localPosition = startPos;
        card.transform.rotation = Quaternion.Euler(startRotation);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!dragManager.isCardDragged)
        {
            card.visualHandler.SetCardBGColor(hoverColor);
            card.visualHandler.SetSortingOrder(10);
            card.transform.localPosition = new Vector3(transform.position.x, 0.5f, transform.position.z);
            card.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        else
        {
            events.OnDraggedCardHoveredOverHandCard.Raise(card, dragManager.draggedCard);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!dragManager.isCardDragged)
        {
            card.visualHandler.SetCardBGColor(defaultColor);
            card.visualHandler.SetSortingOrder(card.index);
            RestartTransformToDefault();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (card.cardOwner == CardOwner.Enemy) return;
        if (card.cardState == CardState.Battle) return;

        SetCollState(false);
        dragManager.SetDraggedCard(card);

        //To take it out of hand
        events.OnHandCardStartDrag.Raise(this, card);

        startPos = card.transform.position;
        startRotation = card.transform.rotation.eulerAngles;
        card.transform.localRotation = Quaternion.Euler(Vector3.zero);
        card.visualHandler.SetSortingOrder(15);
    }

    public void SetCollState(bool enable)
    {
        coll.enabled = enable;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        temp = mousePos;
        temp.z = 0;
        card.transform.position = temp;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject goHit = eventData.pointerCurrentRaycast.gameObject;
        if (goHit == null)
        {
            events.OnHandCardDroppedNowhere.Raise(card, card);
        }

        SetCollState(true);
    }

    public void MoveBackToPlace()
    {
        card.transform.position = startPos;
        card.transform.localRotation = Quaternion.Euler(startRotation);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnDrop(PointerEventData eventData)
    {
        //CardInteractionHandler attackingCardInteractionHandler = eventData.pointerDrag.GetComponent<CardInteractionHandler>();
        //if (attackingCardInteractionHandler != null)
        //{
        //    if (card.cardOwner == CardOwner.Enemy)
        //    {
        //        events.OnCardDropOnCard.Raise(card, attackingCardInteractionHandler.card);
        //    }
        //    else
        //    {
        //        attackingCardInteractionHandler.MoveBackToPlace();
        //    }
        //}
    }

    public IEnumerator MoveCardToPositionOverTime(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = card.transform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            card.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);
            elapsed += Time.deltaTime;
            yield return null;
        }

        card.transform.position = targetPosition; // Ensure final position is set accurately
    }

    public IEnumerator MoveCardByAmountOverTime(float moveDistance, float duration)
    {
        Vector3 targetPosition = transform.position + new Vector3(0, moveDistance, 0);

        yield return StartCoroutine(MoveCardToPositionOverTime(targetPosition, duration));
    }

    public IEnumerator TransformCardUniformly(Vector3? targetPosition, Vector3? targetScale, Vector3? targetEulerAngles, float duration)
    {
        Vector3 startPosition = card.transform.position;
        Vector3 startScale = card.transform.localScale;
        Quaternion startRotation = card.transform.rotation;

        Vector3 endPosition = targetPosition ?? startPosition;
        Vector3 endScale = targetScale ?? startScale;
        Quaternion endRotation = targetEulerAngles.HasValue ? Quaternion.Euler(targetEulerAngles.Value) : startRotation;

        float elapsed = 0;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;

            if (targetPosition.HasValue)
            {
                card.transform.position = Vector3.Lerp(startPosition, endPosition, progress);
            }
            if (targetScale.HasValue)
            {
                card.transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            }
            if (targetEulerAngles.HasValue)
            {
                card.transform.rotation = Quaternion.Lerp(startRotation, endRotation, progress);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final values are set accurately
        if (targetPosition.HasValue)
        {
            card.transform.position = endPosition;
        }
        if (targetScale.HasValue)
        {
            card.transform.localScale = endScale;
        }
        if (targetEulerAngles.HasValue)
        {
            card.transform.rotation = endRotation;
        }
    }
}
