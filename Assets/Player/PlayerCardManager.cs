using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardManager : MonoBehaviour
{
    private RoomStateMachine _sm;
    [field: SerializeField] public List<Card> ActiveCards { get; private set; } = new();
    public HandManager Hand { get; private set; }
    [SerializeField] private Transform _cardSpawnPosition;
    public Codex Codex => _sm.PlayerCodex;

    private void Awake()
    {
        Hand = GetComponentInChildren<HandManager>();
    }

    public void Initialize(RoomStateMachine ctx)
    {
        _sm = ctx;
    }

    //private CardBlueprint DrawCard()
    //{
    //    CardBlueprint drawnBlueprint = Codex.GetCardOverride(Deck.DrawCard());
    //    return drawnBlueprint;
    //}

    //public void DrawCardToHand()
    //{
    //    CardBlueprint blueprint = DrawCard();
    //    SpawnCardToHandByBlueprint(blueprint);
    //}
    //public IEnumerator DrawCardsToHand(int amount)
    //{
    //    if (amount <= 0) yield break;

    //    for (int i = 0; i < amount; i++)
    //    {
    //        DrawCardToHand();
    //        yield return Tools.GetWait(0.25f);
    //    }
    //}
    public void SpawnCardToHandByBlueprint(CardBlueprint blueprint)
    {
        Card card = CardSpawner.Instance.SpawnCard(blueprint, _cardSpawnPosition.position, transform, GameConstants.PLAYER_CARD_LAYER,
            CardInteractionType.Playable, Codex, _sm.DataProvider);

        card.ChangeCardState(CardState.Draw);
        ActiveCards.Add(card);
        Hand.AddCardToHand(card);
        StartCoroutine(Hand.ResetCardsToPlaceholders());
        Tools.PlaySound("Card_Draw", card.transform);
    }
    public IEnumerator SpawnCardsToHandByBlueprint(List<CardBlueprint> blueprints)
    {
        foreach(CardBlueprint blueprint in blueprints)
        {
            SpawnCardToHandByBlueprint(blueprint);
            yield return new WaitForSeconds(0.25f);
        }
    }


    public void RemoveCardFromManager(Card card)
    {
        ActiveCards.Remove(card);
        Hand.RemoveCardFromHand(card);
    }


    public void SpawnCardToHandByArchetype(CardArchetype archetype)
    {
        CardBlueprint blueprint = Codex.GetCardOverride(archetype);
        SpawnCardToHandByBlueprint(blueprint);
    }

}
