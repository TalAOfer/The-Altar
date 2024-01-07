using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardArchetype;
    [SerializeField] private TextMeshProUGUI cardDescription;

    public void InitializeTooltip(CardBlueprint cardBlueprint)
    {
        cardName.text = cardBlueprint.cardName;
        cardArchetype.text = cardBlueprint.defaultPoints.ToString() + " of " + GetSymbolName(cardBlueprint);
        cardDescription.text = cardBlueprint.description;
    }

    private string GetSymbolName(CardBlueprint blueprint)
    {
        string symbolName = "";
        switch (blueprint.cardOwner)
        {
            case CardOwner.Player:
                symbolName = blueprint.cardColor == CardColor.Red ? "Hearts" : "Clubs";
                break;
            case CardOwner.Enemy:
                symbolName = blueprint.cardColor == CardColor.Red ? "Diamonds" : "Spades";
                break;
        }
        return symbolName;
    }
}
