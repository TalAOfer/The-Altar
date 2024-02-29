using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private EventRegistry _events;
    [field: SerializeField] public List<Card> ActiveCards { get; private set; } = new();
    public HandManager Hand { get; private set; }
    private PlayerCardSpawner _spawner;

    private void Awake()
    {
        Hand = GetComponentInChildren<HandManager>();
        _spawner = GetComponent<PlayerCardSpawner>();
        _events = Locator.Events;
    }

    public void RemoveCardFromManager(Card card)
    {
        ActiveCards.Remove(card);
    }

    public void FillHandToMinimum()
    {
        StartCoroutine(FillHandRoutine());
    }

    public IEnumerator FillHandRoutine()
    {
        if (ActiveCards.Count < 3)
        {
            yield return Tools.GetWait(0.35f);

            int amountOfCardsToDraw = 3 - ActiveCards.Count;
            for (int i = 0; i < amountOfCardsToDraw; i++)
            {
                DrawCardToHand();
                yield return Tools.GetWait(0.25f);
            }
        }

        _events.OnFinishedHandFill.Raise();
        _events.SetGameState.Raise(this, GameState.Idle);
    }

    public void SetAllCardState(CardState newState)
    {
        foreach (Card card in ActiveCards)
        {
            card.ChangeCardState(newState);
        }
    }

    public void SetAllPlayerCardCollisions(bool enable)
    {
        foreach (Card card in ActiveCards)
        {
            card.interactionHandler.coll.enabled = enable;
        }
    }

    public void DrawCardToHand()
    {
        CardBlueprint blueprint = _spawner.DrawCard();
        SpawnCardToHand(blueprint);
    }

    public void SpawnCardToHandByArchetype(CardArchetype archetype)
    {
        CardBlueprint blueprint = _spawner.Codex.GetCardOverride(archetype);
        SpawnCardToHand(blueprint);
    }

    private void SpawnCardToHand(CardBlueprint blueprint)
    {
        Card card = _spawner.SpawnCard(blueprint, GameConstants.PLAYER_CARD_LAYER);
        card.ChangeCardState(CardState.Draw);
        ActiveCards.Add(card);
        Hand.AddCardToHand(card);
        Hand.ResetCardsToPlaceholders();
        Tools.PlaySound("Card_Draw", card.transform);
    }
}
