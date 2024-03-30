using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

internal class Battle_AbilitySelected : BaseRoomState
{
    public Battle_AbilitySelected(RoomStateMachine sm, SMContext ctx) : base(sm, ctx)
    {
    }

    List<Card> PlayerCards => _sm.PlayerCardManager.ActiveCards;
    List<Card> EnemyCards => _sm.EnemyCardManager.ActiveEnemies;
    List<Card> AllCards => _sm.DataProvider.GetAllActiveCards();
    Ability CurrentAbility => _ctx.CurrentAbilitySelected;
    List<Card> CardsSelected => _ctx.CurrentCardsSelected;



    public override IEnumerator EnterState()
    {
        if (CardsSelected.Count >= CurrentAbility.TargetAmount)
        {
            MoveToAbilityState();
            yield break;
        }

        UpdateValidCards();
    }

    public override void OnAbilityClicked(AbilityManager abilityManager, Ability ability)
    {
        foreach (var card in CardsSelected)
        {
            DeselectCard(card);
        }

        if (ability == CurrentAbility)
        {
            foreach (var card in AllCards)
            {
                card.visualHandler.ToggleDarkOverlay(false);
            }

            SwitchTo(States.Idle());
        } 
        
        else
        {
            _ctx.CurrentAbilitySelected = ability;
            SwitchTo(States.AbilitySelected());
        }
    }

    private void UpdateValidCards()
    {
        List<Card> allCards = new();
        allCards.AddRange(PlayerCards);
        allCards.AddRange(EnemyCards);

        foreach (Card card in allCards)
        {
            if (!IsCardSelectable(card))
            {
                card.visualHandler.ToggleDarkOverlay(true);
            } 
            
            else
            {
                card.visualHandler.ToggleDarkOverlay(false);
            }
        }
    }

    private bool IsCardSelectable(Card card)
    {
        bool canChoosePlayerCards = CurrentAbility.InteractableType.HasFlag(InteractableType.PlayerCard);
        bool canChooseEnemyCards = CurrentAbility.InteractableType.HasFlag(InteractableType.EnemyCard);
        bool isMinBlocked = CurrentAbility.TargetSingleRestriction is SingleTargetRestriction.BiggerThan;
        int minAmount = CurrentAbility.SingleRestrictionAmount;

        bool isColorBlocked = CurrentAbility.TargetTotalRestriction is TotalTargetRestriction.SameColor && CardsSelected.Count > 0;
        CardColor colorNeeded = CardColor.Black;
        if (isColorBlocked) colorNeeded = CardsSelected[0].cardColor;

        bool isCardAmountBlocked = (isMinBlocked && card.points <= minAmount);
        bool isCardColorBlocked = (isColorBlocked && card.cardColor != colorNeeded);

        bool isSelected;

        if (card.Affinity is Affinity.Player) isSelected = canChoosePlayerCards && !isCardAmountBlocked && !isCardColorBlocked;
        else isSelected = canChooseEnemyCards && !isCardAmountBlocked && !isCardColorBlocked;

        return isSelected;
    }

    public override void HandlePlayerCardPointerClick(Card card, PointerEventData eventData)
    {
        if (!IsCardSelectable(card)) return;

        if (!CardsSelected.Contains(card))
        {
            SelectCard(card);
        }

        else
        {
            DeselectCard(card);
        }

        SwitchTo(States.AbilitySelected());
    }

    public override void HandlePlayerCardPointerEnter(Card card, PointerEventData eventData)
    {
        if (!IsCardSelectable(card)) return;

        Events.ShowTooltip.Raise(_sm, card);

        bool isThisCardAPlayerCard = card.Affinity == Affinity.Player;

        if (isThisCardAPlayerCard)
            card.movement.Highlight();

        else
            card.visualHandler.Animate("Jiggle");
    }

    public override void HandlePlayerCardPointerExit(Card card, PointerEventData eventData)
    {
        if (!IsCardSelectable(card)) return;

        Events.HideTooltip.Raise(_sm, card);

        bool isThisCardSelected = CardsSelected.Contains(card);
        if (isThisCardSelected) return;

        card.movement.Dehighlight();
    }

    private void SelectCard(Card card)
    {
        CardsSelected.Add(card);
        card.movement.Highlight();
        card.visualHandler.EnableOutline(PaletteColor.white);
    }

    private void DeselectCard(Card card)
    {
        CardsSelected.Remove(card);
        card.movement.Dehighlight();
        card.visualHandler.DisableOutline();
    }

    public void MoveToAbilityState()
    {
        Events.HideTooltip.Raise();

        switch (CurrentAbility.Type)
        {
            case AbilityType.Split:
                SwitchTo(States.ApplySplitAbility());
                break;
            case AbilityType.Merge:
                SwitchTo(States.ApplyMergeAbility());
                break;
            case AbilityType.Paint:
                SwitchTo(States.ApplyAbilityEffect());
                break;
        }
    }
}