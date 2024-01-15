using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeckInstance
{
    public List<CardArchetype> cards = new();
    public int min;
    public int max;

    public DeckInstance(int min, int max, bool shouldShuffle)
    {
        this.min = min;
        this.max = max;
        Reinitialize(shouldShuffle);
    }

    public void Reinitialize(bool shouldShuffle)
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

        if (shouldShuffle)
        {
            Tools.ShuffleList(cards);
        }
    }
}
