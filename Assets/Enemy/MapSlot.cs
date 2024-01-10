using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public MapSlotState slotState;
    public int index;
    [SerializeField] private Collider2D coll;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private SpriteRenderer xSr;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color doneColor;
    [SerializeField] private AllEvents events;
    private bool isBlinking;
    [SerializeField] private float blinkingDuration;

    [SerializeField] private List<Sprite> xAnimationSprites;
    [SerializeField] private float xAnimationIntervals = 0.1f;

    private void Awake()
    {
        sr.color = defaultColor;

    }

    public void SetNewCard(Card card)
    {
        StartCoroutine(card.interactionHandler.MoveCardToPositionOverTime(transform.position, 1));
    }

    private void StartBlinking()
    {
        isBlinking = true;
        StartCoroutine(BlinkingRoutine());
    }

    private IEnumerator BlinkingRoutine()
    {
        while (isBlinking)
        {
            yield return StartCoroutine(LerpBetweenColors(defaultColor, hoverColor));
            yield return StartCoroutine(LerpBetweenColors(hoverColor, defaultColor));
        }
    }

    private void StopBlinking()
    {
        isBlinking = false;
    }


    private IEnumerator LerpBetweenColors(Color start, Color end)
    {
        float time = 0;
        while (time < blinkingDuration && isBlinking)
        {
            sr.color = Color.Lerp(start, end, time / blinkingDuration);
            time += Time.deltaTime;
            yield return null;
        }
    }

    public void StopLerping()
    {
        isBlinking = false;
    }

    public IEnumerator SetSlotState(MapSlotState newState)
    {
        slotState = newState;
        switch (slotState)
        {
            case MapSlotState.Idle:
                coll.enabled = false;
                StopBlinking();
                sr.color = defaultColor;
                break;
            case MapSlotState.Choosable:
                StartBlinking();
                coll.enabled = true;
                break;
            case MapSlotState.Occupied:
                coll.enabled = false;
                StopBlinking();
                sr.color = defaultColor;
                break;
            case MapSlotState.Done:
                coll.enabled = false;
                yield return StartCoroutine(AnimateX(xAnimationSprites, xAnimationIntervals));
                events.OnFinishedXAnimation.Raise(this, index);
                break;
        }

        yield return null;
    }

    private IEnumerator AnimateX(List<Sprite> sprites, float changeInterval)
    {
        foreach (Sprite sprite in sprites)
        {
            xSr.sprite = sprite;
            yield return new WaitForSeconds(changeInterval);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        events.OnMapSlotClicked.Raise(this, index);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //sr.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //sr.color = defaultColor;
    }
}

public enum MapSlotState
{
    Idle,
    Choosable,
    Occupied,
    Done
}
