using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerDeck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool canDraw;
    private bool hasDrawn;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private AllEvents events;
    [SerializeField] private float blinkingDuration = 0.75f;
    private void Awake()
    {
        sr.color = defaultColor;
    }

    public void EnableDraw()
    {
        canDraw = true;
    }

    public IEnumerator DrawAfterBattleRoutine()
    {
        canDraw = true;
        hasDrawn = false;
        while(!hasDrawn)
        {
            yield return StartCoroutine(LerpBetweenColors(defaultColor, hoverColor));
            yield return StartCoroutine(LerpBetweenColors(hoverColor, defaultColor));
        }
    }

    private IEnumerator LerpBetweenColors(Color start, Color end)
    {
        float time = 0;
        while (time < blinkingDuration && !hasDrawn)
        {
            sr.color = Color.Lerp(start, end, time / blinkingDuration);
            time += Time.deltaTime;
            yield return null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canDraw) return;
        gameManager.SpawnPlayerCard();

        hasDrawn = true; 
        canDraw = false;
        sr.color = defaultColor;
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!canDraw) return;
        sr.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!canDraw) return;
        sr.color = defaultColor;
    }



}