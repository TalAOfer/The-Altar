using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private AllEvents events;
    public List<Card> activeCards = new();
    public HandManager hand;
    [SerializeField] private PlayerCardSpawner spawner;

    public void FillHandToMinimum()
    {
        StartCoroutine(FillHandRoutine());
    }

    public IEnumerator FillHandRoutine()
    {
        if (activeCards.Count < 3)
        {
            yield return new WaitForSeconds(0.35f);

            int amountOfCardsToDraw = 3 - activeCards.Count;
            for (int i = 0; i < amountOfCardsToDraw; i++)
            {
                CardBlueprint blueprint = spawner.DrawCard();
                Card card = spawner.SpawnCard(blueprint, GameConstants.HAND_LAYER);
                activeCards.Add(card);
                hand.AddCardToHand(card);
                yield return new WaitForSeconds(0.25f);
            }
        }

        events.SetGameState.Raise(this, GameState.Idle);
    }

    public void SetAllCardState(CardState newState)
    {
        foreach (Card card in activeCards)
        {
            card.ChangeCardState(newState);
        }
    }

    public void DrawCardToHand()
    {
        Card card = spawner.SpawnCard(spawner.DrawCard(), GameConstants.HAND_LAYER);
        activeCards.Add(card);
        hand.AddCardToHand(card);
    }

    public void SpawnCardToHandByArchetype(CardArchetype archetype)
    {
        CardBlueprint blueprint = spawner.Codex.GetCardOverride(archetype);
        Card card = spawner.SpawnCard(blueprint, GameConstants.HAND_LAYER);
        activeCards.Add(card);
        hand.AddCardToHand(card);
    }
}
