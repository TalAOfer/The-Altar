using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShapeshiftHelper")]
public class ShapeshiftHelper : ScriptableObject
{
    public CardBlueprint bones;
    public Deck blackDeck;
    public Deck redDeck;

    public CardBlueprint GetCardBlueprint(int currentPoints, CardColor cardColor)
    {
        if (currentPoints == 0)
        {
            return bones;
        } 
        
        else
        {
            int index = currentPoints - 1;
            if (cardColor == CardColor.Black) return blackDeck.cards[index];
            else return redDeck.cards[index];
        }
    }
}
