using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _cardName;
    [SerializeField] private TextMeshProUGUI _cardArchetype;
    [SerializeField] private TextMeshProUGUI _cardDescription;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _digit;
    [SerializeField] private Image _symbol;
    [SerializeField] private SpriteFolder _sprites;
    [SerializeField] private Palette _palette;

    [SerializeField] private GameObject _higherBeingWindow;
    [SerializeField] private GameObject _bloodthirstWindow;
    [SerializeField] private GameObject _tauntWindow;

    private bool IsHigherBeing(Card card) => card.Mask.SpecialEffects.HasFlag(SpecialEffects.HigherBeing);
    private bool IsTaunt(Card card) => card.Mask.SpecialEffects.HasFlag(SpecialEffects.Taunt);

    public void InitializeTooltip(Card card)
    {
        _cardName.text = card.Mask.cardName;
        _cardArchetype.text = Tools.GetCardNameByArchetype(new CardArchetype(card.points, card.cardColor), card.Affinity);
        _cardDescription.text = !card.Mask.isDefault ? GetDescription(card) : "No effect.";
        _icon.sprite = card.Mask.cardSprite;
        SetCardSymbol(card);
        SetSpritesColor(card);
        InitializeSpecialEffectWindows(card);
    }

    private string GetDescription(Card card)
    {
        string description = "";
        if (IsHigherBeing(card)) description += "<b>Higher-Being.</b>\n";
        if (IsTaunt(card)) description += "<b>Taunt.</b>\n";
        description += card.Mask.Description;
        return description;
    }

    private void InitializeSpecialEffectWindows(Card card)
    {
        _higherBeingWindow.SetActive(card.Mask.SpecialEffects.HasFlag(SpecialEffects.HigherBeing));
        _tauntWindow.SetActive(card.Mask.SpecialEffects.HasFlag(SpecialEffects.Taunt));
        _bloodthirstWindow.SetActive(false);
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
                sprite = card.cardColor == CardColor.Red ? _sprites.hearts : _sprites.spades;
                break;
            case Affinity.Enemy:
                sprite = card.cardColor == CardColor.Red ? _sprites.diamonds : _sprites.clubs;
                break;
        }

        _symbol.sprite = sprite;
    }

    public void SetSpritesColor(Card card)
    {
        Color color = card.cardColor == CardColor.Black ? _palette.darkPurple : _palette.lightRed;
        color.a = 0.1f;
        _icon.color = color;
        _digit.color = color;
        _symbol.color = color;
    }

    public void SetNumberSprites(Card card)
    {
        _digit.sprite = _sprites.numbers[card.Mask.Archetype.points];
    }
}
