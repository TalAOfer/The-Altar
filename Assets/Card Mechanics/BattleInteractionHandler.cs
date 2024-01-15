using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleInteractionHandler : CardInteractionBase
{
    [SerializeField] private CurrentGameState gameState;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private float hoverHeightBoostAmount;

    protected override void HandlePointerEnter(Card card, PointerEventData eventData)
    {
        card.visualHandler.SetCardBGColor(hoverColor);

        events.ShowTooltip.Raise(this, card);

        //Boost height
        Vector3 temp = card.interactionHandler.defaultPos;
        temp.y += hoverHeightBoostAmount;
        card.transform.position = temp;
    }

    protected override void HandlePointerExit(Card card, PointerEventData eventData)
    {
        card.visualHandler.SetCardBGColor(defaultColor);

        events.HideTooltip.Raise(this, card);

        //Boost height
        card.interactionHandler.RestartTransformToDefault();
    }

    protected override void HandleBeginDrag(Card card, PointerEventData eventData)
    {
        card.events.HideTooltip.Raise();
        card.interactionHandler.SetCollState(false);

        //To take it out of hand
        events.OnHandCardStartDrag.Raise(this, card);
        card.visualHandler.SetSortingOrder(15);
        draggedCard = card;
        isDragging = true;
    }

    public override void HandleDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 temp = mousePos;
        temp.z = 0;
        draggedCard.transform.position = temp;
    }

    protected override void HandleEndDrag(Card card, PointerEventData eventData)
    {
        draggedCard = null;
        isDragging = false;

        card.interactionHandler.SetCollState(true);

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

    protected override void HandlePointerClick(Card card, PointerEventData eventData)
    {

    }

    private bool ShouldHoverTriggerTooltip(Card card) => gameState.currentState is GameState.ChooseNewBlueprints ||
    (isDragging && gameState.currentState is GameState.Idle
     || (gameState.currentState is GameState.BattleFormation && card.cardState is CardState.Battle));

    private bool ShouldHoverBoostHeight(Card card)
    {
        if (isDragging) return false;
        else return gameState.currentState is GameState.Idle && card.cardOwner is CardOwner.Player;
    }

    private bool ShouldHoverTriggerHandPlaceSwitch(Card card)
    {
        return isDragging && gameState.currentState is GameState.Idle && card.cardOwner is CardOwner.Player;
    }

    private bool CanDrag(Card card)
    {
        return (gameState.currentState is GameState.Idle && card.cardOwner is CardOwner.Player);
    }

    private bool CanClick(Card card) 
    {
        bool isCardSelectedOrSelectable = card.cardState is CardState.Selectable or CardState.Selected;
        return (gameState.currentState is GameState.SelectPlayerCard && isCardSelectedOrSelectable && card.cardOwner is CardOwner.Player);
    } 
}
