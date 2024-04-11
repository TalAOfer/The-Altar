using System;
using System.Collections.Generic;

[Serializable]
public class Deck
{
    public List<CardArchetype> cards;
    public int min;
    public int max = 10;

    public Deck(int min, int max)
    {
        this.min = min;
        this.max = max;
        RepopulateAndShuffle();
    }

    public Deck(Deck deck)
    {
        this.min = deck.min;
        this.max = deck.max;

        // Initialize the cards list with a capacity equal to the original deck's cards count
        this.cards = new List<CardArchetype>(deck.cards.Count);

        // Deep copy of the CardArchetype instances if necessary
        foreach (CardArchetype card in deck.cards)
        {
            // Assuming CardArchetype is a class with a copy constructor or a clone method
            // This is necessary to avoid reference issues between the decks
            this.cards.Add(new CardArchetype(card.points, card.color));
        }
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
        if (cards == null) cards = new();
        else cards.Clear();

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
