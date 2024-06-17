using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CodexUI : MonoBehaviour
{

    private Codex _codex;
    [SerializeField] private List<CodexUICard> cards;

    [SerializeField] bool ManualyInitializeCards;
    [SerializeField] private Palette _palette;
    [SerializeField] private SpriteFolder _sprites;
    [SerializeField] CodexBlueprint _codexBlueprint;

    public void Initialize(Codex codex)
    {
        _codex = codex;
    }

    [Button]
    public void ManuallyInitializeCards()
    {
        for (int i = 0; i < 20; i++)
        {
            var uiCard = cards[i];
            int amount = (i / 2) + 1; // Calculate amount based on the current index
            CardColor color = i % 2 == 0 ? CardColor.Black : CardColor.Red; // Alternate color based on the current index
            string cardColorName = color is CardColor.Black ? "B" : "R";
            string cardName = amount.ToString() + cardColorName;
            CardArchetype archetype = new(amount, color);
            CardBlueprint mask = _codexBlueprint.GetBlueprintByArchetype(archetype);
            Sprite icon = mask.cardSprite;
            Sprite digit = _sprites.numbers[mask.Archetype.points];
            Sprite symbol = GetCardSymbol(mask);
            Color spritesColor = color is CardColor.Black ? _palette.darkPurple : _palette.lightRed; 
            uiCard.ManuallyInitialize(cardName, archetype, spritesColor, icon, digit, symbol);
        }
    }

    public void CheckMaskUpdates()
    {
        for (int i = 0; i < 20; i++)
        {
            var uiCard = cards[i];
            int amount = (i / 2) + 1; // Calculate amount based on the current index
            CardColor color = i % 2 == 0 ? CardColor.Black : CardColor.Red; // Alternate color based on the current index
        }


        foreach (var card in cards) 
        {
            CardBlueprint currentMask = card.Mask;
            CardBlueprint matchingCodexMask = _codex.GetCardOverride(card.Archetype); 
            if (currentMask != matchingCodexMask) 
            {
                card.UpdateCardUI(matchingCodexMask);
            }
        }
    }

    private Sprite GetCardSymbol(CardBlueprint mask)
    {
        Sprite sprite = null;
        switch (mask.Affinity)
        {
            case Affinity.Player:
                sprite = mask.Archetype.color == CardColor.Red ? _sprites.hearts : _sprites.spades;
                break;
            case Affinity.Enemy:
                sprite = mask.Archetype.color == CardColor.Red ? _sprites.diamonds : _sprites.clubs;
                break;
        }

        return sprite;
    }
}
