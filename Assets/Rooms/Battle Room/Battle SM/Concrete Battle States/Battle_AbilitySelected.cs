using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

internal class Battle_AbilitySelected : BaseBattleRoomState
{
    List<Card> PlayerCards => _ctx.PlayerCardManager.ActiveCards;
    List<Card> EnemyCards => _ctx.EnemyCardManager.ActiveEnemies;
    Ability Ability => _ctx.Ctx.CurrentAbilitySelected;
    List<Card> CardsSelected => _ctx.Ctx.CurrentCardsSelected;

    public Battle_AbilitySelected(BattleStateMachine ctx) : base(ctx)
    {
    }

    public override IEnumerator EnterState()
    {
        if (CardsSelected.Count >= Ability.TargetAmount)
        {
            MoveToAbilityState();
            yield break;
        }

        UpdateValidCards();
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
        bool canChoosePlayerCards = Ability.InteractableType.HasFlag(InteractableType.PlayerCard);
        bool canChooseEnemyCards = Ability.InteractableType.HasFlag(InteractableType.EnemyCard);
        bool isMinBlocked = Ability.TargetSingleRestriction is SingleTargetRestriction.BiggerThan;
        int minAmount = Ability.SingleRestrictionAmount;

        bool isColorBlocked = Ability.TargetTotalRestriction is TotalTargetRestriction.SameColor && CardsSelected.Count > 0;
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

        _ctx.SwitchState(_ctx.States.AbilitySelected());
    }

    public override void HandlePlayerCardPointerEnter(Card card, PointerEventData eventData)
    {
        if (!IsCardSelectable(card)) return;

        _ctx.Events.ShowTooltip.Raise(_ctx, card);

        bool isThisCardAPlayerCard = card.Affinity == Affinity.Player;

        if (isThisCardAPlayerCard)
            card.movement.Highlight();

        else
            card.visualHandler.Animate("Jiggle");
    }

    public override void HandlePlayerCardPointerExit(Card card, PointerEventData eventData)
    {
        if (!IsCardSelectable(card)) return;

        _ctx.Events.HideTooltip.Raise(_ctx, card);

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
        _ctx.Events.HideTooltip.Raise();

        switch (Ability.Type)
        {
            case AbilityType.Split:
                _ctx.SwitchState(_ctx.States.ApplySplitAbility());
                break;
            case AbilityType.Merge:
                _ctx.SwitchState(_ctx.States.ApplyMergeAbility());
                break;
            case AbilityType.Paint:
                _ctx.SwitchState(_ctx.States.ApplyAbilityEffect());
                break;
        }
    }
}