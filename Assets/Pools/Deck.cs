using System;
using System.Collections.Generic;

[Serializable]
public class Deck
{
    public List<CardArchetype> cards = new();
    public int min;
    public int max = 10;

    public Deck(int min, int max)
    {
        this.min = min;
        this.max = max;
        RepopulateAndShuffle();
    }

    public CardArchetype DrawCard()
    {
        if (cards.Count == 0)
        {
            RepopulateAndShuffle();
        }

        CardArchetype drawnArchetype = cards[0];
        cards.RemoveAt(0); // Remove the card from the deck
        return drawnArchetype;
    }

    public void RepopulateAndShuffle()
    {
        cards?.Clear();

        for (int i = min; i <= max; i++)
        {
            cards.Add(new CardArchetype(i, CardColor.Black));
        }

        for (int i = min; i <= max; i++)
        {
            cards.Add(new CardArchetype(i, CardColor.Red));
        }

        Tools.ShuffleList(cards);
    }
}
