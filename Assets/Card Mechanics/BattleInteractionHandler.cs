using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleInteractionHandler : CardInteractionBase
{
    [SerializeField] private CurrentGameState gameState;
    [SerializeField] private float hoverHeightBoostAmount;
    private bool isArrowActive;
    private void SelectCard(Card card)
    {
        selectedCard = card;
        if (!card.interactionHandler.isHighlighted) Highlight(card);
        card.visualHandler.ToggleOutline(true, Color.white);
    }
    private void DeselectCurrentCard()
    {
        if (selectedCard == null) return;

        selectedCard.visualHandler.ToggleOutline(false, Color.white);

        if (selectedCard.interactionHandler.isHighlighted) Dehighlight(selectedCard);

        selectedCard = null;
    }

    private void Highlight(Card card)
    {
        card.interactionHandler.isHighlighted = true;
        card.visualHandler.SetSortingLayer(GameConstants.TOP_BATTLE_LAYER);

        card.interactionHandler.SetNewDefaultLocation(null, null, null);
        Vector3 temp = card.interactionHandler.defaultPos;
        temp.y += hoverHeightBoostAmount;
        card.visualHandler.transform.SetPositionAndRotation(temp, Quaternion.Euler(Vector3.zero));
        card.visualHandler.transform.localScale = Vector3.one * 1.2f;
    }

    private void Dehighlight(Card card)
    {
        card.interactionHandler.isHighlighted = false;

        card.visualHandler.SetSortingLayer(GameConstants.HAND_LAYER);
        card.visualHandler.transform.SetPositionAndRotation(card.interactionHandler.defaultPos, Quaternion.Euler(card.interactionHandler.defaultRotation));
        card.visualHandler.transform.localScale = card.interactionHandler.defaultScale;
    }



    protected override void HandlePointerClick(Card card, PointerEventData eventData)
    {
        bool isThereASelectedCard = selectedCard != null;
        bool isThisCardAPlayerCard = card.cardOwner == CardOwner.Player;

        if (isThereASelectedCard)
        {
            if (isThisCardAPlayerCard)
            {
                bool isThisCardTheSelectedCard = selectedCard == card;
                DeselectCurrentCard();

                if (!isThisCardTheSelectedCard)
                {
                    SelectCard(card);
                }

            }

            else
            {
                card.events.Attack.Raise(selectedCard, card);
                DeselectCurrentCard();
                DisableArrow();
            }
        }

        else
        {
            SelectCard(card);
        }
    }

    protected override void HandlePointerEnter(Card card, PointerEventData eventData)
    {
        bool isThereASelectedCard = selectedCard != null;
        bool isThisCardAPlayerCard = card.cardOwner == CardOwner.Player;

        
        card.events.ShowTooltip.Raise(this, card);

        if (!isThereASelectedCard)
        {
            card.events.ShowTooltip.Raise(this, card);
        } else
        {
            if (!isThisCardAPlayerCard)
            {
                card.events.ShowTooltip.Raise(this, card);
            }
        }

        if (isThisCardAPlayerCard)
        {
            Highlight(card);
        }
    }

    protected override void HandlePointerExit(Card card, PointerEventData eventData)
    {
        card.events.HideTooltip.Raise(this, card);

        bool isThisCardAPlayerCard = card.cardOwner == CardOwner.Player;
        bool isThisCardSelected = selectedCard == card;

        if (isThisCardAPlayerCard && !isThisCardSelected)
        {
            Dehighlight(card);
        }
    }

    protected override void HandleBeginDrag(Card card, PointerEventData eventData)
    {
        bool isThereASelectedCard = selectedCard != null;
        bool isThisCardAPlayerCard = card.cardOwner == CardOwner.Player;

        if (!isThisCardAPlayerCard) return;

        if (isThereASelectedCard)
        {
            if (isThisCardAPlayerCard)
            {
                bool isThisCardTheSelectedCard = selectedCard == card;
                DeselectCurrentCard();

                if (!isThisCardTheSelectedCard)
                {
                    SelectCard(card);
                }
            }
        }

        else
        {
            SelectCard(card);
        }
    }

    protected override void HandleEndDrag(Card card, PointerEventData eventData)
    {
        bool isThisCardAPlayerCard = card.cardOwner == CardOwner.Player;
        if (!isThisCardAPlayerCard) return;

        GameObject goHit = eventData.pointerCurrentRaycast.gameObject;
        Card droppedCard = null;
        if (goHit != null)
        {
            CardInteractionHandler cardIhThatItDroppedOn = goHit.GetComponent<CardInteractionHandler>();
            droppedCard = cardIhThatItDroppedOn != null ? cardIhThatItDroppedOn.card : null;
        }

        if (droppedCard == null)
        {
            DeselectCurrentCard();
            DisableArrow();
            return;
        }

        if (droppedCard.cardOwner == CardOwner.Enemy)
        {
            card.events.Attack.Raise(card, droppedCard);
            DeselectCurrentCard();
            DisableArrow();
        }
    }


    public void HandleTriggerEnter(Component sender, object data)
    {
        if (selectedCard == null) return;
        if (!isArrowActive) return;

        DeselectCurrentCard();
        DisableArrow();
    }

    public void HandleTriggerExit(Component sender, object data)
    {
        if (selectedCard == null) return; 
        if (isArrowActive) return;
        isArrowActive = true;
        events.EnableBezierArrow.Raise(this, selectedCard);
    }

    private void DisableArrow()
    {
        isArrowActive = false;
        events.DisableBezierArrow.Raise();
    }

    #region Helpers


    #endregion
}
