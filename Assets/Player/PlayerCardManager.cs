using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardManager : MonoBehaviour
{
    private RoomStateMachine _ctx;
    [field: SerializeField] public List<Card> ActiveCards { get; private set; } = new();
    private GameObject _cardPrefab;
    public HandManager Hand { get; private set; }
    [SerializeField] private Transform _cardSpawnPosition;
    private RunData _runData;
    public Codex Codex => _runData.playerCodex;
    private Deck Deck => _runData.playerDeck;

    private void Awake()
    {
        Hand = GetComponentInChildren<HandManager>();
        _runData = Locator.RunData;
        _cardPrefab = Locator.Prefabs.Card;
    }

    public void Initialize(RoomStateMachine ctx)
    {
        _ctx = ctx;
    }

    private CardBlueprint DrawCard()
    {
        CardBlueprint drawnBlueprint = Codex.GetCardOverride(Deck.DrawCard());
        return drawnBlueprint;
    }

    private void SpawnCardToHandByBlueprint(CardBlueprint blueprint)
    {
        Card card = CardSpawner.Instance.SpawnCard(blueprint, _cardSpawnPosition.position, transform, GameConstants.PLAYER_CARD_LAYER,
            CardInteractionType.Playable, Codex, _ctx.DataProvider);

        card.ChangeCardState(CardState.Draw);
        ActiveCards.Add(card);
        Hand.AddCardToHand(card);
        StartCoroutine(Hand.ResetCardsToPlaceholders());
        Tools.PlaySound("Card_Draw", card.transform);
    }


    public void RemoveCardFromManager(Card card)
    {
        ActiveCards.Remove(card);
        Hand.RemoveCardFromHand(card);
    }

    public void DrawCardToHand()
    {
        CardBlueprint blueprint = DrawCard();
        SpawnCardToHandByBlueprint(blueprint);
    }

    public void SpawnCardToHandByArchetype(CardArchetype archetype)
    {
        CardBlueprint blueprint = Codex.GetCardOverride(archetype);
        SpawnCardToHandByBlueprint(blueprint);
    }



    public IEnumerator DrawCardsToHand(int amount)
    {
        if (amount <= 0) yield break;

        for (int i = 0; i < amount; i++)
        {
            DrawCardToHand();
            yield return Tools.GetWait(0.25f);
        }
    }
}
