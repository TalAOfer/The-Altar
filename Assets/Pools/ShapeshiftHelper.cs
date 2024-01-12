using UnityEngine;

//[CreateAssetMenu(menuName = "ShapeshiftHelper")]
public class ShapeshiftHelper : ScriptableObject
{
    public CardBlueprint bones;
    
    public DeckBlueprint Hearts;
    public DeckBlueprint Clubs;

    public DeckBlueprint Diamonds;
    public DeckBlueprint Spades;

    public CardBlueprint GetCardBlueprint(CardOwner cardOwner, int currentPoints, CardColor cardColor)
    {
        if (currentPoints == 0)
        {
            return bones;
        } 
        
        else
        {
            DeckBlueprint deck = null;
            switch (cardOwner)
            {
                case CardOwner.Player:
                    deck = cardColor == CardColor.Red ? Hearts : Spades;
                    break;
                case CardOwner.Enemy:
                    deck = cardColor == CardColor.Red ? Diamonds : Clubs;
                    break;
            }
            int index = currentPoints - 1;
            return deck.cards[index];
        }
    }
}
