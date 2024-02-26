using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardArchetype;
    [SerializeField] private TextMeshProUGUI cardDescription;
    [SerializeField] private Image icon;
    [SerializeField] private Image digit;
    [SerializeField] private Image symbol;
    [SerializeField] private SpriteFolder sprites;
    [SerializeField] private Palette palette;

    [SerializeField] private GameObject higherBeingWindow;
    [SerializeField] private GameObject bloodthirstWindow;
    [SerializeField] private GameObject meditateWindow;

    private bool IsHigherBeing(Card card) => card.Mask.SpecialEffects.HasFlag(SpecialEffects.HigherBeing);
    private bool IsBloodthirst(Card card) => card.Mask.SpecialEffects.HasFlag(SpecialEffects.Bloodthirst);
    private bool IsMeditate(Card card) => card.Mask.SpecialEffects.HasFlag(SpecialEffects.Meditate);

    public void InitializeTooltip(Card card)
    {
        cardName.text = card.Mask.cardName;
        cardArchetype.text = Tools.GetCardNameByArchetype(new CardArchetype(card.points, card.cardColor), card.Affinity);
        cardDescription.text = !card.Mask.isDefault ? GetDescription(card) : "No effect.";
        icon.sprite = card.Mask.cardSprite;
        SetCardSymbol(card);
        SetSpritesColor(card);
        InitializeSpecialEffectWindows(card);
    }

    private string GetDescription(Card card)
    {
        string description = "";
        if (IsHigherBeing(card)) description += "<b>Higher-Being.</b>\n";
        if (IsMeditate(card)) description += "<b>Meditate.</b>\n";
        if (IsBloodthirst(card)) description += "<b>Bloodthirst.</b>\n";
        description += card.Mask.Description;
        return description;
    }

    private void InitializeSpecialEffectWindows(Card card)
    {
        higherBeingWindow.SetActive(card.Mask.SpecialEffects.HasFlag(SpecialEffects.HigherBeing));
        bloodthirstWindow.SetActive(card.Mask.SpecialEffects.HasFlag(SpecialEffects.Bloodthirst));
        meditateWindow.SetActive(card.Mask.SpecialEffects.HasFlag(SpecialEffects.Meditate));
        
    }

    private string GetSymbolName(Card card)
    {
        string symbolName = "";
        switch (card.Affinity)
        {
            case Affinity.Player:
                symbolName = card.cardColor == CardColor.Red ? "Hearts" : "Clubs";
                break;
            case Affinity.Enemy:
                symbolName = card.cardColor == CardColor.Red ? "Diamonds" : "Spades";
                break;
        }

        return symbolName;
    }

    private void SetCardSymbol(Card card)
    {
        Sprite sprite = null;
        switch (card.Affinity)
        {
            case Affinity.Player:
                sprite = card.cardColor == CardColor.Red ? sprites.hearts : sprites.spades;
                break;
            case Affinity.Enemy:
                sprite = card.cardColor == CardColor.Red ? sprites.diamonds : sprites.clubs;
                break;
        }

        symbol.sprite = sprite;
    }

    public void SetSpritesColor(Card card)
    {
        Color color = card.cardColor == CardColor.Black ? palette.darkPurple : palette.lightRed;
        color.a = 0.1f;
        icon.color = color;
        digit.color = color;
        symbol.color = color;
    }

    public void SetNumberSprites(Card card)
    {
        digit.sprite = sprites.numbers[card.Mask.Archetype.points];
    }
}
