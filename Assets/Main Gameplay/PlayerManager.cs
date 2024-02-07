using System.Collections;
using System.Collections.Generic;
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
                DrawCardToHand();
                yield return new WaitForSeconds(0.25f);
            }
        }

        events.OnFinishedHandFill.Raise();
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
        CardBlueprint blueprint = spawner.DrawCard();
        SpawnCardToHand(blueprint);
    }

    public void SpawnCardToHandByArchetype(CardArchetype archetype)
    {
        CardBlueprint blueprint = spawner.Codex.GetCardOverride(archetype);
        SpawnCardToHand(blueprint);
    }

    private void SpawnCardToHand(CardBlueprint blueprint)
    {
        Card card = spawner.SpawnCard(blueprint, GameConstants.PLAYER_CARD_LAYER);
        card.ChangeCardState(CardState.Draw);
        activeCards.Add(card);
        hand.AddCardToHand(card);
        hand.ResetCardsToPlaceholders();
    }
}
