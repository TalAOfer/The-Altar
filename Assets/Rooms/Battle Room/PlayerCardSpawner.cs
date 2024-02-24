using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    public Transform spawnContainer;
    public Transform spawnPos;

    [SerializeField] private RunData runData;
    public Codex Codex => runData.playerCodex;
    private Deck Deck => runData.playerDeck;

    public CardBlueprint DrawCard()
    {
        CardBlueprint drawnBlueprint = Codex.GetCardOverride(Deck.DrawCard());
        return drawnBlueprint;
    }

    public Card SpawnCard(CardBlueprint cardBlueprint, string sortingLayerName)
    {
        GameObject cardGO = Instantiate(cardPrefab, spawnPos.position, Quaternion.identity, spawnContainer);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(Codex, cardBlueprint, sortingLayerName, CardInteractionType.Playable);

        return card;
    }
}
