using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Deck playerDeckRecipe;
    [SerializeField] private bool shufflePlayerDeck;
    private List<CardBlueprint> playerDeck;
    [SerializeField] private Deck enemyDeckRecipe;
    [SerializeField] private bool shuffleEnemyDeck;
    private List<CardBlueprint> enemyDeck;

    [SerializeField] private MapConfigs mapConfig;

    [SerializeField] private GameObject revealedCardPrefab;

    [SerializeField] private HandManager handManager;

    [SerializeField] private Transform mapMasterContainer;
    [SerializeField] private MapGridArranger grid;

    [SerializeField] private Vector3 outOfScreenBoundsPosition;

    [SerializeField] private Transform deckTransform;
    [SerializeField] private int cardStartingAmount;
    [SerializeField] private float mapScale;
    [SerializeField] private float handScale;

    private void Awake()
    {
        playerDeck = new List<CardBlueprint>(playerDeckRecipe.cards);
        if (shufflePlayerDeck) Tools.ShuffleList(playerDeck);
        enemyDeck = new List<CardBlueprint>(enemyDeckRecipe.cards);
        if (shuffleEnemyDeck) Tools.ShuffleList(enemyDeck);
    }

    public CardBlueprint DrawCard(CardOwner cardOwner)
    {
        List<CardBlueprint> deck = cardOwner == CardOwner.Player ? playerDeck : enemyDeck;

        if (deck.Count == 0)
        {
            throw new System.InvalidOperationException("No cards left in the deck.");
        }

        CardBlueprint drawnCard = deck[0];
        deck.RemoveAt(0); // Remove the card from the deck
        return drawnCard;
    }

    private Card SpawnCard(CardBlueprint cardBlueprintDrawn, int index, string sortingLayerName)
    {
        GameObject cardGO = Instantiate(revealedCardPrefab, deckTransform.position, Quaternion.identity, mapMasterContainer);
        cardGO.transform.localScale = Vector3.one * mapScale;
        cardGO.name = cardBlueprintDrawn.name;

        Card card = cardGO.GetComponent<Card>();
        card.Init(cardBlueprintDrawn, index, sortingLayerName);

        return card;
    }


    [Button]
    public void DrawMap()
    {
        StartCoroutine(DealMap());
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
            Card card = SpawnCard(DrawCard(CardOwner.Player), i, GameConstants.HAND_LAYER);
            handManager.AddCardToHand(card);

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
            bool firstCard = (i == 0);
            int firstCardConfig = randomConfig.x;
            int secondCardConfig = randomConfig.y;

            int containerIndex = firstCard ? firstCardConfig : secondCardConfig;
            SpawnEnemyCard(containerIndex);
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void SpawnEnemyCard(int containerIndex)
    {
        Card card = SpawnCard(DrawCard(CardOwner.Enemy), containerIndex, GameConstants.TOP_MAP_LAYER);
        card.transform.position = grid.MapSlots[containerIndex].transform.position;
        //StartCoroutine(card.interactionHandler.MoveCardToPositionOverTime(grid.MapSlots[containerIndex].transform.position, 1f));
        StartCoroutine(grid.MapSlots[containerIndex].SetSlotState(MapSlotState.Occupied));
        card.interactionHandler.SetNewDefaultLocation(card.transform.position, card.transform.localScale, card.transform.eulerAngles);
    }

    public void OnMapSlotClicked(Component sender, object data)
    {
        int slotIndex = (int)data;
        SpawnEnemyCard(slotIndex);
    }

    public void OnSpawnCardToHand(Component sender, object data)
    {
        ActiveEffect askerEffect = (ActiveEffect)sender;
        CardBlueprint cardToSpawn = (CardBlueprint)data;
       
        //TODO: think about who decides on the indexes
        Card card = SpawnCard(cardToSpawn, 0, GameConstants.HAND_LAYER);
        handManager.AddCardToHand(card);

        StartCoroutine(askerEffect.HandleResponse(card));
    }

    public void OnDrawCardToHand(Component sender, object data)
    {
        ActiveEffect askerEffect = (ActiveEffect)sender;

        Card card = SpawnCard(DrawCard(CardOwner.Player), 0, GameConstants.HAND_LAYER);
        handManager.AddCardToHand(card);

        StartCoroutine(askerEffect.HandleResponse(card));
    }

}
