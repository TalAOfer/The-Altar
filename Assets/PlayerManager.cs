using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private AllEvents events;
    public List<Card> activeCards = new();
    [SerializeField] private HandManager hand;
    [SerializeField] private PlayerCardSpawner spawner;

    public void FillHandToMinimum()
    {
        if (activeCards.Count >= 3) return;
        StartCoroutine(FillHandRoutine());
    }

    public IEnumerator FillHandRoutine()
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

        events.SetGameState.Raise(this, GameState.Idle);
    }

    public void SetAllCardState(CardState newState)
    {
        foreach (Card card in activeCards)
        {
            card.ChangeCardState(newState);
        }
    }
}
