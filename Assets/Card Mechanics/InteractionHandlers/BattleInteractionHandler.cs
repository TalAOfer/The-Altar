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
    [SerializeField] private RoomData roomData;
    private bool isArrowActive;
    private bool canClick = true;

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
        card.ChangeCardState(CardState.Selected);
        if (!card.movement.isHighlighted) Highlight(card);
        card.visualHandler.ToggleOutline(true);
    }
    private void DeselectCurrentCard()
    {
        if (selectedCard == null) return;

        selectedCard.visualHandler.ToggleOutline(false);
        selectedCard.ChangeCardState(CardState.Default);
        events.HideTooltip.Raise();

        if (selectedCard.movement.isHighlighted) Dehighlight(selectedCard);

        selectedCard = null;
    }

    public virtual void Highlight(Card card)
    {
        card.movement.Highlight();
    }

    public virtual void Dehighlight(Card card)
    {
        card.movement.Dehighlight();
    }

    public void ResetInteractions()
    {
        DeselectCurrentCard();
        DisableArrow();
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
        selectedCard.visualHandler.ToggleOutline(false);
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
        if (!canClick) return;
        if (card.cardState != CardState.Default) return;
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;

        bool isThereASelectedCard = selectedCard != null;
        bool isThisCardAPlayerCard = card.Affinity == Affinity.Player;

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
        if (card.cardState != CardState.Default) return;

        bool isThisCardAPlayerCard = card.Affinity == Affinity.Player;

        events.ShowTooltip.Raise(this, card);

        if (isThisCardAPlayerCard)
        {
            Highlight(card);
        } 
        
        else
        {
            card.visualHandler.Animate("Jiggle");
        }

        events.ShowTooltip.Raise(this, card);
    }

    protected override void HandlePointerExit(Card card, PointerEventData eventData)
    {
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;
        if (card.cardState != CardState.Default) return;


        events.HideTooltip.Raise(this, card);

        bool isThisCardAPlayerCard = card.Affinity == Affinity.Player;
        bool isThisCardSelected = selectedCard == card;

        if (isThisCardAPlayerCard && !isThisCardSelected)
        {
            Dehighlight(card);
        }
    }

    protected override void HandleBeginDrag(Card card, PointerEventData eventData)
    {
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;
        if (card.cardState != CardState.Default) return;
        canClick = false;

        bool isThereASelectedCard = selectedCard != null;
        bool isThisCardAPlayerCard = card.Affinity == Affinity.Player;

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
        canClick = true;
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;
        if (selectedCard == null) return;

        bool isThisCardAPlayerCard = card.Affinity == Affinity.Player;
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

        if (droppedCard.Affinity == Affinity.Enemy)
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

        //Vector3 handPos = roomData.PlayerManager.hand.transform.position;
        //handPos.y += 0.5f;
        //roomData.PlayerManager.hand.transform.position = handPos;
        //roomData.PlayerManager.hand.ReorderPlaceholders(true);
    }

    public void HandleTriggerExit(Component sender, object data)
    {
        if (state is BattleInteractionState.Battle or BattleInteractionState.Setup) return;

        if (selectedCard == null) return;
        if (isArrowActive) return;

        PointerEventData eventData = (PointerEventData)data;
        HandCollisionDetector handCollDetector = (HandCollisionDetector)sender;
        Collider2D handColl = handCollDetector.coll;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPoint.z = handColl.transform.position.z;

        // Check if the worldPoint is within the targetCollider
        if (handColl.OverlapPoint(worldPoint)) return;

        EnableArrow();

        //Vector3 handPos = roomData.PlayerManager.hand.transform.position;
        //handPos.y -= 0.5f;
        //roomData.PlayerManager.hand.transform.position = handPos;
        //roomData.PlayerManager.hand.ReorderPlaceholders(true);
    }

    #endregion

}
