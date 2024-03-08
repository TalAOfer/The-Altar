using DG.Tweening;
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

    [SerializeField] private float highlightHeightBoostAmount;
    public bool isHighlighted;

    public Coroutine moveRoutine;
    private Sequence _activeSequence;

    #region Movement Routines

    public void Highlight()
    {
        if (isHighlighted) return;
        if (moveRoutine != null) StopCoroutine(moveRoutine);

        isHighlighted = true;
        card.visualHandler.SetSortingLayer(GameConstants.TOP_PLAYER_CARD_LAYER);

        Vector3 temp = transform.position;
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

    public IEnumerator TransformCardUniformlyToPlaceholder(float speed, Ease ease)
    {
        card.cardState = CardState.Default;
        yield return MoveCard(card.transform, placeHolder.position, placeHolder.localScale, placeHolder.eulerAngles, speed, ease);
    }

    public IEnumerator TransformCardUniformlyToHoveredPlaceholder(float speed, Ease ease)
    {
        Vector3 temp = placeHolder.position;
        temp.y += highlightHeightBoostAmount;

        yield return MoveCard(card.transform, temp, Vector3.one * 1.2f, Vector3.zero, speed, ease);
    }

    public IEnumerator TransformVisualUniformlyToPlaceholder(float speed, Ease ease)
    {
        yield return MoveCard(card.visualHandler.transform, placeHolder.position, placeHolder.localScale, placeHolder.eulerAngles, speed, ease);
    }

    public IEnumerator MoveCard(Transform targetTransform, Vector3? targetPosition, Vector3? targetScale, Vector3? targetEulerAngles, float speed, Ease ease)
    {
        if (_activeSequence != null && _activeSequence.IsActive())
        {
            _activeSequence.Kill();
        }

        _activeSequence = DOTween.Sequence();

        Vector3 startPos = targetTransform.position;
        Vector3 endPos = (Vector3)targetPosition;
        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / speed;

        if (targetPosition != null)
        {
            Tween tween = targetTransform.DOMove((Vector3)targetPosition, duration).SetEase(ease);
            _activeSequence.Join(tween);
        }

        if (targetScale != null)
        {
            Tween tween = targetTransform.DOScale((Vector3)targetScale, duration).SetEase(ease);
            _activeSequence.Join(tween);
        }

        if (targetEulerAngles != null)
        {
            Tween tween = targetTransform.DORotate((Vector3)targetEulerAngles, duration).SetEase(ease);
            _activeSequence.Join(tween);
        }

        _activeSequence.OnComplete(() => Debug.Log("Sequence completed"));
        yield return _activeSequence.WaitForCompletion();
        Debug.Log("Coroutine awaiting sequence completion is now continuing");

    }


    #endregion
}
