using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMovementHandler : MonoBehaviour
{

    [FoldoutGroup("Components")]
    public Card card;
    [FoldoutGroup("Components")]
    [SerializeField] private Collider2D coll;

    [SerializeField]
    private CardData cardData;

    [FoldoutGroup("Default Transforms")]
    public Transform placeHolder;
    [FoldoutGroup("Default Transforms")]
    public Vector3 defaultPos;
    [FoldoutGroup("Default Transforms")]
    public Vector3 defaultScale = Vector3.one;
    [FoldoutGroup("Default Transforms")]
    public Vector3 defaultRotation;


    [SerializeField] private float highlightHeightBoostAmount;
    public bool isHighlighted;

    public Coroutine moveRoutine;

    #region Movement Routines

    public void Initialize()
    {
        SetNewDefaultLocation(card.transform.position, card.transform.localScale, card.transform.eulerAngles);
    }

    public void Highlight()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        

        isHighlighted = true;
        card.visualHandler.SetSortingLayer(GameConstants.TOP_PLAYER_CARD_LAYER);
        SetNewDefaultLocation(null, null, null);

        Vector3 temp = defaultPos;
        temp.y += highlightHeightBoostAmount;
        card.visualHandler.transform.SetPositionAndRotation(temp, Quaternion.Euler(Vector3.zero));
        card.visualHandler.transform.localScale = Vector3.one * 1.2f;
    }

    public void Dehighlight()
    {
        card.movement.isHighlighted = false;

        card.visualHandler.SetSortingLayer(GameConstants.PLAYER_CARD_LAYER);
        StartCoroutine(card.movement.TransformVisualUniformlyToPlaceholder(cardData.DehiglightSpeed, cardData.DehighlightCurve));
    }

    [Button]
    public void SnapCardToVisual()
    {
        Vector3 visualPos = card.visualHandler.transform.position;
        Quaternion visualRot = card.visualHandler.transform.rotation;
        Vector3 visualScale = card.visualHandler.transform.localScale;

        card.transform.position = visualPos;
        card.transform.rotation = visualRot;
        card.transform.localScale = visualScale;
        
        card.visualHandler.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        card.visualHandler.transform.localScale = Vector3.one;
    }

    public Vector2 GetClosestCollPosToOtherCard(Vector2 otherCardPos)
    {
        return coll.ClosestPoint(otherCardPos);
    }

    public void SetNewDefaultLocation(Vector3? position, Vector3? scale, Vector3? rotation)
    {
        defaultPos = position != null ? (Vector3)position : transform.position;
        defaultScale = scale != null ? (Vector3)scale : transform.localScale;
        defaultRotation = rotation != null ? (Vector3)rotation : transform.eulerAngles;
    }

    public void RestartTransformToDefault()
    {
        card.transform.localScale = defaultScale;
        card.transform.position = defaultPos;
        card.transform.rotation = Quaternion.Euler(defaultRotation);
    }

    public IEnumerator TransformCardUniformlyToPlaceholder(float speed, AnimationCurve curve)
    {
        yield return StartCoroutine(TransformCardUniformly(card.transform, placeHolder.position, placeHolder.localScale, placeHolder.eulerAngles, speed, curve));
    }

    public IEnumerator TransformCardUniformlyToHoveredPlaceholder(float speed, AnimationCurve curve)
    {
        Vector3 temp = placeHolder.position;
        temp.y += highlightHeightBoostAmount;

        yield return StartCoroutine(TransformCardUniformly(card.transform, temp, Vector3.one * 1.2f, Vector3.zero, speed, curve));
    }

    public IEnumerator TransformVisualUniformlyToPlaceholder(float speed, AnimationCurve curve)
    {
        yield return StartCoroutine(TransformCardUniformly(card.visualHandler.transform, placeHolder.position, placeHolder.localScale, placeHolder.eulerAngles, speed, curve));
    }

    public IEnumerator TransformCardUniformly(Transform targetTransform, Vector3? targetPosition, Vector3? targetScale, Vector3? targetEulerAngles, float speed, AnimationCurve curve)
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(TransformCardUniformlyRoutine(targetTransform, targetPosition, targetScale, targetEulerAngles, speed, curve));
        yield return moveRoutine;
    }

    private IEnumerator TransformCardUniformlyRoutine(Transform targetTransform, Vector3? targetPosition, Vector3? targetScale, Vector3? targetEulerAngles, float speed, AnimationCurve curve)
    {
        Vector3 startPosition = targetTransform.position;
        Vector3 startScale = targetTransform.localScale;
        Quaternion startRotation = targetTransform.rotation;

        Vector3 endPosition = targetPosition ?? startPosition;
        Vector3 endScale = targetScale ?? startScale;
        Quaternion endRotation = targetEulerAngles.HasValue ? Quaternion.Euler(targetEulerAngles.Value) : startRotation;

        // Calculate positional distance
        float positionDistance = targetPosition.HasValue ? Vector3.Distance(startPosition, endPosition) : 0;

        // Calculate duration based on positional distance and speed
        float duration = positionDistance / speed; // Duration is derived from position change speed

        float elapsed = 0;

        while (elapsed < duration)
        {
            float progress = curve.Evaluate(elapsed / duration);

            // Apply the same progress to all transformations
            targetTransform.position = Vector3.Lerp(startPosition, endPosition, progress);
            targetTransform.localScale = Vector3.Lerp(startScale, endScale, progress);
            targetTransform.rotation = Quaternion.Lerp(startRotation, endRotation, progress);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final values are set accurately
        if (targetPosition.HasValue) targetTransform.position = endPosition;
        if (targetScale.HasValue) targetTransform.localScale = endScale;
        if (targetEulerAngles.HasValue) targetTransform.rotation = endRotation;
    }

    #endregion
}
