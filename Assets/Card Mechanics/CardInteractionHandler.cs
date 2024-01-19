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
    public Vector3 defaultScale = Vector3.one;
    [FoldoutGroup("Default Transforms")]
    public Vector3 defaultRotation;

    [SerializeField] private float hoverHeightBoostAmount;
    public bool isHighlighted;

    public void Initialize()
    {
        SetNewDefaultLocation(card.transform.position, card.transform.localScale, card.transform.eulerAngles);
    }

    public void SetCollState(bool enable)
    {
        coll.enabled = enable;
    }

    public void SetNewDefaultLocation(Vector3? position, Vector3? scale, Vector3? rotation)
    {
        defaultPos = position != null ?  (Vector3)position : transform.position;
        defaultScale = scale != null ? (Vector3)scale : transform.localScale;
        defaultRotation = rotation != null ? (Vector3) rotation : transform.eulerAngles;
    }

    public void RestartTransformToDefault()
    {
        card.transform.localScale = defaultScale;
        card.transform.position = defaultPos;
        card.transform.rotation = Quaternion.Euler(defaultRotation);
    }

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



    #endregion
}