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
    [SerializeField] private Collider2D coll;

    [FoldoutGroup("Dependencies")]
    [SerializeField] private DragManager dragManager;
    [FoldoutGroup("Dependencies")]
    [SerializeField] private CurrentGameState gameState;

    [FoldoutGroup("Default Transforms")]
    public Vector3 defaultPos;
    [FoldoutGroup("Default Transforms")]
    public Vector3 defaultScale;
    [FoldoutGroup("Default Transforms")]
    public Vector3 defaultRotation;
    private Vector3 temp;

    [SerializeField] private float hoverHeightBoostAmount;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color hoverColor;

    public void Initialize()
    {
        SetNewDefaultLocation(card.transform.position, card.transform.localScale, card.transform.eulerAngles);
    }

    public void SetCollState(bool enable)
    {
        coll.enabled = enable;
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

        if (ShouldHoverTriggerTooltip) card.events.ShowTooltip.Raise(this, card);

        if (ShouldHoverBoostHeight)
        {
            card.visualHandler.SetSortingOrder(10);
            card.transform.position = new Vector3(defaultPos.x, defaultPos.y + hoverHeightBoostAmount, defaultPos.z);
            card.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        if (ShouldHoverTriggerHandPlaceSwitch) card.events.OnDraggedCardHoveredOverHandCard.Raise(card, dragManager.draggedCard);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        card.visualHandler.SetCardBGColor(defaultColor);

        if (ShouldHoverTriggerTooltip) card.events.HideTooltip.Raise();

        if (ShouldHoverBoostHeight)
        {
            card.visualHandler.SetSortingOrder(card.index);
            RestartTransformToDefault();
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!CanDrag) return;

        card.events.HideTooltip.Raise();
        SetCollState(false);
        dragManager.SetDraggedCard(card);

        //To take it out of hand
        card.events.OnHandCardStartDrag.Raise(this, card);
        card.visualHandler.SetSortingOrder(15);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!CanDrag) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        temp = mousePos;
        temp.z = 0;
        card.transform.position = temp;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanDrag) return;

        dragManager.SetDraggedCard(null);
        SetCollState(true);

        GameObject goHit = eventData.pointerCurrentRaycast.gameObject;
        CardInteractionHandler cardIhThatItDroppedOn = goHit.GetComponent<CardInteractionHandler>();
        Card droppedCard = cardIhThatItDroppedOn != null ? cardIhThatItDroppedOn.card : null;

        if (droppedCard != null && droppedCard.cardOwner == CardOwner.Enemy)
        {
            card.events.OnCardDropOnCard.Raise(card, droppedCard);
        }

        else
        {
            card.events.OnHandCardDroppedNowhere.Raise(card, card);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CanClick)
        {
            card.events.OnCardClicked.Raise(this, card);
        }
    }

    #region Movement Routines

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

    public IEnumerator TransformCardUniformlyToDefault(float duration)
    {
        yield return StartCoroutine(TransformCardUniformly(defaultPos, defaultScale, defaultRotation, duration));
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

    #endregion

    #region Helpers

    public void OnGameStateChange(Component sender, object data)
    {
        GameState newGameState = (GameState)data;
        switch (newGameState)
        {
            case GameState.Idle:
                coll.enabled = true;
                break;
            case GameState.BattleFormation:
                if (card.cardState != CardState.Battle) coll.enabled = false;
                break;
        }
    }

    private bool ShouldHoverTriggerTooltip => gameState.currentState is GameState.ChooseNewBlueprints || 
        (!dragManager.isCardDragged && gameState.currentState is GameState.Idle
         || (gameState.currentState is GameState.BattleFormation && card.cardState is CardState.Battle));

    private bool ShouldHoverBoostHeight => (gameState.currentState is GameState.Idle && !dragManager.isCardDragged && card.cardOwner is CardOwner.Player);

    private bool ShouldHoverTriggerHandPlaceSwitch => (gameState.currentState is GameState.Idle && dragManager.isCardDragged && card.cardOwner is CardOwner.Player);

    private bool CanDrag => (gameState.currentState is GameState.Idle &&  card.cardOwner is CardOwner.Player);

    private bool CanClick => gameState.currentState == GameState.ChooseNewBlueprints || (gameState.currentState is GameState.SelectPlayerCard && card.cardOwner is CardOwner.Player
        && card.cardState is CardState.Selectable or CardState.Selected);

    #endregion
}