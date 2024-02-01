using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum BattleInteractionState
{
    Setup,
    Idle,
    Battle
}

public class BattleInteractionHandler : CardInteractionBase
{
    [SerializeField] private CurrentGameState gameState;
    [SerializeField] private float hoverHeightBoostAmount;
    private bool isArrowActive;


    public BattleInteractionState state;


    private void Awake()
    {
        SetState(BattleInteractionState.Setup);
    }
    public void SetState(BattleInteractionState newState)
    {
        state = newState;
    }

    #region Actions

    private void SelectCard(Card card)
    {
        selectedCard = card;
        if (!card.movement.isHighlighted) Highlight(card);
        card.visualHandler.ToggleOutline(true, Color.white);
    }
    private void DeselectCurrentCard()
    {
        if (selectedCard == null) return;

        selectedCard.visualHandler.ToggleOutline(false, Color.white);

        if (selectedCard.movement.isHighlighted) Dehighlight(selectedCard);

        selectedCard = null;
    }

    private void Highlight(Card card)
    {
        card.movement.Highlight();
    }

    private void Dehighlight(Card card)
    {
        card.movement.Dehighlight();
    }

    private void DisableArrow()
    {
        isArrowActive = false;
        events.DisableBezierArrow.Raise();
    }

    private void EnableArrow()
    {
        isArrowActive = true;
        events.EnableBezierArrow.Raise(this, selectedCard);
    }

    private void Attack(Card attackingCard, Card attackedCard)
    {
        events.Attack.Raise(attackingCard, attackedCard);
        events.HideTooltip.Raise();
        selectedCard.visualHandler.ToggleOutline(false, Color.white);
        selectedCard = null;

        DisableArrow();
    }

    #endregion

    #region Event Handlers
    public void EnableInteraction()
    {
        SetState(BattleInteractionState.Idle);
    }

    protected override void HandlePointerClick(Card card, PointerEventData eventData)
    {
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;

        bool isThereASelectedCard = selectedCard != null;
        bool isThisCardAPlayerCard = card.cardOwner == CardOwner.Player;

        if (isThereASelectedCard)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                DeselectCurrentCard();
                DisableArrow();
                return;
            }

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
                Attack(selectedCard, card);
            }
        }

        else
        {
            if (isThisCardAPlayerCard) SelectCard(card);
        }
    }

    protected override void HandlePointerEnter(Card card, PointerEventData eventData)
    {
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;


        bool isThereASelectedCard = selectedCard != null;
        bool isThisCardAPlayerCard = card.cardOwner == CardOwner.Player;


        card.events.ShowTooltip.Raise(this, card);

        if (!isThereASelectedCard)
        {
            card.events.ShowTooltip.Raise(this, card);
        }
        else
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
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;


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
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;


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
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;


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
            Attack(card, droppedCard);
        }
    }


    public void HandleTriggerEnter(Component sender, object data)
    {
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;

        if (selectedCard == null) return;
        if (!isArrowActive) return;

        DeselectCurrentCard();
        DisableArrow();
    }

    public void HandleTriggerExit(Component sender, object data)
    {
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;

        if (selectedCard == null) return;
        if (isArrowActive) return;
        EnableArrow();
    }

    #endregion

    #region Helpers


    #endregion
}
