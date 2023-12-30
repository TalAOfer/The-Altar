using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class CardInteractionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Card card;
    [SerializeField] private Collider2D coll;
    public Vector3 defaultPos;
    public Vector3 defaultScale;
    public Vector3 defaultRotation;
    private Vector3 temp;

    [SerializeField] Color defaultColor;
    [SerializeField] Color hoverColor;
    [SerializeField] AllEvents events;
    [SerializeField] private DragManager dragManager;

    public void Initialize()
    {
        SetNewDefaultLocation(card.transform.position, card.transform.localScale, card.transform.eulerAngles);
    }

    public void SetNewDefaultLocation(Vector3 position, Vector3 scale, Vector3 rotation)
    {
        defaultPos = position;
        defaultScale = scale;
        defaultRotation = rotation;
    }

    private void RestartTransformToDefault()
    {
        card.transform.localScale = defaultScale;
        card.transform.position = defaultPos;
        card.transform.rotation = Quaternion.Euler(defaultRotation);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        card.visualHandler.SetCardBGColor(hoverColor);

        if (card.cardOwner != CardOwner.Player) return;

        if (!dragManager.isCardDragged)
        {
            card.visualHandler.SetCardBGColor(hoverColor);
            card.visualHandler.SetSortingOrder(10);
            card.transform.position = new Vector3(defaultPos.x, defaultPos.y + 0.5f, defaultPos.z);
            card.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        else
        {
            events.OnDraggedCardHoveredOverHandCard.Raise(card, dragManager.draggedCard);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        card.visualHandler.SetCardBGColor(defaultColor);

        if (card.cardOwner != CardOwner.Player) return;
        if (dragManager.isCardDragged) return;

        card.visualHandler.SetSortingOrder(card.index);
        RestartTransformToDefault();

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (card.cardOwner != CardOwner.Player) return;

        SetCollState(false);
        dragManager.SetDraggedCard(card);

        //To take it out of hand
        events.OnHandCardStartDrag.Raise(this, card);
        card.visualHandler.SetSortingOrder(15);
    }

    public void SetCollState(bool enable)
    {
        coll.enabled = enable;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (card.cardOwner != CardOwner.Player) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        temp = mousePos;
        temp.z = 0;
        card.transform.position = temp;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (card.cardOwner != CardOwner.Player) return;

        dragManager.SetDraggedCard(null);
        SetCollState(true);

        GameObject goHit = eventData.pointerCurrentRaycast.gameObject;
        CardInteractionHandler cardIhThatItDroppedOn = goHit.GetComponent<CardInteractionHandler>();
        Card droppedCard = cardIhThatItDroppedOn != null ? cardIhThatItDroppedOn.card : null;

        if (droppedCard != null && droppedCard.cardOwner == CardOwner.Enemy)
        {
            events.OnCardDropOnCard.Raise(card, droppedCard);
        }

        else
        {
            events.OnHandCardDroppedNowhere.Raise(card, card);
        }

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
