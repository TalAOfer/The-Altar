using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Deck deckRecipe;
    [SerializeField] private MapConfigs mapConfig;

    [SerializeField] private GameObject upsideDownCardPrefab;
    [SerializeField] private GameObject revealedCardPrefab;

    [SerializeField] private HandManager handManager;

    [SerializeField] private Transform mapMasterContainer;
    [SerializeField] private Transform[] mapCardContainers;

    [SerializeField] private Vector3 outOfScreenBoundsPosition;

    private List<CardBlueprint> deck;
    [SerializeField] private Transform deckTransform;
    [SerializeField] private int cardStartingAmount;
    [SerializeField] private float mapScale;

    private void Awake()
    {
        deck = new List<CardBlueprint>(deckRecipe.cards);
        Tools.ShuffleList(deck);
    }


    private Card SpawnCard(CardBlueprint cardBlueprint, CardOwner cardOwner, int index, string sortingLayerName)
    {
        GameObject prefabToSpawn = cardOwner == CardOwner.Reward ? upsideDownCardPrefab : revealedCardPrefab;
        GameObject cardGO = Instantiate(prefabToSpawn, deckTransform.position, Quaternion.identity, mapMasterContainer);
        cardGO.transform.localScale = Vector3.one * mapScale;
        cardGO.name = cardBlueprint.name;

        Card card = cardGO.GetComponent<Card>();
        card.Init(cardBlueprint, cardOwner, index, sortingLayerName);

        return card;
    }

    [Button]
    public void DrawMap()
    {
        StartCoroutine(DealMap());
    }

    

    public CardBlueprint DrawCard()
    {
        if (deck.Count == 0)
        {
            throw new System.InvalidOperationException("No cards left in the deck.");
        }

        CardBlueprint drawnCard = deck[0];
        deck.RemoveAt(0); // Remove the card from the deck
        return drawnCard;
    }

    private IEnumerator DealMap()
    {
        //for (int i = 0; i < 9; i++)
        //{
        //    Card card = SpawnCard(DrawCard(), CardState.Reward, i, GameConstants.BOTTOM_MAP_LAYER);
        //    card.transform.position = mapCardContainers[i].position;
        //    yield return new WaitForSeconds(0.25f);
        //}
        yield return null;
        StartCoroutine(DealPlayer());
    }

    private IEnumerator DealPlayer()
    {
        for (int i = 0; i < cardStartingAmount; i++)
        {
            Card card = SpawnCard(DrawCard(), CardOwner.Player, i, GameConstants.HAND_LAYER);
            handManager.AddCardToHand(card);
            card.interactionHandler.SetNewDefaultLocation();
            yield return new WaitForSeconds(0.25f);
        }
        StartCoroutine(DealEnemies());
    }

    private IEnumerator DealEnemies()
    {
        int randForConfig = Random.Range(0, 8);
        Vector2Int randomConfig = mapConfig.options[randForConfig];

        for (int i = 0; i < 2; i++)
        {
            int containerIndex = (i == 0) ? randomConfig.x : randomConfig.y;
            Card card = SpawnCard(DrawCard(), CardOwner.Enemy, i, GameConstants.TOP_MAP_LAYER);
            card.transform.position = mapCardContainers[containerIndex].position;
            card.interactionHandler.SetNewDefaultLocation();
            yield return new WaitForSeconds(0.25f);
        }
    }


}
