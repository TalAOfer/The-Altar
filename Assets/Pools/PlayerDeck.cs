using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerDeck : Deck, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool canDraw;
    private bool hasDrawn;
    [SerializeField] private SpriteRenderer sr;

    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private HandManager handManager;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color hoverColor;
    [SerializeField] private float blinkingDuration = 0.75f;

    private void Start()
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
        while (!hasDrawn)
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
        DrawPlayerCard();

        hasDrawn = true;
        canDraw = false;
        sr.color = defaultColor;
    }

    public void DrawPlayerCard()
    {
        //TODO: who's in charge of indexes?
        Card card = SpawnCard(DrawCard(), 0, GameConstants.HAND_LAYER);
        playerManager.activeCards.Add(card);
        handManager.AddCardToHand(card);

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

    public void OnSpawnCardToHand(Component sender, object data)
    {
        ActiveEffect askerEffect = (ActiveEffect)sender;
        CardBlueprint cardToSpawn = (CardBlueprint)data;

        //TODO: think about who decides on the indexes
        Card card = SpawnCard(cardToSpawn, 0, GameConstants.HAND_LAYER);

        playerManager.activeCards.Add(card);
        handManager.AddCardToHand(card);

        StartCoroutine(askerEffect.HandleResponse(this, null));
    }

}