using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName = "ShapeshiftHelper")]
public class ShapeshiftHelper : ScriptableObject
{
    public CardBlueprint bones;
    
    public Deck Hearts;
    public Deck Clubs;

    public Deck Diamonds;
    public Deck Spades;

    public CardBlueprint GetCardBlueprint(CardOwner cardOwner, int currentPoints, CardColor cardColor)
    {
        if (currentPoints == 0)
        {
            return bones;
        } 
        
        else
        {
            Deck deck = null;
            switch (cardOwner)
            {
                case CardOwner.Player:
                    deck = cardColor == CardColor.Red ? Hearts : Clubs;
                    break;
                case CardOwner.Enemy:
                    deck = cardColor == CardColor.Red ? Diamonds : Spades;
                    break;
            }
            int index = currentPoints - 1;
            return deck.cards[index];
        }
    }
}
