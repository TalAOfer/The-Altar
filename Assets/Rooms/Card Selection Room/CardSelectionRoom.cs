using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class CardSelectionRoom : Room
{
    [SerializeField] private AllEvents events;
    private FloorManager floorManager;
    [SerializeField] private RunData runData;
    [SerializeField] List<GameObject> texts;

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject link;

    [SerializeField] private Transform spawnContainer;

    [SerializeField] private float defaultSpacingX;
    [SerializeField] private float defaultSpacingY;

    public List<LinkedCards> linkedCardsList = new();

    private Vector3 temp = new();

    [Button]
    public override void InitializeRoom(FloorManager floorManager, RoomBlueprint roomBlueprint)
    {
        base.InitializeRoom(floorManager, roomBlueprint);

        this.floorManager = floorManager;

        events.SetGameState.Raise(this, GameState.ChooseNewBlueprints);

        ToggleTexts(true);

        linkedCardsList.Clear();
        SpawnLinkedCards(roomBlueprint);
    }
    public override void OnRoomFinishedLerping()
    {
        base.OnRoomFinishedLerping();

        foreach (LinkedCards linkedCards in linkedCardsList)
        {
            linkedCards.playerCard.interactionHandler.SetNewDefaultLocation(linkedCards.playerCard.transform.position, Vector3.one, Vector3.zero);
            linkedCards.enemyCard.interactionHandler.SetNewDefaultLocation(linkedCards.enemyCard.transform.position, Vector3.one, Vector3.zero);
        }
    }
    public override void OnRoomFinished()
    {
        base.OnRoomFinished();

        ToggleTexts(false);
    }

    public void SpawnLinkedCards(RoomBlueprint roomBlueprint)
    {
        int minDraw = roomBlueprint.minDraw;
        int maxDraw = roomBlueprint.maxDraw;
        int cardAmount = roomBlueprint.amountOfOptions;

        float startOffset = -defaultSpacingX * (cardAmount - 1) / 2;

        for (int i = 0; i < cardAmount; i++)
        {
            CardBlueprint playerDrawnBlueprint = runData.playerPool.GetRandomCardByPoints(minDraw, maxDraw);
            Card playerCard = SpawnCard(playerDrawnBlueprint, GameConstants.TOP_BATTLE_LAYER, runData.playerCodex);
            playerCard.index = i;
            temp.x = startOffset + i * defaultSpacingX;
            temp.y = playerCard.transform.position.y - defaultSpacingY;
            playerCard.transform.position = temp;
            playerCard.interactionHandler.SetNewDefaultLocation(playerCard.transform.position, playerCard.transform.localScale, Vector3.zero);

            CardBlueprint enemyDrawnBlueprint = runData.enemyPool.GetRandomCardByPoints(minDraw, maxDraw);
            Card enemyCard = SpawnCard(enemyDrawnBlueprint, GameConstants.TOP_BATTLE_LAYER, runData.enemyCodex);
            enemyCard.index = i;
            temp.y = enemyCard.transform.position.y + defaultSpacingY;
            enemyCard.transform.position = temp;
            enemyCard.interactionHandler.SetNewDefaultLocation(playerCard.transform.position, playerCard.transform.localScale, Vector3.zero);

            GameObject linkGo = Instantiate(link, transform.position, Quaternion.identity, spawnContainer);
            linkGo.transform.localPosition = new Vector3(temp.x, 0, 0);


            linkedCardsList.Add(new(playerCard, enemyCard, linkGo));
        }
    }


    private void ToggleTexts(bool enable)
    {
        foreach (GameObject text in texts)
        {
            text.SetActive(enable);
        }
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
        events.HideTooltip.Raise(this, null);
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
            Destroy(linkedCards.link);
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

[Serializable]
public class LinkedCards
{
    public Card playerCard;
    public Card enemyCard;
    public bool wasChosen;
    public GameObject link;

    public LinkedCards(Card playerCard, Card enemyCard, GameObject link)
    {
        wasChosen = false;
        this.playerCard = playerCard;
        this.enemyCard = enemyCard;
        this.link = link;
    }
}
