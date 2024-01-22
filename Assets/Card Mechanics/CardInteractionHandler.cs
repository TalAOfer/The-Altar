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
    public Transform placeHolder;
    [FoldoutGroup("Default Transforms")]
    public Vector3 defaultPos;
    [FoldoutGroup("Default Transforms")]
    public Vector3 defaultScale = Vector3.one;
    [FoldoutGroup("Default Transforms")]
    public Vector3 defaultRotation;

    public Coroutine moveRoutine;


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

    public Vector2 GetClosestCollPosToOtherCard(Vector2 otherCardPos)
    {
        return coll.ClosestPoint(otherCardPos);
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

    public IEnumerator TransformCardUniformlyToPlaceholder(float duration)
    {
       yield return StartCoroutine(TransformCardUniformly(card.transform, placeHolder.position, placeHolder.localScale, placeHolder.eulerAngles, duration, null));
    }

    public IEnumerator TransformVisualUniformlyToPlaceholder(float duration)
    {
        yield return StartCoroutine(TransformCardUniformly(card.visualHandler.transform, placeHolder.position, placeHolder.localScale, placeHolder.eulerAngles, duration, null));
    }

    public IEnumerator TransformCardUniformly(Transform targetTransform, Vector3? targetPosition, Vector3? targetScale, Vector3? targetEulerAngles, float duration, AnimationCurve curve)
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(TransformCardUniformlyRoutine(targetTransform, targetPosition, targetScale, targetEulerAngles, duration, curve));
        yield return moveRoutine;
    }

    public IEnumerator TransformCardBySpeed(Transform targetTransform, Vector3? targetPosition, Vector3? targetScale, Vector3? targetEulerAngles, float speed)
    {
        // Calculate distances to determine the duration
        float positionDistance = targetPosition.HasValue ? Vector3.Distance(targetTransform.position, targetPosition.Value) : 0;
        float scaleDistance = targetScale.HasValue ? Vector3.Distance(targetTransform.localScale, targetScale.Value) : 0;
        float rotationDistance = targetEulerAngles.HasValue ? Quaternion.Angle(targetTransform.rotation, Quaternion.Euler(targetEulerAngles.Value)) : 0;

        // Calculate the longest duration based on speed
        float maxDistance = Mathf.Max(positionDistance, scaleDistance, rotationDistance);
        float duration = maxDistance / speed;

        // Now call TransformCardUniformly with the calculated duration
        yield return StartCoroutine(TransformCardUniformly(targetTransform, targetPosition, targetScale, targetEulerAngles, duration, AnimationCurve.Linear(0, 0, 1, 1)));
    }

    private IEnumerator TransformCardUniformlyRoutine(Transform targetTransform, Vector3? targetPosition, Vector3? targetScale, Vector3? targetEulerAngles, float duration, AnimationCurve curve)
    {
        Vector3 startPosition = targetTransform.position;
        Vector3 startScale = targetTransform.localScale;
        Quaternion startRotation = targetTransform.rotation;

        Vector3 endPosition = targetPosition ?? startPosition;
        Vector3 endScale = targetScale ?? startScale;
        Quaternion endRotation = targetEulerAngles.HasValue ? Quaternion.Euler(targetEulerAngles.Value) : startRotation;

        //Set to linear curve if no curve was fed
        curve ??= AnimationCurve.Linear(0, 0, 1, 1);

        float elapsed = 0;

        while (elapsed < duration)
        {
            float progress = curve.Evaluate(elapsed / duration);

            if (targetPosition.HasValue)
            {
                targetTransform.position = Vector3.Lerp(startPosition, endPosition, progress);
            }
            if (targetScale.HasValue)
            {
                targetTransform.localScale = Vector3.Lerp(startScale, endScale, progress);
            }
            if (targetEulerAngles.HasValue)
            {
                targetTransform.rotation = Quaternion.Lerp(startRotation, endRotation, progress);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final values are set accurately
        if (targetPosition.HasValue)
        {
            targetTransform.position = endPosition;
        }
        if (targetScale.HasValue)
        {
            targetTransform.localScale = endScale;
        }
        if (targetEulerAngles.HasValue)
        {
            targetTransform.rotation = endRotation;
        }

        moveRoutine = null;
    }

    #endregion
}