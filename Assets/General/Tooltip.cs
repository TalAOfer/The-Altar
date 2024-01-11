using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardArchetype;
    [SerializeField] private TextMeshProUGUI cardDescription;

    public void InitializeTooltip(Card card)
    {
        cardName.text = card.currentOverride.cardName;
        cardArchetype.text = card.points.ToString() + " of " + GetSymbolName(card);
        cardDescription.text = card.currentOverride.description;
    }

    private string GetSymbolName(Card card)
    {
        string symbolName = "";
        switch (card.cardOwner)
        {
            case CardOwner.Player:
                symbolName = card.cardColor == CardColor.Red ? "Hearts" : "Clubs";
                break;
            case CardOwner.Enemy:
                symbolName = card.cardColor == CardColor.Red ? "Diamonds" : "Spades";
                break;
        }
        return symbolName;
    }
}
