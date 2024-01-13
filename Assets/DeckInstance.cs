using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeckInstance
{
    public List<CardArchetype> cards = new();

    public DeckInstance()
    {
        Reinitialize();
    }

    public void Reinitialize()
    {
        cards?.Clear();

        for (int i = 1; i <= 10; i++)
        {
            cards.Add(new CardArchetype(i, CardColor.Black));
        }

        for (int i = 1; i <= 10; i++)
        {
            cards.Add(new CardArchetype(i, CardColor.Red));
        }
    }
}
