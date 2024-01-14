using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class CardSelectionRoom : Room
{
    [SerializeField] private AllEvents events;
    private FloorManager floorManager;
    [SerializeField] private RunData runData;

    [SerializeField] private GameObject cardPrefab;

    [SerializeField] private Transform spawnContainer;

    [SerializeField] private float defaultSpacingX;
    [SerializeField] private float defaultSpacingY;

    private List<LinkedCards> linkedCardsList = new();

    private Vector3 temp = new();

    [Button]
    public override void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint)
    {
        this.floorManager = floorManager;
        int cardAmount = 2;

        events.SetGameState.Raise(this, GameState.ChooseNewBlueprints);

        linkedCardsList.Clear();

        float startOffset = -defaultSpacingX * (cardAmount - 1) / 2;

        for (int i = 0; i < cardAmount; i++)
        {
            CardBlueprint playerDrawnBlueprint = runData.playerPool.GetRandomCardByPoints(roomBlueprint.minDraw, roomBlueprint.maxDraw);
            Card playerCard = SpawnCard(playerDrawnBlueprint, GameConstants.BOTTOM_BATTLE_LAYER, runData.playerCodex);
            playerCard.index = i;
            temp.x = startOffset + i * defaultSpacingX;
            temp.y = playerCard.transform.position.y - defaultSpacingY;
            playerCard.transform.position = temp;
            playerCard.interactionHandler.SetNewDefaultLocation(playerCard.transform.position, playerCard.transform.localScale, Vector3.zero);

            CardBlueprint enemyDrawnBlueprint = runData.enemyPool.GetRandomCardByPoints(roomBlueprint.minDraw, roomBlueprint.maxDraw);
            Card enemyCard = SpawnCard(enemyDrawnBlueprint, GameConstants.BOTTOM_BATTLE_LAYER, runData.enemyCodex);
            enemyCard.index = i;
            temp.y = enemyCard.transform.position.y + defaultSpacingY;
            enemyCard.transform.position = temp;
            enemyCard.interactionHandler.SetNewDefaultLocation(playerCard.transform.position, playerCard.transform.localScale, Vector3.zero);

            linkedCardsList.Add(new(playerCard, enemyCard));
        }
    }
    public override void OnRoomFinished()
    {
        throw new System.NotImplementedException();
    }

    public void HandleCardClick(Component sender, object data)
    {
        Card clickedCard = data as Card;
        HandleChoice(clickedCard.index);
    }

    public void HandleChoice(int index)
    {
        LinkedCards linkedCards = linkedCardsList[index];

        linkedCards.wasChosen = true;
        CardBlueprint chosenPlayerBlueprint = linkedCards.playerCard.currentOverride;
        CardBlueprint chosenEnemyBlueprint = linkedCards.enemyCard.currentOverride;

        runData.playerCodex.OverrideCard(chosenPlayerBlueprint);
        runData.enemyCodex.OverrideCard(chosenEnemyBlueprint);

        RemoveAllCards();
        floorManager.NextRoom();
    }

    [Button]
    public void RemoveAllCards()
    {
        List<LinkedCards> linkedCardsToDelete = new(linkedCardsList);
        foreach (LinkedCards linkedCards in linkedCardsToDelete)
        {
            if (!linkedCards.wasChosen)
            {
                runData.playerPool.ReturnBlueprintToPool(linkedCards.playerCard.currentOverride);
                runData.enemyPool.ReturnBlueprintToPool(linkedCards.enemyCard.currentOverride);
            }

            linkedCardsList.Remove(linkedCards);

            Destroy(linkedCards.playerCard.gameObject);
            Destroy(linkedCards.enemyCard.gameObject);
        }
    }

    public Card SpawnCard(CardBlueprint cardBlueprint, string sortingLayerName, BlueprintPoolInstance codex)
    {
        GameObject cardGO = Instantiate(cardPrefab, transform.position, Quaternion.identity, spawnContainer);
        cardGO.name = cardBlueprint.name;
        Card card = cardGO.GetComponent<Card>();
        card.Init(codex, cardBlueprint, sortingLayerName);

        return card;
    }

}

public class LinkedCards
{
    public Card playerCard;
    public Card enemyCard;
    public bool wasChosen;

    public LinkedCards(Card playerCard, Card enemyCard)
    {
        wasChosen = false;
        this.playerCard = playerCard;
        this.enemyCard = enemyCard;
    }
}
