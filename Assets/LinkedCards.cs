using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class LinkedCards : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private RunData runData;
    [SerializeField] private AllEvents events;
    [SerializeField] private Transform enemyCardPos;
    [SerializeField] private Transform playerCardPos;

    [SerializeField] private List<GameObject> selectionArrows;
    [SerializeField] private Collider2D linkColl;
    public bool IsPointerOverLinkCollider { get; set; }
    public bool wasChosen { get; set; }

    public Card playerCard { get; private set; }
    public Card enemyCard { get; private set; }
    public int index { get; private set; }
    private RoomBlueprint roomBlueprint;

    public void Init(RoomBlueprint roomBlueprint, int index)
    {
        wasChosen = false;
        this.roomBlueprint = roomBlueprint;
        this.index = index;
        SpawnCards();
    }

    public void OutlineCards()
    {
        playerCard.visualHandler.ToggleOutline(true, Color.black);
        enemyCard.visualHandler.ToggleOutline(true, Color.black);
        playerCard.visualHandler.Animate("Jiggle");
        enemyCard.visualHandler.Animate("Jiggle");
    }

    public void DeOutlineCards()
    {
        playerCard.visualHandler.ToggleOutline(false, Color.black);
        enemyCard.visualHandler.ToggleOutline(false, Color.black);
    }

    public void ToggleSelectionArrows(bool enable)
    {
        foreach (var item in selectionArrows)
        {
            item.SetActive(enable);
        }
    }

    public void SpawnCards()
    {
        CardBlueprint playerDrawnBlueprint = runData.playerPool.GetRandomCardByPoints(roomBlueprint.playerDrawMinMax.x, roomBlueprint.playerDrawMinMax.y);
        playerCard = SpawnCard(playerDrawnBlueprint, GameConstants.TOP_BATTLE_LAYER, runData.playerCodex);
        playerCard.transform.localPosition = playerCardPos.localPosition;
        playerCard.index = index;

        CardBlueprint enemyDrawnBlueprint = runData.enemyPool.GetRandomCardByPoints(roomBlueprint.enemyDrawMinMax.x, roomBlueprint.enemyDrawMinMax.y);
        enemyCard = SpawnCard(enemyDrawnBlueprint, GameConstants.TOP_BATTLE_LAYER, runData.enemyCodex);
        enemyCard.transform.localPosition = enemyCardPos.localPosition;
        enemyCard.index = index;
    }


    public Card SpawnCard(CardBlueprint cardBlueprint, string sortingLayerName, BlueprintPoolInstance codex)
    {
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, transform);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(codex, cardBlueprint, sortingLayerName, CardInteractionType.Selection);

        return card;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        events.OnLinkPointerEnter.Raise(this, eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        events.OnLinkPointerExit.Raise(this, eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        events.OnLinkClick.Raise(this, eventData);
    }
}